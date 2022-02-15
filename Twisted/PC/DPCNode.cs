using System.Collections.Generic;

namespace Twisted.PC
{
    public abstract class DPCNode : TreeNode
    {
        private protected DPCNode(DPCNodeReader reader, DPCNode? parent = null) : base(parent)
        {
            Position = reader.Position;
        }

        public static bool ToStringVerbose { get; set; }

        public List<DPCNode> Children { get; } = new List<DPCNode>();

        public long Position { get; }

        public long Length { get; private set; }

        public override string ToString()
        {
            return $"{GetType().Name[nameof(DPCNode).Length..]}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(Depth)}: {Depth}";
        }

        private protected void SetLength(DPCNodeReader reader)
        {
            Length = reader.Position - Position;
        }
    }
}