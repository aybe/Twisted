namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon04010B0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon04010B0C(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(32);
    }
}