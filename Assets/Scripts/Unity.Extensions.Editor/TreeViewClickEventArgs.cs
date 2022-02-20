using System;

namespace Unity.Extensions.Editor
{
    public sealed class TreeViewClickEventArgs : EventArgs
    {
        internal TreeViewClickEventArgs(TreeNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public TreeNode Node { get; }
    }
}