namespace Twisted.Tests.PC;

public sealed class DPCNode0107XXXX : DPCNode
{
    internal DPCNode0107XXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        reader.Position += 20;

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        children = reader.ReadAddresses(b1);
    }
}