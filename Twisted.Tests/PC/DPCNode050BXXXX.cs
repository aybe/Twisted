namespace Twisted.Tests.PC;

public sealed class DPCNode050BXXXX : DPCNode
{
    internal DPCNode050BXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        reader.Position += 52;

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        children = reader.ReadAddresses(b3);
    }
}