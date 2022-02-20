﻿using System.IO;

namespace Twisted.Graphics
{
    public abstract class PolygonF4 : PolygonF
    {
        protected PolygonF4(BinaryReader reader, int polygonSize = -1, int polygonFaces = -1, int positionVertices = -1, int positionNormals = -1, int offsetNormals = -1)
            : base(reader, polygonSize, polygonFaces, positionVertices, positionNormals, offsetNormals)
        {
        }
    }
}