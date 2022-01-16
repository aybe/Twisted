using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNode0107XXXX : DPCNode
{
    internal DPCNode0107XXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(20);

        I1 = bytes.ReadInt32(4,  Endianness.LE);
        I2 = bytes.ReadInt32(8,  Endianness.LE);
        I3 = bytes.ReadInt32(12, Endianness.LE);
        I4 = bytes.ReadInt32(16, Endianness.LE);

        ChildrenCount = reader.ReadByte();

        B2 = reader.ReadByte();
        B3 = reader.ReadByte();
        B4 = reader.ReadByte();

        // Assert.AreNotEqual(0, b1, Position.ToString());

        Assert.AreEqual((byte)0, B4, Position.ToString());

        children = reader.ReadAddresses(ChildrenCount);

        SetLength(reader);
    }

    public int I1 { get; }

    public int I2 { get; }

    public int I3 { get; }

    public int I4 { get; }

    public byte ChildrenCount { get; }

    public byte B2 { get; }

    public byte B3 { get; }

    public byte B4 { get; }

    public override string ToString()
    {
        return
            $"{base.ToString()}, {nameof(I1)}: {I1}, {nameof(I2)}: {I2}, {nameof(I3)}: {I3}, {nameof(I4)}: {I4}, {nameof(ChildrenCount)}: {ChildrenCount}, {nameof(B2)}: {B2}, {nameof(B3)}: {B3}, {nameof(B4)}: {B4}";
    }
}