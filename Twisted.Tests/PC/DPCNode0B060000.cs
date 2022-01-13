namespace Twisted.Tests.PC;

public sealed class DPCNode0B060000 : DPCNode
{
    internal DPCNode0B060000(DPCNodeReader reader, out int[] addresses) : base(reader)
    {
        var bytes = reader.ReadBytes(8);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        addresses = reader.ReadAddresses(b3 == 1 ? 3 : 4);
    }
}