using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon03010A09 : PolygonF3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 20
        // TODO unknown int32 @ 24
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
    {
        public Polygon03010A09(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 40, 3, positionVertices, positionNormals, 32)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x34;

        protected override int? TextureElements { get; } = 3;

        protected override int? TexturePosition { get; } = 20;
    }
}