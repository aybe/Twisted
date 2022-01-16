namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon83030C09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon83030C09(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(36);
    }
}