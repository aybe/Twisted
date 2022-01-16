namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon84040E0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon84040E0C(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(44);
    }
}