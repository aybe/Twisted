namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon84010C0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon84010C0C(BinaryReader reader, int vertices) : base(reader, vertices)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(36);
    }
}