using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon00030706 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00030706(BinaryReader reader, int positionVertices) : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(16);
    }
}