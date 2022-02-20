using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon00010708 : PolygonF4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 20
        // TODO unknown int32 @ 24
    {
        public Polygon00010708(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 28, 4, positionVertices, positionNormals, 20)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x38;
    }
}