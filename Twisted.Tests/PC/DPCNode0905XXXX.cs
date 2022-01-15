using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNode0905XXXX : DPCNode
{
    internal DPCNode0905XXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        reader.Position += 4;

        B1 = reader.ReadByte();

        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        // Assert.AreEqual(0, b1, reader.Position.ToString());

        Assert.AreEqual(0, b2, reader.Position.ToString());

        Assert.AreEqual(0, b4, reader.Position.ToString());

        var addresses = reader.ReadAddresses(b3);

        SetLength(reader);

        children = addresses;
    }

    public byte B1 { get; }

    public override string ToString()
    {
        var s = base.ToString();

        if (B1 != 0)
        {
            s = $"{s}, {nameof(B1)}: {B1}";
        }

        return s;
    }
}