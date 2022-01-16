using System;
using System.IO;
using JetBrains.Annotations;
using Twisted.Tests.PS.V2.Extensions;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

public sealed class DMDNode020X : DMDNode
{
    public DMDNode020X([CanBeNull] DMDNode parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown1 = reader.ReadInt16(Endianness.LittleEndian);
        var unknown2 = reader.ReadBytes(12);

        var count = reader.ReadInt32(Endianness.LittleEndian);
        ReadAddressesThenNodes(reader, count);
    }
}