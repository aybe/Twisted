using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon84040F0C : PolygonGT4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 52
        // TODO unknown int32 @ 56
    {
        public Polygon84040F0C(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 60, 4, positionVertices, positionNormals, 52)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 32;

        protected override int? TextureWindowPosition { get; } = 48;
    }
}