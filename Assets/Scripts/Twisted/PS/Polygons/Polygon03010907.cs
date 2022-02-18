﻿using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon03010907 : PolygonFT3
        // TODO unknown int32 @ 12
    {
        public Polygon03010907(BinaryReader reader, int positionVertices)
            : base(reader, 36, 3, positionVertices)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x24;

        protected override int? TextureElements { get; } = 3;

        protected override int? TexturePosition { get; } = 20;
    }
}