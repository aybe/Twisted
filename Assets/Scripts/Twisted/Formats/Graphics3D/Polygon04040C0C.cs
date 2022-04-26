using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon04040C0C : PolygonGT4
        // TODO unknown int32 @ 12
    {
        public Polygon04040C0C(BinaryReader reader, int positionVertices)
            : base(reader, 48, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3E;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 32;
    }
}