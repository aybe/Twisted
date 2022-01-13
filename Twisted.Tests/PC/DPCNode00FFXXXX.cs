using System;

namespace Twisted.Tests.PC;

public sealed class DPCNode00FFXXXX : DPCNode
{
    internal DPCNode00FFXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        reader.Position += 4;

        var addresses = reader.ReadAddresses(3);

        children = Array.Empty<int>();
    }
}