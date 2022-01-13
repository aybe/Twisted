namespace Twisted.Tests.PC;

public sealed class DPCNode0905XXXX : DPCNode
{
    internal DPCNode0905XXXX(DPCNodeReader reader, out int[] addresses) : base(reader)
    {
        reader.Position += 4;

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        addresses = reader.ReadAddresses(b3);
    }
}