using System;
using System.IO;
using JetBrains.Annotations;
using Twisted.Tests.PS.V2.Extensions;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

public sealed class DMDNode0903 : DMDNode
{
    public DMDNode0903([CanBeNull] DMDNode parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown1 = reader.ReadInt16(Endianness.LittleEndian);
        var b1       = reader.ReadByte();
        var b2       = reader.ReadByte();
        var b3       = reader.ReadByte();
        var b4       = reader.ReadByte();

        ReadAddressesThenNodes(reader, b3);
    }
}