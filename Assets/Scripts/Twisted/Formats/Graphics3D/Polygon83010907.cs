using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon83010907 : PolygonFT3
        // TODO unknown int32 @ 12
    {
        public Polygon83010907(BinaryReader reader, int positionVertices)
            : base(reader, 36, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x24;

        protected override int? TextureElements { get; } = 3;

        protected override int? TextureElementsPosition { get; } = 20;

        protected override int? TextureWindowPosition { get; } = 32;
    }
}