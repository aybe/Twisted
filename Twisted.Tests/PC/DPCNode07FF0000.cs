using System;

namespace Twisted.Tests.PC;

public sealed class DPCNode07FF0000 : DPCNode
{
    internal DPCNode07FF0000(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(72);

        SetLength(reader);

        children = Array.Empty<int>();
    }
}