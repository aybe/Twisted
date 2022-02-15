using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon04040E0C : Polygon
    {
        public Polygon04040E0C(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 56, 4, positionVertices, positionNormals, 48)
        {
        }
    }
}