namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon04010909 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon04010909(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(24);
    }
}