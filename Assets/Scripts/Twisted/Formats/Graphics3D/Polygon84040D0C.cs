﻿using System.IO;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon84040D0C : PolygonGT4
        // TODO unknown int32 @ 12
    {
        public Polygon84040D0C(BinaryReader reader, int positionVertices)
            : base(reader, 52, 4, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 4;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TextureElementsPosition { get; } = 32;

        protected override int? TextureWindowPosition { get; } = 48;
    }
}