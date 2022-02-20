using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon00010604 : PolygonF3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 20
    {
        public Polygon00010604(BinaryReader reader, int positionVertices)
            : base(reader, 24, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x20;
    }
}