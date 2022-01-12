using System.Collections.Generic;

namespace Twisted.Tests;

public abstract class DPCNode
{
    public DPCNode? Parent { get; set; }

    public List<DPCNode> Children { get; set; } = new();

    public DPCNode Root
    {
        get
        {
            var root = this;

            while (root.Parent != null)
            {
                root = root.Parent;
            }

            return root;
        }
    }
}