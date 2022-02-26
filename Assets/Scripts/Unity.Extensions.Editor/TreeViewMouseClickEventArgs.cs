using System;

namespace Unity.Extensions.Editor
{
    public sealed class TreeViewMouseClickEventArgs : EventArgs
    {
        public TreeViewMouseClickEventArgs(TreeNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public TreeNode Node { get; }
    }
}