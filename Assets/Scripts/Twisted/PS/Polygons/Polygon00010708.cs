﻿using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00010708 : Polygon, IPolygonF4C38
    {
        public Polygon00010708(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 28, 4, positionVertices, positionNormals, 20)
        {
        }
    }
}