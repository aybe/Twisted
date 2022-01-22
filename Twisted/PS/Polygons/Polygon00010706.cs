using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal sealed class Polygon00010706 : Polygon
{
    private readonly byte[] Bytes;

    public Polygon00010706(BinaryReader reader, int positionVertices, int positionNormals)
        : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(16);

        var normal1 = Bytes.ReadUInt16(8,  Endianness.LE);
        var normal2 = Bytes.ReadUInt16(10, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(12, Endianness.LE);

        ReadNormals(reader, positionNormals, normal1, normal2, normal3);
    }
}