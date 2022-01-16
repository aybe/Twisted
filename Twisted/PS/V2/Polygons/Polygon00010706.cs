namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon00010706 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00010706(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(16);
    }
}