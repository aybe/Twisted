namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon04010A09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon04010A09(BinaryReader reader, int positionVertices) : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(28);
    }
}