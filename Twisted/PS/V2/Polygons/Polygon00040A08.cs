namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon00040A08 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00040A08(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(28);
    }
}