namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon04010A09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon04010A09(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(28);
    }
}