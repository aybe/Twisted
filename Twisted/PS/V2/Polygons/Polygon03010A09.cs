namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon03010A09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon03010A09(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(28);
    }
}