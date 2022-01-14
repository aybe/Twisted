namespace Twisted.Tests.PC;

public sealed class DPCNodeXXXXXXXX : DPCNode
{
    internal DPCNodeXXXXXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(4);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        var addresses = reader.ReadAddresses(1);

        children = addresses;
    }
}