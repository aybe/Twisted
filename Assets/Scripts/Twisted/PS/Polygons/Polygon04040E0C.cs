using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon04040E0C : PolygonG4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
        // TODO unknown int32 @ 40
        // TODO unknown int32 @ 44
        // TODO unknown int32 @ 48
        // TODO unknown int32 @ 52
    {
        public Polygon04040E0C(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 56, 4, positionVertices, positionNormals, 48)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;
    }
}