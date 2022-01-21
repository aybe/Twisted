namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon84040E0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon84040E0C(BinaryReader reader, int positionVertices) : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(44);
    }
}