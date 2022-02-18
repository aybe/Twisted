using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00010505 : Polygon, IPolygonF4C28
    {
        public Polygon00010505(BinaryReader reader, int positionVertices)
            : base(reader, 20, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x28;
    }
}