﻿using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon00010505 : PolygonF4
        // TODO unknown int32 @ 12
    {
        public Polygon00010505(BinaryReader reader, int positionVertices)
            : base(reader, 20, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x28;
    }
}