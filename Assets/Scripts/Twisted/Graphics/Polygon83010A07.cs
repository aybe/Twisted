﻿using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon83010A07 : PolygonFT3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 32 with E2
        // TODO unknown int32 @ 36
    {
        public Polygon83010A07(BinaryReader reader, int positionVertices)
            : base(reader, 40, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x24;

        protected override int? TextureElements { get; } = 3;

        protected override int? TexturePosition { get; } = 20;
    }
}