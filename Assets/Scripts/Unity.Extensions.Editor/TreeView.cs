using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Extensions.Editor
{
    public sealed class TreeView<T> : TreeView where T : TreeNode
    {
        public TreeView(TreeViewState state, T? root = default) : base(state)
        {
            enableItemHovering            = true;
            showAlternatingRowBackgrounds = true;
            getNewSelectionOverride       = GetNewSelectionOverride;

            SetRoot(root);
        }

        private T? Root { get; set; }

        public bool HasRoot => Root != null;

        public TreeViewSearchFilterHandler<T>? SearchFilter { get; set; }

        private List<int> GetNewSelectionOverride(TreeViewItem clickedItem, bool keepMultiSelection, bool useActionKeyAsShift)
        {
            // multi-selection isn't implemented for this method, throw if it's the case

            const string message = "The new selection override doesn't support multi-selection.";

            try
            {
                Assert.IsFalse(CanMultiSelect(null!), message);
            }
            catch (Exception e) // because the above can fail... guys, how difficult would it have been to have a CanMultiSelect() overload?
            {
                throw new AssertionException(message, e.Message);
            }

            // we want a user-friendly context click behavior, i.e. one that doesn't sadistically changes actual selection

            var selection = new List<int>();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                selection.AddRange(GetSelection());
            }
            else
            {
                selection.Add(clickedItem.id);
            }

            return selection;
        }

        public void SetRoot(T? root)
        {
            Root = root;
            Reload();
        }

        public void SetRowHeight(float value)
        {
            rowHeight = value;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem<T>(-1, -1, null!) { children = new List<TreeViewItem>() };

            if (Root != null)
            {
                var index = 0;

                var stack = new Stack<TreeViewItem<T>>();

                stack.Push(new TreeViewItem<T>(index++, 0, Root.ToString(), Root) { parent = root });

                while (stack.Any())
                {
                    var current = stack.Pop();

                    current.parent.AddChild(current);

                    foreach (var child in current.Data!.Reverse())
                    {
                        stack.Push(new TreeViewItem<T>(index++, 0, child.ToString(), child as T) { parent = current });
                    }
                }
            }

            SetupDepthsFromParentsAndChildren(root);

            return root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = base.BuildRows(root);

            if (SearchFilter is null)
                return rows;

            var list = SearchFilter(searchString, rows.Cast<TreeViewItem<T>>());

            rows.Clear();

            foreach (var item in list)
            {
                rows.Add(item);
            }

            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var rect = new Rect(args.rowRect);

            rect.xMin += GetContentIndent(args.item);

            CenterRectUsingSingleLineHeight(ref rect);

            GUI.Label(rect, args.item.displayName);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            NodeSelectionChanged?.Invoke(this, new TreeViewSelectionEventArgs<T>(selectedIds.Select(GetNode).ToList()));
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
            return ((TreeViewItem<T>)FindItem(id, rootItem)).Data!;
        }

        public event EventHandler<TreeViewMouseClickEventArgs>? NodeMouseContextClick;

        public event EventHandler<TreeViewMouseClickEventArgs>? NodeMouseDoubleClick;

        public event EventHandler<TreeViewMouseClickEventArgs>? NodeMouseSingleClick;

        public event EventHandler<TreeViewSelectionEventArgs<T>>? NodeSelectionChanged;
    }
}