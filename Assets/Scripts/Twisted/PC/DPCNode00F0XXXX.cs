using System;

namespace Twisted.PC
{
    public sealed class DPCNode00F0XXXX : DPCNode
    {
        internal DPCNode00F0XXXX(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
        {
            var bytes = reader.ReadBytes(16);

            SetLength(reader);

            children = Array.Empty<int>();
        }
    }
}