using System;

namespace Twisted.Tests.PC;

public sealed class DPCNode020XXXXX : DPCNode
{
    internal DPCNode020XXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(16);

        A = bytes.ReadInt32(4,  Endianness.LE);
        B = bytes.ReadInt32(8,  Endianness.LE);
        C = bytes.ReadInt32(12, Endianness.LE);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        var addresses = reader.ReadAddresses(b1);

        children = Array.Empty<int>();
    }

    public int A { get; }

    public int B { get; }

    public int C { get; }

    public override string ToString()
    {
        return ToStringVerbose
            ? $"{base.ToString()}, {nameof(A)}: {A}, {nameof(B)}: {B}, {nameof(C)}: {C}"
            : base.ToString();
    }
}