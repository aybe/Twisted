using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon83030D09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon83030D09(BinaryReader reader, int positionVertices, int positionNormals)
        : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(40);

        var normal1 = Bytes.ReadUInt16(32, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(34, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(36, Endianness.LE);

        ReadNormals(reader, positionNormals, normal1, normal2, normal3);
    }
}