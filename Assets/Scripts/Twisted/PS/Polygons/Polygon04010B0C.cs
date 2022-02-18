using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon04010B0C : PolygonFT4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 36
        // TODO unknown int32 @ 40
    {
        public Polygon04010B0C(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 44, 4, positionVertices, positionNormals, 36)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 20;
    }
}