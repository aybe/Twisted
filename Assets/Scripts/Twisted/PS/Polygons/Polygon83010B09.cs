using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon83010B09 : Polygon, IPolygonF3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 20
        // TODO unknown int32 @ 24
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
        // TODO unknown int32 @ 40
    {
        public Polygon83010B09(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 44, 3, positionVertices, positionNormals, 36)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x34;
    }
}