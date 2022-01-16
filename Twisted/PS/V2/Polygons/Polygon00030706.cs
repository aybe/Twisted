namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon00030706 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00030706(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(16);
    }
}