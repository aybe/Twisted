using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon83010B09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon83010B09(BinaryReader reader, int positionVertices, int positionNormals) 
        : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3]); // TODO this should be a triangle

        Bytes = reader.ReadBytes(32);

        var normal1 = Bytes.ReadUInt16(24, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(26, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(28, Endianness.LE);

        ReadNormals(reader, positionNormals, normal1, normal2, normal3);
    }
}