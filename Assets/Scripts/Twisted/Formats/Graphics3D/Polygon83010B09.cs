using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon83010B09 : PolygonFT3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 36
        // TODO unknown int32 @ 40
    {
        public Polygon83010B09(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 44, 3, positionVertices, positionNormals, 36)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x34;

        protected override int? TextureElements { get; } = 3;

        protected override int? TextureElementsPosition { get; } = 20;

        protected override int? TextureWindowPosition { get; } = 32;
    }
}