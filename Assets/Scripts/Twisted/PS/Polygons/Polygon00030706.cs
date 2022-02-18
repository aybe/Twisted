using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00030706 : PolygonG4
        // TODO unknown int32 @ 12
    {
        public Polygon00030706(BinaryReader reader, int positionVertices)
            : base(reader, 28, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 3;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x30;
    }
}