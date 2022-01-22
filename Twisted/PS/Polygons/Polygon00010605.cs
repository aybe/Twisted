namespace Twisted.PS.Polygons;

internal sealed class Polygon00010605 : Polygon
{
    private byte[] Bytes;

    public Polygon00010605(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4, polygonSize: 24, polygonFaces: 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;

        Bytes = reader.ReadBytes(12);
    }
}