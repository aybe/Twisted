using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal sealed class Polygon00010708 : PolygonQuad
{
    private readonly byte[] Bytes;

    public Polygon00010708(BinaryReader reader, int positionVertices, int positionNormals)
        : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(16);

        var normal1 = Bytes.ReadUInt16(8,  Endianness.LE);
        var normal2 = Bytes.ReadUInt16(10, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(12, Endianness.LE);
        var normal4 = Bytes.ReadUInt16(14, Endianness.LE);

        ReadNormals(reader, positionNormals, normal1, normal2, normal3, normal4);
    }
}