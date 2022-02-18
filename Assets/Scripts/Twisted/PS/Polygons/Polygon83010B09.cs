using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon83010B09 : PolygonF3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 32 with E2
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

        protected override int? TextureElements { get; } = 3;

        protected override int? TexturePosition { get; } = 20;
    }
}