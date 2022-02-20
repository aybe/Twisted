using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon83030C09 : PolygonGT3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 40 with E2
        // TODO unknown int32 @ 44
    {
        public Polygon83030C09(BinaryReader reader, int positionVertices)
            : base(reader, 48, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 3;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x34;

        protected override int? TextureElements { get; } = 3;

        protected override int? TexturePosition { get; } = 28;
    }
}