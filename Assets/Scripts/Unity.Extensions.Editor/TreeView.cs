using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Unity.Extensions.Editor
{
    // this is possibly the shittiest base class you'll ever have to deal with in your lifetime,
    // it is so poorly implemented that these virtual members are virtually useless...
    // you must re-implement everything simply because the morons who wrote it are clueless.

    // the general idea was simple, a control on par with what you'd find in WinForms or WPF,
    // i.e. decent sorting and filtering but most importantly, avoid the need to derive for no real reason
    // there were a lot of traps in getting this up and obviously no documentation for it, all Unity-style

    [PublicAPI]
    public class TreeView<T> : TreeView where T : TreeNode // non-abstract as we have a decent, reusable base
    {
        #region Fields

        private T? _root;

        #endregion

        #region Constructors

        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader)
            : base(state, multiColumnHeader)
        {
            multiColumnHeader.sortingChanged += SortChanged;

            PropertyChanged += OnPropertyChanged;

            // NOTE: stupid tree needed a Reload() or it would throw after ctor, but after writing a shit-ton of code it's not the case anymore
        }

        #endregion

        #region Properties (private)

        private Dictionary<T, int> NodeForward { get; } = new();

        private Dictionary<int, T> NodeReverse { get; } = new();

        private List<TreeViewItem> Rows { get; } = new(100); // watch and learn, never trust input from Unity monkeys

        #endregion

        #region Properties (public)

        public Func<TreeViewItem<T>, bool>? OnCanMultiSelect { private get; init; } // we need to expose this protected shit #1

        public GetNewSelectionFunction OnGetNewSelection // we need to expose this protected shit #2
        {
            init => getNewSelectionOverride = value; // set-only, how cool is that?
        }

        public T? Root
        {
            get => _root;
            set
            {
                if (EqualityComparer<T?>.Default.Equals(value, _root))
                    return;

                _root = value;
                OnPropertyChanged();
            }
        }

        public float RowHeight // we need to expose this protected shit #3
        {
            get => rowHeight;
            set => rowHeight = value;
        }

        #endregion

        #region Overrides

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem<T>(-1, -1);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            // fucking base class logic is useless, we must do it ourselves which means redo it in its entirety...

            Rows.Clear();

            if (Root is null)
            {
                return Rows;
            }

            root.children?.Clear(); // these idiots occasionally forget doing so, avoid a nasty leak

            if (string.IsNullOrWhiteSpace(searchString)) // build a hierarchical view only including expanded items
            {
                var stack = new Stack<TreeViewItem<T>>();

                stack.Push(new TreeViewItem<T>(NodeForward[Root], Root.Depth, Root.ToString(), Root) { parent = root });

                while (stack.Count > 0)
                {
                    var pop = stack.Pop();

                    pop.parent.AddChild(pop);

                    Rows.Add(pop);

                    var data = pop.Data ?? throw new InvalidDataException();

                    if (data.Count <= 0)
                        continue;

                    if (IsExpanded(pop.id))
                    {
                        foreach (var node in data.Cast<T>().Reverse())
                        {
                            stack.Push(new TreeViewItem<T>(NodeForward[node], node.Depth, node.ToString(), node) { parent = pop });
                        }
                    }
                    else
                    {
                        pop.children = CreateChildListForCollapsedParent(); // some damn fine Unity shit
                    }
                }
            }
            else // build a flattened, filtered view
            {
                var items = Root
                    .TraverseDfs()
                    .Cast<T>()
                    .Where(s => Filter(s, searchString))
                    .Select(s => new TreeViewItem<T>(NodeForward[s], 0, s.ToString(), s));

                Rows.AddRange(items); // notice how we don't update passed root here
            }

            Sort(Rows, rootItem, multiColumnHeader);

            return Rows; // got it? these monkeys like to call your method many times as they're clueless
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return OnCanMultiSelect switch
            {
                null => base.CanMultiSelect(item),
                _    => OnCanMultiSelect.Invoke(item as TreeViewItem<T> ?? throw new InvalidDataException())
            };
        }

        protected override IList<int> GetAncestors(int id)
            // this shit is needed by BuildRows
        {
            var ancestors = new List<int>();

            var node = NodeReverse[id];

            while (node.Parent != null)
            {
                var parent = (node.Parent as T)!;

                ancestors.Add(NodeForward[parent]);

                node = parent;
            }

            return ancestors;
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
            // this shit is needed by BuildRows
        {
            var descendants = new List<int>();

            var stack = new Stack<T>();

            stack.Push(NodeReverse[id]);

            while (stack.Any())
            {
                var pop = stack.Pop();

                if (pop.Count <= 0)
                    continue;

                descendants.Add(NodeForward[pop]);

                foreach (var child in pop)
                {
                    stack.Push((child as T)!);
                }
            }

            return descendants;
        }

        protected override void RowGUI(RowGUIArgs args)
            // TODO this could be further improved by providing custom GUI handlers
        {
            // render the actual columns, account for the fact that the user may have reordered them

            var item = args.item as TreeViewItem<T> ?? throw new InvalidDataException();
            var node = item.Data ?? throw new InvalidDataException();

            var columns = args.GetNumVisibleColumns();

            for (var i = 0; i < columns; i++)
            {
                var columnIndex = args.GetColumn(i);
                var column      = multiColumnHeader.state.columns[columnIndex] as TreeViewColumn<T> ?? throw new InvalidDataException();
                var rect        = args.GetCellRect(i);

                CenterRectUsingSingleLineHeight(ref rect);

                if (column.IsPrimaryColumn)
                {
                    columnIndexForTreeFoldouts = columnIndex;
                }

                if (columnIndex == columnIndexForTreeFoldouts)
                {
                    rect.xMin += GetContentIndent(item);
                }

                var result = column.Getter.Invoke(node);

                GUI.Label(rect, result.ToString(), TreeViewStyles.Label[column.TextAnchor]);
            }
        }

        #endregion

        #region Filtering and sorting

        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        private bool Filter(T node, string text)
            // TODO virtual
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

            // try to find an ordinal, case-insensitive match on every column's ToString

            foreach (var column in multiColumnHeader.state.columns.Cast<TreeViewColumn<T>>())
            {
                var value = column.Getter(node);

                if (value.ToString().Contains(text, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<TreeViewItem<T>> Sort(IEnumerable<TreeViewItem<T>> source)
            // TODO virtual
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            // here, unlike in their samples, we don't do the work twice and obviously don't refer to things using literal strings

            foreach (var index in multiColumnHeader.state.sortedColumns)
            {
                var column     = multiColumnHeader.state.columns[index] as TreeViewColumn<T> ?? throw new InvalidDataException();
                var descending = multiColumnHeader.IsSortedAscending(index) is false;
                source = source.Sort(s => column.Getter(s.Data ?? throw new InvalidDataException()), null, descending);
            }

            return source;
        }

        private void Sort(IList<TreeViewItem> rows, TreeViewItem root, MultiColumnHeader header)
        {
            if (rows is null)
                throw new ArgumentNullException(nameof(rows));

            if (root is null)
                throw new ArgumentNullException(nameof(root));

            if (header is null)
                throw new ArgumentNullException(nameof(header));

            if (rows.Count <= 1 || header.sortedColumnIndex is -1)
                return;

            // it couldn't be more fucked up than that, we must deal with rows on screen, thanks guys!

            var items = new List<TreeViewItem<T>>(rows.Count);

            if (string.IsNullOrWhiteSpace(searchString))
            {
                // basically, nasty, we want deep sorting a list that represents a hierarchy of nodes

                var stack = new Stack<TreeViewItem<T>>(rows.Count);

                foreach (var item in rows.Cast<TreeViewItem<T>>().Reverse())
                {
                    if (item.parent == root)
                    {
                        stack.Push(item);
                    }
                }

                items.AddRange(stack);

                while (stack.Count > 0)
                {
                    var pop = stack.Pop();

                    if (!Sort(pop))
                        continue;

                    var sort = Sort(pop.children.Cast<TreeViewItem<T>>()).ToList();
                    var slot = items.IndexOf(pop);

                    items.InsertRange(slot + 1, sort);

                    foreach (var item in sort.Where(Sort))
                    {
                        stack.Push(item);
                    }
                }
            }
            else
            {
                items.AddRange(Sort(rows.Cast<TreeViewItem<T>>())); // easy case, input is not a hierarchy
            }

            // I will let you have a guess on why in this schizophrenic API we shall not update the root...

            rows.Clear();

            rows.AddRange(items);
        }

        private static bool Sort(TreeViewItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            return item.children is not null && IsChildListForACollapsedParent(item.children) is false;
        }

        private void SortChanged(MultiColumnHeader header)
        {
            Sort(GetRows(), rootItem, header); // NOTE: this is way more subtle than you think it is
        }

        #endregion

        #region Events

        public event EventHandler<TreeViewMouseClickEventArgs>? NodeMouseContextClick;

        public event EventHandler<TreeViewMouseClickEventArgs>? NodeMouseDoubleClick;

        public event EventHandler<TreeViewMouseClickEventArgs>? NodeMouseSingleClick;

        public event EventHandler<TreeViewSelectionEventArgs<T>>? NodeSelectionChanged;

        protected override void SelectionChanged(IList<int> ids)
        {
            NodeSelectionChanged?.Invoke(this, new TreeViewSelectionEventArgs<T>(ids.Select(GetNode).ToList()));
        }

        protected override void SingleClickedItem(int id)
        {
            NodeMouseSingleClick?.Invoke(this, new TreeViewMouseClickEventArgs(GetNode(id)));
        }

        protected override void DoubleClickedItem(int id)
        {
            NodeMouseDoubleClick?.Invoke(this, new TreeViewMouseClickEventArgs(GetNode(id)));
        }

        protected override void ContextClickedItem(int id)
        {
            NodeMouseContextClick?.Invoke(this, new TreeViewMouseClickEventArgs(GetNode(id)));
        }

        private T GetNode(int id)
        {
            // another great method from Unity, it doesn't always work and obviously lacks documentation

            var item = FindItem(id, rootItem) ?? GetRows().Single(s => s.id == id);
            var type = item as TreeViewItem<T> ?? throw new InvalidDataException();
            var node = type.Data ?? throw new InvalidDataException();

            return node;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not nameof(Root))
                return;

            // unlike these sadistic coders, we don't ask consumers to provide ids, it's an implementation detail...

            NodeForward.Clear();
            NodeReverse.Clear();

            if (Root is not null)
            {
                foreach (var node in Root.TraverseDfs().Cast<T>())
                {
                    NodeForward.Add(node, NodeForward.Count);
                    NodeReverse.Add(NodeReverse.Count, node);
                }
            }

            Reload();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}