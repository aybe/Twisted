using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity.Extensions;
using UnityEngine.UIElements;

namespace Editor
{
    // some general notes about the masterpiece of shit that this tree view is

    // not only the code monkeys at Unity managed to do it in 3 FUCKING YEARS,
    // it's also unbelievably buggy but from them this isn't a surprise at all

    // worst being the column sorting stuff, that was literally freezing Unity

    // to deep sort 10K+ nodes: theirs = ~30 second, mine = less than a second

    public class GenericTreeView<T> : MultiColumnTreeView, IDisposable where T : TreeNode
    {
        protected GenericTreeView()
        {
            // hook to this so that we can provide GetSelection<T> and SetSelection<T>

            onSelectionChange += SelectionChangedCallback;

            // find the content container so that we can register context click events
            // note that we can't use 'this' as it would screw column headers contexts

            ContextMenuManipulator = new ContextualMenuManipulator(ContextMenuBuilder);

            ContextMenuManipulatorContainer = this.Q(className: ScrollView.contentUssClassName)
                                              ?? throw new InvalidOperationException("Content container could not be found.");

            ContextMenuManipulatorContainer.AddManipulator(ContextMenuManipulator);
        }

        #region General stuff

        private List<TreeViewItemData<T>>? Items { get; set; } // their junk struct

        private T? Node { get; set; } // root node

        private Dictionary<T, int>? Nodes { get; set; } // for fast ID retrieval

        public void Dispose()
        {
            columnSortingChanged -= SortChanged;

            onSelectionChange -= SelectionChangedCallback;

            ContextMenuManipulatorContainer.RemoveManipulator(ContextMenuManipulator);
        }

        public new void Focus() // because their stupid method can't even focus right
        {
            // depending selection and search history the selection might become null
            // thus, when control gets focus, navigation will not work until 2nd time
            // ensure that something is selected so that navigation works on 1st time

            if (selectedItem is null)
            {
                var node = Items?.FirstOrDefault().data;

                if (node != null)
                {
                    SelectNode(node, true, true);
                }
            }

            this.Q<ScrollView>().contentContainer.Focus(); // do focus this correctly
        }

        public int GetRowCount()
        {
            ThrowIfNodesIsNull();

            return Nodes!.Count;
        }

        private void ThrowIfNodesIsNull()
        {
            if (Nodes is null)
            {
                throw new InvalidOperationException($"This instance hasn't been assigned a root node using {nameof(SetRootNode)}.");
            }
        }

        #endregion

        #region Columns

        private GenericTreeViewColumn<T>[]? Columns { get; set; }

        public void SetColumns(GenericTreeViewColumn<T>[] collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (collection.Length is 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(collection));

            columnSortingChanged -= SortChanged;

            foreach (var column in collection)
            {
                columns.Add(column.GetColumn());
            }

            columnSortingChanged += SortChanged;

            Columns = collection;
        }

        #endregion

        #region Building

        private static List<TSource> GetList<TSource>(IEnumerable<TSource> collection, Func<TSource, IEnumerable<TSource>> children)
            // flatten a hierarchy
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (children is null)
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

        private List<TreeViewItemData<T>> GetRootItems()
        {
            if (Columns is null || Columns.Length is 0)
            {
                throw new InvalidOperationException($"This instance hasn't been assigned columns using {nameof(SetColumns)}.");
            }

            if (Node is null)
            {
                return new List<TreeViewItemData<T>>(); // until root gets assigned, keep user and their private shit happy
            }

            // here we use a dictionary because we'll query this (rows * columns) times and that will sum up really quickly

            var descriptions = sortedColumns as SortColumnDescription[] ?? sortedColumns.ToArray();
            var dictionary   = descriptions.ToDictionary(s => s.columnName, s => Columns.Single(t => t.Name == s.columnName));

            var items = string.IsNullOrWhiteSpace(SearchFilter)
                ? GetRootItemsTree(descriptions, dictionary)
                : GetRootItemsList(descriptions, dictionary);

            return items;
        }

        private List<TreeViewItemData<T>> GetRootItemsList(
            SortColumnDescription[] descriptions, IReadOnlyDictionary<string, GenericTreeViewColumn<T>> dictionary)
        {
            if (descriptions is null)
                throw new ArgumentNullException(nameof(descriptions));

            if (dictionary is null)
                throw new ArgumentNullException(nameof(dictionary));

            var nodes = new List<T>();
            var stack = new Stack<T>(new[] { Node! });
            var regex = new Regex(SearchFilter!, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                foreach (var column in Columns!)
                {
                    var o = column.ValueGetter?.Invoke(pop);
                    var s = column.ValueFormatter?.Invoke(o);
                    var b = s != null && regex.IsMatch(s);

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

            var sort = nodes.AsEnumerable();

            foreach (var description in descriptions)
            {
                var column = dictionary[description.columnName];
                var getter = column.ValueGetter;

                if (getter is null)
                {
                    throw new NullReferenceException($"{nameof(GenericTreeViewColumn<T>.ValueGetter)} is null.");
                }

                sort = sort.Sort(getter, null, description.direction is SortDirection.Descending);
            }

            if (SearchFilterEqualityComparer is not null)
            {
                sort = sort.Distinct(SearchFilterEqualityComparer);
            }

            return sort.Select((s, t) => new TreeViewItemData<T>(t, s)).ToList();
        }

        private List<TreeViewItemData<T>> GetRootItemsTree(
            SortColumnDescription[] descriptions, IReadOnlyDictionary<string, GenericTreeViewColumn<T>> dictionary)
        {
            if (descriptions is null)
                throw new ArgumentNullException(nameof(descriptions));

            if (dictionary is null)
                throw new ArgumentNullException(nameof(dictionary));

            var list = new List<TreeViewItemData<T>>();

            var stack = new Stack<(T Node, TreeViewItemData<T>? Container)>();

            stack.Push((Node!, null));

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
                    var getter = column.ValueGetter;

                    if (getter is null)
                    {
                        throw new NullReferenceException($"{nameof(GenericTreeViewColumn<T>.ValueGetter)} is null.");
                    }

                    children = children.Sort(getter, null, description.direction is SortDirection.Descending);
                }

                foreach (var child in children)
                {
                    stack.Push((child, item));
                }
            }

            return list;
        }

        public void SetRootNode(T? node)
        {
            Node = node;

            Rebuild();
        }

        private new void Rebuild() // let's pile up on their favorite 'new' keyword... and sweep that shit under the rug
        {
            // here we basically build their items, our nodes map and run their initialization sequence

            Items = GetRootItems();

            Nodes = GetList(Items, s => s.children).ToDictionary(s => s.data, s => s.id);

            SetRootItems(Items);

            base.Rebuild();
        }

        #endregion

        #region Sorting

        private Task? SortBackgroundTask { get; set; }

        private void SortChanged()
        {
            // these code monkeys will raise this event N headers + 2 times in a row for NO FUCKING REASON
            // this, unless you hold a FUCKING modifier such as Shift or Ctrl, obviously, not explained...
            // now let's protect ourselves from their stupid shit by using a task with a rudimentary guard

            // this, along our manual handling brings back SMOOTH performance like there was in IMGUI tree

            if (SortBackgroundTask?.IsCompleted == false)
                return;

            SortBackgroundTask = Task.Factory.StartNew(
                Sort,
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext() // this is WPF-like, cross-thread stuff
            );
        }

        private void Sort()
        {
            // the deep sorting will screw ids/expanded/selection, first, save this information

            var selection = GetSelection();

            SortSaveExpanded(out var collapsed, out var expanded);

            // perform the actual sorting, this will also update our dictionary that we'll need

            Rebuild();

            // restore the collapsed/expanded state of the tree view items but in a FAST manner

            SortLoadExpanded(collapsed, expanded);

            if (selection.Any())
            {
                var dictionary = Nodes!;

                // now it's time to restore the selection that was previously made by the user

                SetSelectionById(selection.Select(s => dictionary[s]));

                // scroll to something or it'll suck, not perfect because of their incompetence

                ScrollToItemById(dictionary[selection.First()]);
            }

            // redraw control and use our own focus because these morons even failed on this...

            RefreshItems();

            Focus();
        }

        private void SortSaveExpanded(out HashSet<T> collapsed, out HashSet<T> expanded)
        {
            // using their stuff at the bare minimum as it's complete junk that is unbelievably slow

            collapsed = new HashSet<T>();
            expanded  = new HashSet<T>();

            var controller = viewController;

            var items = GetList(Items!, s => s.children);

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
        }

        private void SortLoadExpanded(HashSet<T> collapsed, HashSet<T> expanded)
            // here we use a simple but effective approach that scale well for 10K+ tree nodes
        {
            // build a dictionary to be able to retrieve newly assigned IDs to tree view items

            var ids = GetList(Items!, s => s.children).ToDictionary(s => s.data, s => s.id);

            // now really stupid: we'd do it in whichever way that will take the shortest time
            // this, because their crap is exponentially longer as there are nodes in the tree

            var controller = viewController;

            if (expanded.Count > collapsed.Count)
            {
                controller.ExpandAll();

                foreach (var data in collapsed)
                {
                    controller.CollapseItem(ids[data], false);
                }
            }
            else
            {
                controller.CollapseAll();

                foreach (var data in expanded)
                {
                    controller.ExpandItem(ids[data], false, false);
                }
            }
        }

        #endregion

        #region Selection

        public event EventHandler<TreeViewSelectionChangedEventArgs<T>>? SelectionChanged;

        private void SelectionChangedCallback(IEnumerable<object> objects)
        {
            SelectionChanged?.Invoke(this, new TreeViewSelectionChangedEventArgs<T>(objects.Cast<T>().ToArray()));
        }

        public IReadOnlyList<T> GetSelection()
        {
            var nodes = selectedItems.Cast<T>().ToArray();

            return nodes;
        }

        public void SetSelection(IEnumerable<T> nodes)
        {
            var indices = nodes.Select(GetNodeId).ToArray();

            SetSelection(indices);
        }

        public void SelectNode(T node, bool notify, bool scroll)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var id = GetNodeId(node);

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

        private int GetNodeId(T node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            ThrowIfNodesIsNull();

            if (!Nodes!.TryGetValue(node, out var id))
            {
                throw new ArgumentOutOfRangeException(nameof(node), node, "The node is neither the root node nor a child of it.");
            }

            return id;
        }

        #endregion

        #region Search

        private string? SearchFilter { get; set; }

        private IEqualityComparer<T>? SearchFilterEqualityComparer { get; set; }

        public string? GetSearchFilter()
        {
            return SearchFilter;
        }

        public void SetSearchFilter(string? value)
        {
            SearchFilter = value;

            Rebuild();
        }

        public IEqualityComparer<T>? GetSearchFilterEqualityComparer()
        {
            return SearchFilterEqualityComparer;
        }

        public void SetSearchFilterEqualityComparer(IEqualityComparer<T>? equalityComparer)
        {
            SearchFilterEqualityComparer = equalityComparer;

            Rebuild();
        }

        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public bool IsValidSearchFilter(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            try
            {
                new Regex(value, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Context menu

        private ContextualMenuManipulator ContextMenuManipulator { get; }

        private VisualElement? ContextMenuManipulatorContainer { get; }

        public TreeViewContextMenuHandler<T>? ContextMenuHandler { get; set; }

        private void ContextMenuBuilder(ContextualMenuPopulateEvent evt)
        {
            // I've decided to use this approach which, I believe, is the least intrusive way to implement this
            // and it follows what appears to be the newest crap in their junk API: ContextualMenuPopulateEvent
            // this has many benefits, it works on the entire row and doesn't steal focus like in IMGUI version
            // now the downside is that we have to rely on user data because TreeNode is by nature not bindable

            if (ContextMenuHandler is null)
            {
                return; // user may not choose to use context menu, in this case there's no point in doing work
            }

            // get the row container that holds the controls representing a cell

            var target = evt.triggerEvent.target as VisualElement ?? throw new InvalidOperationException();

            while (target != null)
            {
                if (target.ClassListContains(MultiColumnController.rowContainerUssClassName))
                    break;

                target = target.parent;
            }

            if (target is null)
            {
                throw new InvalidOperationException("Row container could not be found.");
            }

            // get user data from the first control found that represents a cell

            var element = target.Q(className: GenericTreeViewColumn<T>.ControlUssClassName);

            if (element is null)
            {
                throw new InvalidOperationException("Cell control could not be found.");
            }

            // invoke user callback to populate context menu, it will be displayed immediately after that

            var node = element.userData as T ?? throw new InvalidOperationException("Cell has no user data.");

            ContextMenuHandler.Invoke(node, evt.menu);
        }

        #endregion
    }
}