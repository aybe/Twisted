using System;

namespace Twisted.Tests.PC;

public sealed class DPCNode08FF0000 : DPCNode
{
    internal DPCNode08FF0000(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(120);

        children = Array.Empty<int>();
    }
}