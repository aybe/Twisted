using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal sealed class Polygon00040A08 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00040A08(BinaryReader reader, int positionVertices, int normalsPosition) 
        : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(28);

        var normal1 = Bytes.ReadUInt16(20, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(22, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(24, Endianness.LE);
        var normal4 = Bytes.ReadUInt16(26, Endianness.LE);

        ReadNormals(reader, normalsPosition, normal1, normal2, normal3, normal4);
    }
}