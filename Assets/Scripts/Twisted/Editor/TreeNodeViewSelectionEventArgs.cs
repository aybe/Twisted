using System;
using System.Collections.Generic;

namespace Twisted.Editor
{
    public sealed class TreeNodeViewSelectionEventArgs : EventArgs
    {
        internal TreeNodeViewSelectionEventArgs(IList<TreeNode> nodes)
        {
            Nodes = nodes;
        }

        public IList<TreeNode> Nodes { get; }
    }
}