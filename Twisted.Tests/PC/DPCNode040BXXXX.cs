using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNode040BXXXX : DPCNode
{
    internal DPCNode040BXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(16);

        A = bytes.ReadInt32(4,  Endianness.LE);
        B = bytes.ReadInt32(8,  Endianness.LE);
        C = bytes.ReadInt32(12, Endianness.LE);

        // Assert.IsTrue(A > 0);
        // Assert.IsTrue(B > 0);
        // Assert.IsTrue(C > 0);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        // Assert.AreEqual(0, b1, Position.ToString());

        Assert.AreEqual(0, b2, Position.ToString());

        // Assert.AreEqual(1, b3, Position.ToString());

        // Assert.AreEqual(0, b4, Position.ToString());

        children = reader.ReadAddresses(b3);
    }

    public int A { get; }

    public int B { get; }

    public int C { get; }

    public override string ToString()
    {
        return ToStringVerbose
            ? $"{base.ToString()}, {nameof(A)}: {A}, {nameof(B)}: {B}, {nameof(C)}: {C}"
            : base.ToString();
    }
}