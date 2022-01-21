using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V2.Polygons;

[Obsolete("This base class is not only for quads but triangles as well, push members up")]
internal abstract class PolygonQuad : Polygon
{
    protected PolygonQuad(
        BinaryReader reader,
        int positionVertices = -1,
        int countVertices = -1,
        int positionNormals = -1,
        int countNormals = -1
    )
        : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var indices = ReadIndices(4);

        Indices = indices.Select(s => (int)s).ToArray();

        if (positionVertices != -1)
        {
            Assert.IsFalse(countVertices <= 0, "countVertices <= 0");

            var position = reader.BaseStream.Position;

            foreach (var index in indices)
            {
                reader.BaseStream.Position = positionVertices + index * 8;
                var vertex = reader.ReadBytes(8);
            }

            reader.BaseStream.Position = position;
        }
        // ReSharper disable once RedundantIfElseBlock
        else
        {
            // TODO assert fail
        }
    }

    public override IReadOnlyList<int> Indices { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(Indices)}: {string.Join(", ", Indices)}";
    }
}