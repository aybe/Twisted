using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal sealed class Polygon04040E0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon04040E0C(BinaryReader reader, int positionVertices, int positionNormals)
        : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(44);

        var normal1 = Bytes.ReadUInt16(36, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(38, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(40, Endianness.LE);
        var normal4 = Bytes.ReadUInt16(42, Endianness.LE);

        ReadNormals(reader, positionNormals, normal1, normal2, normal3, normal4);
    }
}