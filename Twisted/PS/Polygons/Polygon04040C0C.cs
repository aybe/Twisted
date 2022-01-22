namespace Twisted.PS.Polygons;

internal sealed class Polygon04040C0C : Polygon
{
    private byte[] Bytes;

    public Polygon04040C0C(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4, polygonSize: 48, polygonFaces: 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;
        Bytes = reader.ReadBytes(36);
    }
}