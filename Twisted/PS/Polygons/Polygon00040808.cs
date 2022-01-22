namespace Twisted.PS.Polygons;

internal sealed class Polygon00040808 : Polygon
{
    private byte[] Bytes;

    public Polygon00040808(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4, polygonSize: 32, polygonFaces: 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;
        Bytes = reader.ReadBytes(20);

        // TODO four colors at end?
    }
}