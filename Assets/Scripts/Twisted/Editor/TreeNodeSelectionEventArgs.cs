using System;
using System.Collections.Generic;

namespace Twisted.Editor
{
    public class TreeNodeSelectionEventArgs : EventArgs
    {
        internal TreeNodeSelectionEventArgs(IList<TreeNode> nodes)
        {
            Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
        }

        public IList<TreeNode> Nodes { get; }
    }
}