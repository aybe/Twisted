using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon84040D0C : PolygonG4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
        // TODO unknown int32 @ 40
        // TODO unknown int32 @ 44
        // TODO unknown int32 @ 48 ends with E2
    {
        public Polygon84040D0C(BinaryReader reader, int positionVertices)
            : base(reader, 52, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 32;
    }
}