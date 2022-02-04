using System;
using System.Collections.Generic;

namespace Twisted.Editor
{
    public sealed class TreeNodeViewSelectionEventArgs : EventArgs
    {
        internal TreeNodeViewSelectionEventArgs(IList<TreeNode> nodes, int button, int clicks)
        {
            Nodes  = nodes;
            Button = button;
            Clicks = clicks;
        }

        public IList<TreeNode> Nodes { get; }

        public int Button { get; }

        public int Clicks { get; }
    }
}