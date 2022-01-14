using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNodeXXXXXXXX : DPCNode
{
    internal DPCNodeXXXXXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(8);

        var i1 = bytes.ReadInt32(4, Endianness.LE);

        // Assert.AreEqual(0, i1, Position.ToString());

        var addresses = reader.ReadAddresses(1);

        children = addresses;
    }
}