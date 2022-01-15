namespace Twisted.Tests.PC;

public sealed class DPCNode0107XXXX : DPCNode
{
    internal DPCNode0107XXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(20);

        A = bytes.ReadInt32(4,  Endianness.LE);
        B = bytes.ReadInt32(8,  Endianness.LE);
        C = bytes.ReadInt32(12, Endianness.LE);
        D = bytes.ReadInt32(16, Endianness.LE);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        children = reader.ReadAddresses(b1);

        SetLength(reader);
    }

    public int A { get; }

    public int B { get; }

    public int C { get; }

    public int D { get; }

    public override string ToString()
    {
        return ToStringVerbose
            ? $"{base.ToString()}, {nameof(A)}: {A}, {nameof(B)}: {B}, {nameof(C)}: {C}, {nameof(D)}: {D}"
            : base.ToString();
    }
}