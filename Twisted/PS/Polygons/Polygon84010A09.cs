namespace Twisted.PS.Polygons;

internal sealed class Polygon84010A09 : Polygon
{
    private byte[] Bytes;

    public Polygon84010A09(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4, polygonSize: 40, polygonFaces: 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;
        Bytes = reader.ReadBytes(28);
    }
}