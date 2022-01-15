using System;

namespace Twisted.Tests.PC;

public sealed class DPCNode00F0XXXX : DPCNode
{
    internal DPCNode00F0XXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(16);

        SetLength(reader);

        children = Array.Empty<int>();
    }
}