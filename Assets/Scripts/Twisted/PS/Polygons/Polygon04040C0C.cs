﻿using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon04040C0C : Polygon, IPolygonG4C3E
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 32
        // TODO unknown int32 @ 36
        // TODO unknown int32 @ 40
        // TODO unknown int32 @ 44
    {
        public Polygon04040C0C(BinaryReader reader, int positionVertices)
            : base(reader, 48, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3E;
    }
}