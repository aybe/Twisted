using Twisted.Extensions;
using Twisted.PS.Polygons;

namespace Twisted.PS;

public sealed class DMDNode00FF : DMDNode
{
    public uint PositionVertices { get; }

    public uint PositionNormals { get; }

    public uint PositionPolygons { get; }

    public IReadOnlyList<IPolygon> Polygons { get; }

    public byte Flags { get; }

    public DMDNode00FF(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        PositionVertices = ReadAddress(reader);
        PositionNormals  = ReadAddress(reader, false); // TODO could it be that out of bounds signifies no normals?
        PositionPolygons = ReadAddress(reader);

        var bytes = reader.ReadBytes(28);

        Flags = bytes[11];

        if ((Flags & 0x80) != 0)
        {
            var extraBytes = reader.ReadBytes(32 + 16); // TODO 00 10 00 00 .. .. .. .., 00 01 05 05 .. .. .. .. 
        }

        SetLength(reader);

        var count = bytes.ReadUInt16(0, Endianness.LE);

        reader.BaseStream.Position = PositionPolygons;

        Polygons = PolygonReader.TryRead(reader, count, PositionVertices, PositionNormals);
    }

    public override string ToString()
    {
        return $"{base.ToString()}, " +
               $"{nameof(PositionVertices)}: {PositionVertices}, " +
               $"{nameof(PositionNormals)}: {PositionNormals}, " +
               $"{nameof(PositionPolygons)}: {PositionPolygons}, " +
               $"{nameof(Polygons)}: {Polygons.Count}";
    }
}