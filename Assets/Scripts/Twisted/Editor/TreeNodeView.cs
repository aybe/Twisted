using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Twisted.Editor
{
    public sealed class TreeNodeView : TreeView
    {
        public TreeNodeView(TreeViewState state, TreeNode? root = default) : base(state)
        {
            enableItemHovering            = true;
            showAlternatingRowBackgrounds = true;

            SetRoot(root);
        }

        private TreeNode? Root { get; set; }

        public bool HasRoot => Root != null;

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

        protected override void RowGUI(RowGUIArgs args)
        {
            var rect = new Rect(args.rowRect);

            rect.xMin += GetContentIndent(args.item);

            CenterRectUsingSingleLineHeight(ref rect);

            GUI.Label(rect, args.item.displayName);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            var nodes = selectedIds.Select(id => ((TreeViewItem<TreeNode>)FindItem(id, rootItem)).Data!).ToList();

            OnSelectionChanged?.Invoke(this, new TreeNodeViewSelectionEventArgs(nodes));
        }

        public event EventHandler<TreeNodeViewSelectionEventArgs>? OnSelectionChanged;

        private sealed class TreeViewItem<T> : TreeViewItem
        {
            public TreeViewItem(int id, int depth, string displayName, T? data = default) : base(id, depth, displayName)
            {
                Data = data;
            }

            public T? Data { get; }
        }
    }
}