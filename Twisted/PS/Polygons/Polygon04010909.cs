namespace Twisted.PS.Polygons;

internal sealed class Polygon04010909 : Polygon
{
    private byte[] Bytes;

    public Polygon04010909(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4, polygonSize: 36, polygonFaces: 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

//        Bytes = reader.ReadBytes(24);
    }
}