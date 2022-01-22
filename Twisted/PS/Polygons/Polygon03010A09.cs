using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal sealed class Polygon03010A09 : PolygonQuad
{
    private readonly byte[] Bytes;

    public Polygon03010A09(BinaryReader reader, int positionVertices, int normalsPosition)
        : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(28);

        var normal1 = Bytes.ReadUInt16(20, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(22, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(24, Endianness.LE);

        ReadNormals(reader, normalsPosition, normal1, normal2, normal3);
    }
}