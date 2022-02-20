using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon00040908 : PolygonG4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 32
    {
        public Polygon00040908(BinaryReader reader, int positionVertices)
            : base(reader, 36, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x38;
    }
}