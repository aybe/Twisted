using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Twisted.Tests.PS.V2.Extensions;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

[NoReorder]
public sealed class DMDNode00FF : DMDNode
{
    internal readonly uint                    VerticesOffset;
    internal readonly uint                    NormalOffset;
    internal readonly uint                    PolygonsOffset;
    public readonly   IReadOnlyList<IPolygon> Polygons;

    public DMDNode00FF([CanBeNull] DMDNode parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown1 = reader.ReadInt16(Endianness.LittleEndian);

        VerticesOffset = ReadAddress(reader);
        NormalOffset   = ReadAddress(reader);
        PolygonsOffset = ReadAddress(reader);

        var bytes = reader.ReadBytes(4);

        var count = bytes[0];

        reader.BaseStream.Position = PolygonsOffset;

        Polygons = PolygonReader.TryRead(reader, count);
    }

    public override string ToString()
    {
        return $"{base.ToString()}, " +
               $"{nameof(VerticesOffset)}: {VerticesOffset}, " +
               $"{nameof(NormalOffset)}: {NormalOffset}, " +
               $"{nameof(PolygonsOffset)}: {PolygonsOffset}, " +
               $"{nameof(Polygons)}: {Polygons.Count}";
    }
}