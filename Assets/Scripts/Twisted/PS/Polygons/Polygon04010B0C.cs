using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon04010B0C : Polygon
    {
        public Polygon04010B0C(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 44, 4, positionVertices, positionNormals, 36)
        {
            var data = GetObjectData();

            TextureInfo = ReadTexture(data, 20);

            TextureUVs = ReadTextureUVs(data, 20, 4);
        }
    }
}