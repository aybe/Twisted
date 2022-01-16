namespace Twisted.PS.V2.Polygons;

internal abstract class PolygonQuad : Polygon
{
    protected PolygonQuad(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var indices = ReadIndices(4);

        Indices = indices.Select(s => (int)s).ToArray();
    }

    public override IReadOnlyList<int> Indices { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(Indices)}: {string.Join(", ", Indices)}";
    }
}