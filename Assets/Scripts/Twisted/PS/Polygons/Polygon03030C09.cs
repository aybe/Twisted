using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon03030C09 : PolygonG3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
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
    }
}