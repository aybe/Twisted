using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal sealed class Polygon03030C09 : PolygonQuad
{
    private readonly byte[] Bytes;

    public Polygon03030C09(BinaryReader reader, int positionVertices, int positionNormals)
        : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(36);

        var normal1 = Bytes.ReadUInt16(28, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(30, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(32, Endianness.LE);

        ReadNormals(reader, positionNormals, normal1, normal2, normal3);
    }
}