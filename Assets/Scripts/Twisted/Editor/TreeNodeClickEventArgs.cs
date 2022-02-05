﻿using System;

namespace Twisted.Editor
{
    public sealed class TreeNodeClickEventArgs : EventArgs
    {
        internal TreeNodeClickEventArgs(TreeNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public TreeNode Node { get; }
    }
}