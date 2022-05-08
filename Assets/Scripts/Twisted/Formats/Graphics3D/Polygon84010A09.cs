using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon84010A09 : PolygonFT3
        // TODO unknown int32 @ 12
    {
        public Polygon84010A09(BinaryReader reader, int positionVertices)
            : base(reader, 40, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x2C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 20;

        protected override int? TextureWindowPosition { get; } = 36;
    }
}