using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Extensions;
using UnityEngine.UIElements;

namespace Twisted.Controls
{
    public class TreeView<T> : MultiColumnTreeView, IDisposable where T : TreeNode
    // not only the code monkeys at Unity managed to do it in 3 FUCKING YEARS,
    // it's also unbelievably buggy but from them this isn't a surprise at all
    // worst being the column sorting stuff, that was literally freezing Unity
    // to deep sort 10K+ nodes: theirs = ~30 second, mine = less than a second
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
        public T? RootNode { get; set; }

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

        #endregion

        #region Public methods

        /// <inheritdoc cref="VisualElement.Focus" />
        public new void Focus() // how the fuck they even managed to fail on that?
        {
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
        public new void Rebuild() // let's go pile up on their favorite 'new' keyword
        {
            Builder.Rebuild();

            base.Rebuild();
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
        public void SetSelection(IEnumerable<T> nodes)
        {
            var indices = nodes.Select(node => Builder.GetNodeIdentifier(node)).ToArray();

            SetSelection(indices);
        }

        /// <summary>
        ///     Selects a node for this instance.
        /// </summary>
        public void SelectNode(T node, bool notify, bool scroll)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var id = Builder.GetNodeIdentifier(node);

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

            var element = target.Q(className: TreeViewColumn<T>.ControlUssClassName);

            if (element is null)
            {
                throw new InvalidOperationException("Cell control could not be found.");
            }

            // invoke user callback to populate context menu, it will be displayed immediately after that

            var node = element.userData as T ?? throw new InvalidOperationException("Cell has no user data.");

            ContextMenuHandler.Invoke(node, evt.menu);
        }

        private void OnSelectionChanged(IEnumerable<object> objects)
        {
            SelectionChanged?.Invoke(this, new TreeViewSelectionChangedEventArgs<T>(objects.Cast<T>().ToArray()));
        }

        #endregion
    }
}