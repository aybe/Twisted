namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon84040D0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon84040D0C(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(40);
    }
}