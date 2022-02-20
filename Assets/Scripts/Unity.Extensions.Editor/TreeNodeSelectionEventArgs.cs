using System;
using System.Collections.Generic;

namespace Unity.Extensions.Editor
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