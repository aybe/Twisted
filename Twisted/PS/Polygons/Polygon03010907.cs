using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.Polygons;

internal sealed class Polygon03010907 : Polygon
{
    private byte[] Bytes;

    public Polygon03010907(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 3, polygonSize: 36, polygonFaces: 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;
        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(24);
    }
}