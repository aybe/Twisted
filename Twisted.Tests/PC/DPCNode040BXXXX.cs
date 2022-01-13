namespace Twisted.Tests.PC;

public sealed class DPCNode040BXXXX : DPCNode
{
    internal DPCNode040BXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        reader.Position += 16;

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        children = reader.ReadAddresses(b3);
    }
}