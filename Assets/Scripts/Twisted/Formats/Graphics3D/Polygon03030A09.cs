using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon03030A09 : PolygonGT3
        // TODO unknown int32 @ 12
    {
        public Polygon03030A09(BinaryReader reader, int positionVertices)
            : base(reader, 40, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 3;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x34;

        protected override int? TextureElements { get; } = 3;

        protected override int? TextureElementsPosition { get; } = 28;
    }
}