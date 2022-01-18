using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PC;

public sealed class DPCNode0905XXXX : DPCNode
{
    internal DPCNode0905XXXX(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
    {
        reader.Position += 4;

        B1 = reader.ReadByte();

        B2 = reader.ReadByte();

        ChildrenCount = reader.ReadByte();

        B4 = reader.ReadByte();

        // Assert.AreEqual(0, b1, reader.Position.ToString());

        Assert.AreEqual(0, B2, reader.Position.ToString());

        Assert.AreEqual(0, B4, reader.Position.ToString());

        var addresses = reader.ReadAddresses(ChildrenCount);

        SetLength(reader);

        children = addresses;
    }

    public byte B1 { get; }

    public byte B2 { get; }

    public byte ChildrenCount { get; }

    public byte B4 { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(B1)}: {B1}, {nameof(B2)}: {B2}, {nameof(ChildrenCount)}: {ChildrenCount}, {nameof(B4)}: {B4}";
    }
}