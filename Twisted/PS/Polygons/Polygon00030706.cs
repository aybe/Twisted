using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00030706 : Polygon
    {
        public Polygon00030706(BinaryReader reader, int positionVertices)
            : base(reader, 28, 3, positionVertices)
        {
            // TODO int32 + rgba * 4?
        }
    }
}