using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00040A08 : Polygon, IPolygonG4C38
    {
        public Polygon00040A08(BinaryReader reader, int positionVertices, int normalsPosition)
            : base(reader, 40, 4, positionVertices, normalsPosition, 32)
        {
        }
    }
}