namespace Twisted.PS.Polygons;

internal sealed class Polygon84040D0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon84040D0C(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(40);
    }
}