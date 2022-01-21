namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon03010807 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon03010807(BinaryReader reader, int positionVertices) : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(20);
    }
}