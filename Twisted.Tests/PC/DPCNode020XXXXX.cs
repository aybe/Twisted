using System;

namespace Twisted.Tests.PC;

public sealed class DPCNode020XXXXX : DPCNode
{
    internal DPCNode020XXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        reader.Position += 16;

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        var addresses = reader.ReadAddresses(b1);

        children = Array.Empty<int>();
    }
}