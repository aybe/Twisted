﻿using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon00030806 : PolygonG3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 28
    {
        public Polygon00030806(BinaryReader reader, int positionVertices)
            : base(reader, 32, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 3;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x30;
    }
}