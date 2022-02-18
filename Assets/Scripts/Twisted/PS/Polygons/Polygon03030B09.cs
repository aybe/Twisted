using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon03030B09 : Polygon, IPolygonG3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
        // TODO unknown int32 @ 40
    {
        public Polygon03030B09(BinaryReader reader, int positionVertices)
            : base(reader, 44, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 3;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x34;
    }
}