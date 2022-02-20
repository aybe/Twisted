using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon00030906 : PolygonG3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
    {
        public Polygon00030906(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 36, 3, positionVertices, positionNormals, 28)
        {
        }

        protected override int? ColorElements { get; } = 3;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x30;
    }
}