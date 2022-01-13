using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNodeRoot : DPCNode
{
    internal DPCNodeRoot(DPCNodeReader reader, DPCNode? parent = null) : base(reader, parent)
    {
    }

    internal DPCNodeRoot(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
    {
        var address = reader.ReadAddress();
        
        reader.Position = address;

        var count = reader.ReadInt32(Endianness.LE);

        children = reader.ReadAddresses(count);
    }
}