using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00040A08 : Polygon, IPolygonG4C38
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
    {
        public Polygon00040A08(BinaryReader reader, int positionVertices, int normalsPosition)
            : base(reader, 40, 4, positionVertices, normalsPosition, 32)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x38;
    }
}