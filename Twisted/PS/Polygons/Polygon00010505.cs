namespace Twisted.PS.Polygons;

internal sealed class Polygon00010505 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00010505(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(8);
    }
}