using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00010706 : Polygon, IPolygonF3C30
    {
        public Polygon00010706(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 28, 3, positionVertices, positionNormals, 20)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x30;
    }
}