using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon84010B09 : PolygonF3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 36 with E2
        // TODO unknown int32 @ 40
    {
        public Polygon84010B09(BinaryReader reader, int positionVertices)
            : base(reader, 44, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x2C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 20;
    }
}