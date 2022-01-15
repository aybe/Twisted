using System;

namespace Twisted.Tests.PC;

public sealed class DPCNode00100000 : DPCNode
{
    internal DPCNode00100000(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(16);

        SetLength(reader);

        children = Array.Empty<int>();
    }
}