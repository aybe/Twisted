namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon00040908 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00040908(BinaryReader reader, int positionVertices) : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(24);
    }
}