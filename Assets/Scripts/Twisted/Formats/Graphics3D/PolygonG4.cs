using System.IO;

namespace Twisted.Formats.Graphics3D
{
    public abstract class PolygonG4 : PolygonG
    {
        protected PolygonG4(BinaryReader reader, int polygonSize = -1, int polygonFaces = -1, int positionVertices = -1, int positionNormals = -1, int offsetNormals = -1)
            : base(reader, polygonSize, polygonFaces, positionVertices, positionNormals, offsetNormals)
        {
        }
    }
}