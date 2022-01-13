using System.Collections.Generic;

namespace Twisted.Tests.PC;

public abstract class DPCNode
{
    private protected DPCNode(DPCNodeReader reader, DPCNode? parent = null)
    {
        Parent   = parent;
        Position = reader.Position;
    }

    public DPCNode? Parent { get; set; }

    public List<DPCNode> Children { get; } = new();

    public int Depth
    {
        get
        {
            var depth = 0;

            for (var node = this; node.Parent != null; node = node.Parent)
            {
                depth++;
            }

            return depth;
        }
    }

    public long Position { get; }

    public DPCNode Root
    {
        get
        {
            var root = this;

            for (var node = this; node.Parent != null; node = node.Parent)
            {
                root = node;
            }

            return root;
        }
    }
}