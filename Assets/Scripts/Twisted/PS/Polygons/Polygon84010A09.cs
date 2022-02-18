using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon84010A09 : PolygonF3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 20
        // TODO unknown int32 @ 24
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36 ends with E2
    {
        public Polygon84010A09(BinaryReader reader, int positionVertices)
            : base(reader, 40, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x2C;
    }
}