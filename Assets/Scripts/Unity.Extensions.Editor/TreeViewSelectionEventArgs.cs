using System;
using System.Collections.Generic;

namespace Unity.Extensions.Editor
{
    public class TreeViewSelectionEventArgs<T> : EventArgs where T : TreeNode
    {
        internal TreeViewSelectionEventArgs(IList<T> nodes)
        {
            Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
        }

        public IList<T> Nodes { get; }
    }
}