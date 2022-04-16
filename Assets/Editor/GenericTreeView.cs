using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

namespace Editor
{
    public class GenericTreeView<T> : MultiColumnTreeView, IDisposable where T : TreeNode
    {
        private readonly GenericTreeViewColumn<T>[] Columns;

        private T? Root;

        private List<TreeViewItemData<T>>? RootItems;

        public GenericTreeView(GenericTreeViewColumn<T>[] columns)
        {
            if (columns == null)
                throw new ArgumentNullException(nameof(columns));

            if (columns.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(columns));

            Columns = columns;

            foreach (var column in columns)
            {
                this.columns.Add(column.GetColumn());
            }

            sortingEnabled = true;

            columnSortingChanged += OnColumnSortingChanged;

            SetRoot(null); // because good old bad habits prevail at Unity, ensure that their internal crap gets initialized
        }

        public void Dispose()
        {
            columnSortingChanged -= OnColumnSortingChanged;
        }

        public void SetRoot(T? node)
        {
            visible = node is not null; // prevent a NRE when clicking an empty tree, better than showing lonesome headers

            Root = node;

            RootItems = GetRootItems();

            SetRootItems(RootItems);

            Rebuild();
        }

        private void OnColumnSortingChanged()
            // BUG: Unity code monkeys raise this N headers + 2 times in a row... unless you hold a fucking modifier!
        {
            Debug.Log(nameof(OnColumnSortingChanged));

            // deep sorting screws ids and thus expanded nodes, save this info to restore it later on new hierarchy

            Profiler.BeginSample($"{nameof(OnColumnSortingChanged)}: {nameof(SaveExpandedNodes)}");
            var data = SaveExpandedNodes();
            Profiler.EndSample();

            Profiler.BeginSample($"{nameof(OnColumnSortingChanged)}: {nameof(GetRootItems)}");
            RootItems = GetRootItems(); // NOTE: this sorts the items
            Profiler.EndSample();

            Profiler.BeginSample($"{nameof(OnColumnSortingChanged)}: {nameof(SetRootItems)}");
            SetRootItems(RootItems);
            Profiler.EndSample();

            Profiler.BeginSample($"{nameof(OnColumnSortingChanged)}: {nameof(LoadExpandedNodes)}");
            LoadExpandedNodes(data);
            Profiler.EndSample();

            Profiler.BeginSample($"{nameof(OnColumnSortingChanged)}: {nameof(RefreshItems)}");
            RefreshItems();
            Profiler.EndSample();
        }

        private static List<TSource> Flatten<TSource>(IEnumerable<TSource> collection, Func<TSource, IEnumerable<TSource>> children)
        {
            var list = new List<TSource>();

            var stack = new Stack<TSource>();

            foreach (var source in collection)
            {
                stack.Push(source);
            }

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                list.Add(pop);

                foreach (var child in children(pop).Reverse())
                {
                    stack.Push(child);
                }
            }

            return list;
        }

        private TreeNodeState SaveExpandedNodes()
        {
            var controller = viewController;

            Profiler.BeginSample($"{nameof(SaveExpandedNodes)}: GetExpandedNodes");

            var items     = Flatten(RootItems!, s => s.children);
            var collapsed = new HashSet<T>();
            var expanded  = new HashSet<T>();

            foreach (var item in items)
            {
                var data = item.data;

                if (controller.IsExpanded(item.id))
                {
                    expanded.Add(data);
                }
                else
                {
                    collapsed.Add(data);
                }
            }

            Profiler.EndSample();

            return new TreeNodeState(collapsed, expanded);
        }

        private void LoadExpandedNodes(TreeNodeState state)
        {
            var controller = viewController;

            var expand = state.Expanded.Count > state.Collapsed.Count;

            Debug.Log($"Expanded objects: {state.Expanded.Count}, " +
                      $"Collapsed objects: {state.Collapsed.Count}, " +
                      $"{nameof(expand)}: {(expand ? "<color=green>TRUE</color>" : "<color=red>FALSE</color>")}");

            Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: flatten items");
            var items = Flatten(RootItems!, s => s.children);
            Profiler.EndSample();

            Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: data -> id map");
            var dictionary = items.ToDictionary(s => s.data, s => s.id);
            Profiler.EndSample();

            if (expand)
            {
                Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: expand all");
                controller.ExpandAll();
                Profiler.EndSample();

                foreach (var data in state.Collapsed)
                {
                    var id = dictionary[data];

                    Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: collapse item");
                    controller.CollapseItem(id, false);
                    Profiler.EndSample();
                }
            }
            else
            {
                Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: collapse all");
                controller.CollapseAll();
                Profiler.EndSample();

                foreach (var data in state.Expanded)
                {
                    var id = dictionary[data];

                    Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: expand item");
                    controller.ExpandItem(id, false, false);
                    Profiler.EndSample();
                }
            }
        }

        private List<TreeViewItemData<T>> GetRootItems()
        {
            var list = new List<TreeViewItemData<T>>();

            if (Root is null)
            {
                return list;
            }

            var stack = new Stack<(T Node, TreeViewItemData<T>? Container)>();

            stack.Push((Root, null));

            var descriptions = sortedColumns as SortColumnDescription[] ?? sortedColumns.ToArray();

            var dictionary = descriptions.ToDictionary(s => s.columnName, s => Columns.Single(t => t.Name == s.columnName));

            var id = 0;

            while (stack.Count > 0)
            {
                var (node, container) = stack.Pop();

                var data = new TreeViewItemData<T>(id++, node);

                if (container is null)
                {
                    list.Add(data);
                }
                else
                {
                    ((IList<TreeViewItemData<T>>)container.Value.children).Add(data);
                }

                var children = node.Cast<T>().Reverse();

                foreach (var description in descriptions)
                {
                    var column = dictionary[description.columnName];
                    var getter = column.ValueGetter ?? throw new InvalidOperationException();
                    var order  = description.direction is SortDirection.Descending;

                    children = children.Sort(getter, null, order);
                }

                foreach (var child in children)
                {
                    stack.Push((child, data));
                }
            }

            return list;
        }

        private class TreeNodeState
        {
            public TreeNodeState(HashSet<T> collapsed, HashSet<T> expanded)
            {
                Collapsed = collapsed ?? throw new ArgumentNullException(nameof(collapsed));
                Expanded  = expanded ?? throw new ArgumentNullException(nameof(expanded));
            }

            public HashSet<T> Collapsed { get; }

            public HashSet<T> Expanded { get; }
        }
    }
}