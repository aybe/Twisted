using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon04010B0C : PolygonF4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 20
        // TODO unknown int32 @ 24
        // TODO unknown int32 @ 28
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
        // TODO unknown int32 @ 40
        // TODO unknown int32 @ 44
    {
        public Polygon04010B0C(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 44, 4, positionVertices, positionNormals, 36)
        {
            var data = GetObjectData();

            TextureInfo = ReadTexture(data, 20);
        protected override int? ColorElements { get; } = 1;

            TextureUVs = ReadTextureUVs(data, 20, 4);
        }
        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;
    }
}