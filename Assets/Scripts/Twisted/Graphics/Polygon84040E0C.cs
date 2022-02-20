using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon84040E0C : PolygonGT4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 48 with E2
        // TODO unknown int32 @ 52
    {
        public Polygon84040E0C(BinaryReader reader, int positionVertices)
            : base(reader, 56, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 32;
    }
}