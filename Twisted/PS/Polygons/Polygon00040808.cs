namespace Twisted.PS.Polygons;

internal sealed class Polygon00040808 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00040808(BinaryReader reader, int positionVertices) : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(20);
        
        // TODO four colors at end?
    }
}