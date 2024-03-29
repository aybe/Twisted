﻿using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon04040D0C : PolygonGT4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 48
    {
        public Polygon04040D0C(BinaryReader reader, int positionVertices)
            : base(reader, 52, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TextureElementsPosition { get; } = 32;
    }
}