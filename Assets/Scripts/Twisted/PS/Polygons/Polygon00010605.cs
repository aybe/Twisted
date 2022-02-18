using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00010605 : Polygon, IPolygonF4C28
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 20
    {
        public Polygon00010605(BinaryReader reader, int positionVertices)
            : base(reader, 24, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x28;
    }
}