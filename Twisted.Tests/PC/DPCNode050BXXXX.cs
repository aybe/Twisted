using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNode050BXXXX : DPCNode
{
    internal DPCNode050BXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        reader.ReadBytes(52);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        Assert.AreEqual(1, b3, nameof(b3));
        // Assert.AreEqual(1, b4, nameof(b4));

        children = reader.ReadAddresses(b3);
    }
}