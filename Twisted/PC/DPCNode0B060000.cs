using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PC;

public sealed class DPCNode0B060000 : DPCNode
{
    internal DPCNode0B060000(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(8);

        var i1 = bytes.ReadInt32(4, Endianness.LE);

        Assert.IsTrue(i1 is 0);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        Assert.IsTrue(b1 is 0);
        Assert.IsTrue(b2 is 0);
        Assert.IsTrue(b3 is 1 or 2);
        Assert.IsTrue(b4 is 0);

        var addresses = reader.ReadAddresses(b3 == 1 ? 3 : 4);

        SetLength(reader);

        children = addresses;
    }
}