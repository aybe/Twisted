using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00040808 : PolygonG4
        // TODO unknown int32 @ 12
    {
        public Polygon00040808(BinaryReader reader, int positionVertices)
            : base(reader, 32, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x38;
    }
}