using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Extensions;
using UnityEngine.UIElements;

namespace Editor
{
    public class GenericTreeView<T> : MultiColumnTreeView, IDisposable where T : TreeNode
    {
        private readonly GenericTreeViewColumn<T>[] Columns;

        private T? Root;

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

            var items = GetRootItems();

            SetRootItems(items);

            Rebuild();
        }

        private void OnColumnSortingChanged()
            // BUG: Unity code monkeys raise this N headers + 2 times in a row... unless you hold a fucking modifier!
        {
            // deep sorting screws ids and thus expanded nodes, save this info to restore it later on new hierarchy

            SaveExpandedNodes(out var map1, out var map2);

            SetRootItems(GetRootItems());

            LoadExpandedNodes(map1, map2);

            RefreshItems();
        }

        private void SaveExpandedNodes(out IReadOnlyDictionary<object, int> map1, out IReadOnlyDictionary<int, bool> map2)
        {
            var dictionary1 = new Dictionary<object, int>();
            var dictionary2 = new Dictionary<int, bool>();

            var controller = viewController;

            var ids = controller.GetAllItemIds();

            foreach (var id in ids)
            {
                var index = controller.GetIndexForId(id);

                if (index is -1)
                    continue;

                var key1 = controller.GetItemForIndex(index);
                var key2 = controller.IsExpanded(id);
                dictionary1.Add(key1, id);
                dictionary2.Add(id, key2);
            }

            map1 = new ReadOnlyDictionary<object, int>(dictionary1);
            map2 = new ReadOnlyDictionary<int, bool>(dictionary2);
        }

        private void LoadExpandedNodes(IReadOnlyDictionary<object, int> map1, IReadOnlyDictionary<int, bool> map2)
        {
            if (map1 == null)
                throw new ArgumentNullException(nameof(map1));

            if (map2 == null)
                throw new ArgumentNullException(nameof(map2));

            var controller = viewController;

            controller.CollapseAll();

            var ids = controller.GetAllItemIds();

            foreach (var id in ids)
            {
                var index = controller.GetIndexForId(id);

                if (index is -1)
                    continue;

                var o = controller.GetItemForIndex(index);

                if (!map1.TryGetValue(o, out var i))
                    continue;

                if (!map2.TryGetValue(i, out var b))
                    continue;

                if (b)
                {
                    controller.ExpandItem(id, false, false);
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
    }
}