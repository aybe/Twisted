using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.Polygons;

internal sealed class Polygon00030706 : Polygon
{
    private byte[] Bytes;

    public Polygon00030706(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(16);

        // TODO int32 + rgba * 4?
    }
}