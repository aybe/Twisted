namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon00010708 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00010708(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(16);
    }
}