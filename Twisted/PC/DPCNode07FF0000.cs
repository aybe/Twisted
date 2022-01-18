﻿namespace Twisted.PC;

public sealed class DPCNode07FF0000 : DPCNode
{
    internal DPCNode07FF0000(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
    {
        var bytes = reader.ReadBytes(72);

        SetLength(reader);

        children = Array.Empty<int>();
    }
}