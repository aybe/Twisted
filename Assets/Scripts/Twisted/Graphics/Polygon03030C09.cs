using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon03030C09 : PolygonGT3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 40
        // TODO unknown int32 @ 44
    {
        public Polygon03030C09(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 48, 3, positionVertices, positionNormals, 40)
        {
        }

        protected override int? ColorElements { get; } = 3;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x34;

        protected override int? TextureElements { get; } = 3;

        protected override int? TexturePosition { get; } = 28;
    }
}