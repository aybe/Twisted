using JetBrains.Annotations;
using Twisted.PS.V2.Polygons;

namespace Twisted.PS.V2;

[NoReorder]
public sealed class DMDNode00FF : DMDNode
{
    public readonly uint                    VerticesOffset;
    public readonly uint                    NormalOffset;
    public readonly uint                    PolygonsOffset;
    public readonly IReadOnlyList<IPolygon> Polygons;

    public DMDNode00FF(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        VerticesOffset = ReadAddress(reader);
        NormalOffset   = ReadAddress(reader, false); // TODO could it be that out of bounds signifies no normals?
        PolygonsOffset = ReadAddress(reader);

        var bytes = reader.ReadBytes(76);

        SetLength(reader);

        var count = bytes[0];

        reader.BaseStream.Position = PolygonsOffset;

        Polygons = PolygonReader.TryRead(reader, count, VerticesOffset, NormalOffset);
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