using Twisted.Extensions;

namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon84040F0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon84040F0C(BinaryReader reader, int positionVertices, int positionNormals)
        : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(48);

        var normal1 = Bytes.ReadUInt16(40, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(42, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(44, Endianness.LE);
        var normal4 = Bytes.ReadUInt16(46, Endianness.LE);

        ReadNormals(reader, positionNormals, normal1, normal2, normal3, normal4);
    }
}