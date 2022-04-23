using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
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

            Builder = new TreeViewBuilder<T>(this);
            Sorter  = new TreeViewSorter<T>(this);
        }

        internal TreeViewBuilder<T> Builder { get; }

        private TreeViewSorter<T> Sorter { get; }

        #region General stuff

        public T? Node { get; set; } // root node

        public void Dispose()
        {
            columnSortingChanged -= Sorter.OnSortChanged;

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
                var node = Builder.FirstOrDefault().data;

                if (node != null)
                {
                    SelectNode(node, true, true);
                }
            }

            this.Q<ScrollView>().contentContainer.Focus(); // do focus this correctly
        }

        public int GetRowCount()
        {
            return Builder.GetNodeCount();
        }

        #endregion

        #region Columns

        internal GenericTreeViewColumn<T>[]? Columns { get; private set; }

        public void SetColumns(GenericTreeViewColumn<T>[] collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (collection.Length is 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(collection));

            columnSortingChanged -= Sorter.OnSortChanged;

            foreach (var column in collection)
            {
                columns.Add(column.GetColumn());
            }

            columnSortingChanged += Sorter.OnSortChanged;

            Columns = collection;
        }

        #endregion

        #region Building

        public void SetRootNode(T? node)
        {
            Node = node;

            Rebuild();
        }

        /// <inheritdoc cref="BaseVerticalCollectionView.Rebuild" />
        public new void Rebuild() // let's pile up on their favorite 'new' keyword
        {
            Builder.Rebuild();

            base.Rebuild();
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
            return Builder.GetNodeIdentifier(node);
        }

        #endregion

        #region Search

        /// <summary>
        ///     Gets or sets the search filter for this instance.
        /// </summary>
        /// <remarks>
        ///     Call <see cref="Rebuild" /> after to apply changes.
        /// </remarks>
        public string? SearchFilter { get; set; }

        /// <summary>
        ///     Gets or sets the search filter comparer for this instance.
        /// </summary>
        /// <remarks>
        ///     Call <see cref="Rebuild" /> after to apply changes.
        /// </remarks>
        public IEqualityComparer<T>? SearchFilterComparer { get; set; }

        /// <summary>
        ///     Gets whether the search filter is a valid regular expression pattern.
        /// </summary>
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public bool SearchFilterValidate(string value)
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