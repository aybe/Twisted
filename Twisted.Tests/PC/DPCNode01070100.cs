using System;

namespace Twisted.Tests.PC;

public sealed class DPCNode01070100 : DPCNode
{
    internal DPCNode01070100(DPCNodeReader reader, out int[] children) : base(reader)
    {
        var bytes = reader.ReadBytes(20);

        A = bytes.ReadInt32(4,  Endianness.LE);
        B = bytes.ReadInt32(8,  Endianness.LE);
        C = bytes.ReadInt32(12, Endianness.LE);

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        var addresses = reader.ReadAddresses(b1);

        children = Array.Empty<int>();

        // children = addresses; // BUG TODO this may not be regular nodes
    }

    public int A { get; }

    public int B { get; }

    public int C { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(A)}: {A}, {nameof(B)}: {B}, {nameof(C)}: {C}";
    }
}