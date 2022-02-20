﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace Twisted.Editor
{
    public sealed class TreeNodeView : TreeView
    {
        public TreeNodeView(TreeViewState state, TreeNode? root = default) : base(state)
        {
            enableItemHovering            = true;
            showAlternatingRowBackgrounds = true;
            getNewSelectionOverride       = GetNewSelectionOverride;

            SetRoot(root);
        }

        private TreeNode? Root { get; set; }

        public bool HasRoot => Root != null;

        public TreeNodeViewSearchFilterHandler? SearchFilter { get; set; }

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

        public void SetRoot(TreeNode? root)
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
            var root = new TreeViewItem<TreeNode>(-1, -1, null!) { children = new List<TreeViewItem>() };

            if (Root != null)
            {
                var index = 0;

                var stack = new Stack<TreeViewItem<TreeNode>>();

                stack.Push(new TreeViewItem<TreeNode>(index++, 0, Root.ToString(), Root) { parent = root });

                while (stack.Any())
                {
                    var current = stack.Pop();

                    current.parent.AddChild(current);

                    foreach (var child in current.Data!.Reverse())
                    {
                        stack.Push(new TreeViewItem<TreeNode>(index++, 0, child.ToString(), child) { parent = current });
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

            var list = SearchFilter(searchString, rows);

            rows.Clear();

            rows.AddRange(list);

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
            NodeSelectionChanged?.Invoke(this, new TreeNodeSelectionEventArgs(selectedIds.Select(GetNode).ToList()));
        }

        protected override void SingleClickedItem(int id)
        {
            NodeMouseSingleClick?.Invoke(this, new TreeNodeClickEventArgs(GetNode(id)));
        }

        protected override void DoubleClickedItem(int id)
        {
            NodeMouseDoubleClick?.Invoke(this, new TreeNodeClickEventArgs(GetNode(id)));
        }

        protected override void ContextClickedItem(int id)
        {
            NodeMouseContextClick?.Invoke(this, new TreeNodeClickEventArgs(GetNode(id)));
        }

        private TreeNode GetNode(int id)
        {
            return ((TreeViewItem<TreeNode>)FindItem(id, rootItem)).Data!;
        }

        public event EventHandler<TreeNodeClickEventArgs>? NodeMouseContextClick;

        public event EventHandler<TreeNodeClickEventArgs>? NodeMouseDoubleClick;

        public event EventHandler<TreeNodeClickEventArgs>? NodeMouseSingleClick;

        public event EventHandler<TreeNodeSelectionEventArgs>? NodeSelectionChanged;
    }
}