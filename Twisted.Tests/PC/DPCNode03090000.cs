using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNode03090000 : DPCNode
{
    internal DPCNode03090000(DPCNodeReader reader, out int[] addresses) : base(reader)
    {
        var bytes = reader.ReadBytes(8);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        // addresses = reader.ReadAddresses(b4);
        addresses = Array.Empty<int>();
        Assert.AreEqual(0, b1, Position.ToString());

        Assert.IsTrue(b2 is 1 or 2, Position.ToString());

        Assert.AreEqual(0, b3, Position.ToString());

        Assert.IsTrue(b4 is 1 or 2 or 3, Position.ToString());
    }
}