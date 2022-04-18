using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Extensions;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

namespace Editor
{
    // some general notes about the masterpiece of shit that this tree view is:

    // not only the code monkeys at Unity managed to do it in 3 FUCKING YEARS,
    // it's also unbelievably buggy but from them this isn't a surprise at all...

    // it looks like it's been inspired from IMGUI since the same silly design
    // forces one to roll-out surprisingly similar fixes for it to work right
    // at least the IMGUI version was fast even though it was a pain to use...

    // the most astonishing is the column sorting and all of their stuff around
    // that is needed for implementing a decent multi-column deep sort; in short,
    // you can't trust much of their code either because it's buggy, slow or both

    // e.g. deep sort 10K+ nodes: theirs = ~30 seconds, mine = less than a second

    public class GenericTreeView<T> : MultiColumnTreeView, IDisposable where T : TreeNode
    {
        private readonly GenericTreeViewColumn<T>[] Columns;

        private Dictionary<T, int>? NodesDictionary;

        private T? Root;

        private List<TreeViewItemData<T>>? RootItems;

        private Task? SortingTask;

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

            SetRoot(null); // good old habits prevail at Unity, this initializes their internal shit
        }

        private string? SearchFilter { get; set; }

        public void Dispose()
        {
            columnSortingChanged -= OnColumnSortingChanged;
        }

        public void SetSearchFilter(string? searchFilter)
        {
            SearchFilter = searchFilter;
            Reload();
        }

        public int GetNodeId(T node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return NodesDictionary is null ? -1 : NodesDictionary[node];
        }

        public void SetRoot(T? node)
        {
            Root = node;

            Reload();
        }

        private void Reload()
        {
            RootItems = GetRootItems();

            NodesDictionary = Flatten(RootItems, s => s.children).ToDictionary(s => s.data, s => s.id);

            SetRootItems(RootItems);

            Rebuild();
        }

        private void OnColumnSortingChanged()
        {
            // these code monkey will raise N headers + 2 times in a row... for no fucking reason
            // that is unless you hold a fucking modifier such as Shift or Ctrl, not explained...

            // protect ourselves from their stupid shit by using a task with a rudimentary guard

            if (SortingTask?.IsCompleted == false)
                return;

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext(); // just like in WPF

            SortingTask = Task.Factory.StartNew(SortTreeItems, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }

        private void SortTreeItems()
        {
            // deep sorting will screw ids and therefore expanded nodes, save this information

            Profiler.BeginSample($"{nameof(SortTreeItems)}: {nameof(SaveExpandedNodes)}");
            var state = SaveExpandedNodes();
            Profiler.EndSample();

            // perform the actual sorting

            Profiler.BeginSample($"{nameof(SortTreeItems)}: {nameof(GetRootItems)}");
            RootItems = GetRootItems();
            Profiler.EndSample();

            Profiler.BeginSample($"{nameof(SortTreeItems)}: {nameof(SetRootItems)}");
            SetRootItems(RootItems);
            Profiler.EndSample();

            // restore the collapsed/expanded state of tree view items but in a FAST manner

            Profiler.BeginSample($"{nameof(SortTreeItems)}: {nameof(LoadExpandedNodes)}");
            LoadExpandedNodes(state);
            Profiler.EndSample();

            // finally, get this stupid control to redraw itself

            Profiler.BeginSample($"{nameof(SortTreeItems)}: {nameof(RefreshItems)}");
            RefreshItems();
            Profiler.EndSample();
        }

        private static List<TSource> Flatten<TSource>(IEnumerable<TSource> collection, Func<TSource, IEnumerable<TSource>> children)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (children == null)
                throw new ArgumentNullException(nameof(children));

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
            // if we were to use some of their dumb methods, this would take ages because they're poorly written

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
            // their shitty methods take exponentially longer as you have more nodes in the tree
            // as they're pure garbage, they simply freeze the UI for an astonishingly long time
            // here we use a very simple approach that works and scales well for 10K+ tree nodes

            // build a dictionary to be able to retrieve newly assigned IDs to tree view items

            Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: flatten items");
            var items = Flatten(RootItems!, s => s.children);
            Profiler.EndSample();

            Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: data -> id map");
            var dictionary = items.ToDictionary(s => s.data, s => s.id);
            Profiler.EndSample();

            // now downright fucking stupid: do it in whichever way that will take the shortest time

            var controller = viewController;

            var expand = state.Expanded.Count > state.Collapsed.Count;

            if (expand)
            {
                Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: {nameof(controller.ExpandAll)}");
                controller.ExpandAll();
                Profiler.EndSample();

                foreach (var data in state.Collapsed)
                {
                    var id = dictionary[data];

                    Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: {nameof(controller.CollapseItem)}");
                    controller.CollapseItem(id, false);
                    Profiler.EndSample();
                }
            }
            else
            {
                Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: {nameof(controller.CollapseAll)}");
                controller.CollapseAll();
                Profiler.EndSample();

                foreach (var data in state.Expanded)
                {
                    var id = dictionary[data];

                    Profiler.BeginSample($"{nameof(LoadExpandedNodes)}: {nameof(controller.ExpandItem)}");
                    controller.ExpandItem(id, false, false);
                    Profiler.EndSample();
                }
            }

            // the main cause of all that is that these newbies don't know how to use LINQ reasonably
        }

        private List<TreeViewItemData<T>> GetRootItems()
        {
            // another damn fine struct from Unity with, among other things, everything useful being internal

            if (string.IsNullOrWhiteSpace(SearchFilter))
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

                    var item = new TreeViewItemData<T>(id++, node);

                    if (container is null)
                    {
                        list.Add(item);
                    }
                    else
                    {
                        ((IList<TreeViewItemData<T>>)container.Value.children).Add(item);
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
                        stack.Push((child, item));
                    }
                }
                return list;
            }
            else
            {
                var list = new List<TreeViewItemData<T>>();

                if (Root is null)
                {
                    return list;
                }

                var stack = new Stack<T>();

                stack.Push(Root);

                var id = 0;

                while (stack.Count > 0)
                {
                    var pop = stack.Pop();

                    foreach (var column in Columns)
                    {
                        var o = column.ValueGetter?.Invoke(pop);
                        var s = column.ValueFormatter?.Invoke(o);
                        var b = s?.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase);

                        if (b is false)
                            continue;

                        nodes.Add(pop);
                        break;
                    }

                    foreach (var child in pop.Cast<T>().Reverse())
                    {
                        stack.Push(child);
                    }
                }

                return list; // TODO sort items
            }
        }

        public void SelectNode(T node, bool notify, bool scroll)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var id = GetNodeId(node);

            if (id is -1)
            {
                throw new InvalidOperationException();
            }

            if (notify)
            {
                SetSelectionById(id);
            }
            else
            {
                SetSelectionByIdWithoutNotify(new[] { id });
            }

            if (scroll)
            {
                ScrollToItemById(id);
            }
        }

        private sealed class TreeNodeState
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