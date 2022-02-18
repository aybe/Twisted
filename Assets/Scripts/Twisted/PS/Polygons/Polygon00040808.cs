﻿using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon00040808 : Polygon, IPolygonG4C38
    {
        public Polygon00040808(BinaryReader reader, int positionVertices)
            : base(reader, 32, 4, positionVertices)
        {
            // TODO four colors at end?
        }
    }
}