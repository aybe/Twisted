using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon04010909 : PolygonFT4
        // TODO unknown int32 @ 12
    {
        public Polygon04010909(BinaryReader reader, int positionVertices)
            : base(reader, 36, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x2C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TextureElementsPosition { get; } = 20;
    }
}