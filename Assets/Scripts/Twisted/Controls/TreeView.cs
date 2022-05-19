using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Twisted.Controls
{
    public class TreeView<T> : MultiColumnTreeView, IDisposable where T : TreeNode
    // not only these morons managed to do it 3 FUCKING years, it's also incredibly buggy
    // the worst being the column sorting that would literally freeze the whole interface
    // sorting using their shit is like 30 seconds, using mine is literally instantaneous
    {
        protected TreeView()
        {
            // hook to this so that we can provide GetSelection<T> and SetSelection<T>

            onSelectionChange += OnSelectionChanged;

            // find the content container so that we can register context click events
            // note that we can't use 'this' as it would screw column headers contexts

            ContextMenu = new ContextualMenuManipulator(OnContextMenuPopulate);

            ContextMenuHost = this.Q(className: ScrollView.contentUssClassName)
                              ?? throw new InvalidOperationException("Content container could not be found.");

            ContextMenuHost.AddManipulator(ContextMenu);

            // register another callback so that we can do an Explorer-like navigation

            ContextMenuHost.RegisterCallback<KeyDownEvent>(OnScrollViewContentContainerKeyDown, TrickleDown.TrickleDown);

            Builder = new TreeViewBuilder<T>(this);
            Sorter  = new TreeViewSorter<T>(this);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            columnSortingChanged -= Sorter.OnSortChanged;

            onSelectionChange -= OnSelectionChanged;

            ContextMenuHost.RemoveManipulator(ContextMenu);
        }

        #endregion

        #region Public events

        /// <summary>
        ///     Occurs when the selection has changed.
        /// </summary>
        public event EventHandler<TreeViewSelectionChangedEventArgs<T>>? SelectionChanged;

        #endregion

        #region Private properties

        internal TreeViewBuilder<T> Builder { get; }

        private TreeViewSorter<T> Sorter { get; }

        internal TreeViewColumn<T>[]? Columns { get; private set; }

        private ContextualMenuManipulator ContextMenu { get; }

        private VisualElement? ContextMenuHost { get; }

        #endregion

        #region Public properties

        /// <summary>
        ///     Gets or sets the context menu handler for this instance.
        /// </summary>
        public TreeViewContextMenuHandler<T>? ContextMenuHandler { get; set; }

        /// <summary>
        ///     Gets or sets the root node for this instance.
        /// </summary>
        /// <remarks>
        ///     Call <see cref="Rebuild" /> to apply changes.
        /// </remarks>
        public T? RootNode { get; set; }

        /// <summary>
        ///     Gets or sets the search filter for this instance.
        /// </summary>
        /// <remarks>
        ///     Call <see cref="Rebuild" /> to apply changes.
        /// </remarks>
        public string? SearchFilter { get; set; }

        /// <summary>
        ///     Gets or sets the search filter comparer for this instance.
        /// </summary>
        /// <remarks>
        ///     Call <see cref="Rebuild" /> to apply changes.
        /// </remarks>
        public IEqualityComparer<T>? SearchFilterComparer { get; set; }

        #endregion

        #region Public methods

        /// <inheritdoc cref="VisualElement.Focus" />
        public new void Focus()
        {
            // amazingly, they couldn't even get this thing right...

            this.Q<ScrollView>().contentContainer.Focus();
        }

        /// <summary>
        ///     Gets the number of visible rows for this instance.
        /// </summary>
        /// <returns></returns>
        public int GetRowCount()
        {
            return Builder.GetNodeCount();
        }

        /// <inheritdoc cref="BaseVerticalCollectionView.Rebuild" />
        public new void Rebuild()
        {
            // backup the actual selection so that we can restore it after rebuild

            var selection = GetSelection();

            // rebuild the tree, this will filter and sort according current rules

            Builder.Rebuild();

            base.Rebuild();

            // restore the saved selection, it will reveal an item and looks clean

            var ids = selection.Select(s => Builder.GetNodeIdentifier(s)).ToList();

            ids.RemoveAll(s => s is -1); // because some items may now be filtered

            SetSelectionByIdWithoutNotify(ids);

            // this works properly when transitioning from/to filter/distinct mode
        }

        /// <summary>
        ///     Gets or sets the columns for this instance.
        /// </summary>
        public void SetColumns(TreeViewColumn<T>[] collection)
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

        /// <summary>
        ///     Gets the selected nodes for this instance.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<T> GetSelection()
        {
            var nodes = selectedItems.Cast<T>().ToArray();

            return nodes;
        }

        /// <summary>
        ///     Sets the selected nodes for this instance.
        /// </summary>
        public void SetSelection(IEnumerable<T> nodes, bool notify)
        {
            var indices = new List<int>();

            foreach (var node in nodes)
            {
                var id = Builder.GetNodeIdentifier(node);

                if (id is -1)
                {
                    throw new InvalidOperationException($"Couldn't get identifier for '{node}'.");
                }

                indices.Add(id);
            }

            if (notify)
            {
                SetSelectionById(indices);
            }
            else
            {
                SetSelectionByIdWithoutNotify(indices);
            }
        }

        /// <summary>
        ///     Selects a node for this instance.
        /// </summary>
        public void SelectNode(T node, bool notify, bool scroll)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var id = Builder.GetNodeIdentifier(node);

            if (id is -1)
            {
                throw new InvalidOperationException($"Couldn't get identifier for '{node}'.");
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

        #region Private methods

        private void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
        {
            // this is the least intrusive approach that doesn't steal focus like in IMGUI tree view

            // but we do have to rely on user data for this to work because TreeNode is not bindable

            if (ContextMenuHandler is null)
                return;

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

            var element = target.Q(className: TreeViewColumn<T>.ControlUssClassName);

            if (element is null)
            {
                throw new InvalidOperationException("Cell control could not be found.");
            }

            // invoke callback to populate menu, get displayed immediately after

            var node = element.userData as T ?? throw new InvalidOperationException("Cell has no user data.");

            ContextMenuHandler.Invoke(node, evt.menu);
        }

        private void OnSelectionChanged(IEnumerable<object> objects)
        {
            SelectionChanged?.Invoke(this, new TreeViewSelectionChangedEventArgs<T>(objects.Cast<T>().ToArray()));
        }

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
        private void OnScrollViewContentContainerKeyDown(KeyDownEvent evt)
        {
            // Explorer-like keyboard navigation

            switch (evt.keyCode)
            {
                case KeyCode.LeftArrow:
                    if (!viewController.IsExpandedByIndex(selectedIndex))
                    {
                        var id = GetParentIdForIndex(selectedIndex);

                        if (id is not -1)
                        {
                            var index = viewController.GetIndexForId(id);
                            SetSelection(index);
                            ScrollToItem(index);
                            evt.StopImmediatePropagation();
                        }
                    }
                    break;
                case KeyCode.RightArrow:
                    if (viewController.IsExpandedByIndex(selectedIndex))
                    {
                        var ids = GetChildrenIdsForIndex(selectedIndex).ToArray();

                        if (ids.Any())
                        {
                            var id    = ids.First();
                            var index = viewController.GetIndexForId(id);
                            SetSelection(index);
                            ScrollToItem(index);
                            evt.StopImmediatePropagation();
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}