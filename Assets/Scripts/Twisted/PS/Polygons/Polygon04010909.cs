using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon04010909 : Polygon, IPolygonF4C2C
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 20
        // TODO unknown int32 @ 24
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
    {
        public Polygon04010909(BinaryReader reader, int positionVertices)
            : base(reader, 36, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x2C;
    }
}