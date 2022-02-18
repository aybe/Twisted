using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00030906 : Polygon, IPolygonG3C30
    {
        public Polygon00030906(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 36, 3, positionVertices, positionNormals, 28)
        {
        }
    }
}