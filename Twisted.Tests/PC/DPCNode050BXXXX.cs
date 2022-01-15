using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNode050BXXXX : DPCNode
{
    internal DPCNode050BXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(52);

        var i1  = bytes.ReadInt32(4,  Endianness.LE);
        var i2  = bytes.ReadInt32(8,  Endianness.LE);
        var i3  = bytes.ReadInt32(12, Endianness.LE);
        var i4  = bytes.ReadInt32(16, Endianness.LE);
        var i5  = bytes.ReadInt32(20, Endianness.LE);
        var i6  = bytes.ReadInt32(24, Endianness.LE);
        var i7  = bytes.ReadInt32(28, Endianness.LE);
        var i8  = bytes.ReadInt32(32, Endianness.LE);
        var i9  = bytes.ReadInt32(36, Endianness.LE);
        var i10 = bytes.ReadInt32(40, Endianness.LE);
        var i11 = bytes.ReadInt32(44, Endianness.LE);
        var i12 = bytes.ReadInt32(48, Endianness.LE);

        // Assert.AreEqual(0, i1, reader.Position.ToString());
        // Assert.AreEqual(0, i2, reader.Position.ToString());
        // Assert.AreEqual(0, i3, reader.Position.ToString());
        // Assert.AreEqual(0, i4,  reader.Position.ToString());
        // Assert.AreEqual(0, i5,  reader.Position.ToString());
        // Assert.AreEqual(0, i6,  reader.Position.ToString());
        // Assert.AreEqual(0, i7,  reader.Position.ToString());
        // Assert.AreEqual(0, i8,  reader.Position.ToString());
        // Assert.AreEqual(0, i9,  reader.Position.ToString());
        // Assert.AreEqual(0, i10, reader.Position.ToString());
        // Assert.AreEqual(0, i11, reader.Position.ToString());
        // Assert.AreEqual(0, i12, reader.Position.ToString());

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        Assert.AreEqual(1, b3, nameof(b3));
        // Assert.AreEqual(1, b4, nameof(b4));

        var addresses = reader.ReadAddresses(b3);

        SetLength(reader);

        children = addresses;
    }
}