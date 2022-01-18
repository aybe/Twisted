using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PC;

public sealed class DPCNode03090000 : DPCNode
{
    internal DPCNode03090000(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
    {
        var bytes = reader.ReadBytes(8);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        Assert.AreEqual(0, b1, Position.ToString());

        Assert.IsTrue(b2 is 1 or 2, Position.ToString());

        Assert.AreEqual(0, b3, Position.ToString());

        Assert.IsTrue(b4 is 1 or 2 or 3, Position.ToString());

        var i1 = reader.ReadInt32(Endianness.LE);

        Assert.AreEqual(0, i1, Position.ToString());

        var addresses = reader.ReadAddresses(b4);

        SetLength(reader);

        children = addresses;
    }
}