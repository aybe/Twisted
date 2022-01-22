namespace Twisted.PS.Polygons;

internal sealed class Polygon84040D0C : Polygon
{
    private byte[] Bytes;

    public Polygon84040D0C(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4, polygonSize: 52, polygonFaces: 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;
        Bytes = reader.ReadBytes(40);
    }
}