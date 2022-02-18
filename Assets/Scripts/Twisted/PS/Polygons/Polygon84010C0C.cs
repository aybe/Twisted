﻿using System.IO;

namespace Twisted.PS.Polygons
{
    internal sealed class Polygon84010C0C : PolygonFT4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 36 with E2
        // TODO unknown int32 @ 40
        // TODO unknown int32 @ 44
    {
        public Polygon84010C0C(BinaryReader reader, int vertices, int positionNormals)
            : base(reader, 48, 4, vertices, positionNormals, 40)
        {
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 20;
    }
}