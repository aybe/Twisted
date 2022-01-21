namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon84010C0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon84010C0C(BinaryReader reader, int vertices) : base(reader, vertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(36);
        
        // TODO tex stuff ? 4 bytes
        // TODO ...
        // TODO tex stuff E2 ? 4 bytes
        // TODO normals ? 8 bytes
    }
}