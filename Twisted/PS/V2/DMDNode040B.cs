﻿namespace Twisted.PS.V2;

public sealed class DMDNode040B : DMDNode
{
    public DMDNode040B(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadBytes(16);

        var countMaybe = unknown2[14]; // TODO

        ReadAddressesThenNodes(reader, 1);
    }
}