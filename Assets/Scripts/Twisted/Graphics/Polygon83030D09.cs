﻿using System.IO;

namespace Twisted.Graphics
{
    internal sealed class Polygon83030D09 : PolygonGT3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 40 ends with E2
        // TODO unknown int32 @ 44
        // TODO unknown int32 @ 48
    {
        public Polygon83030D09(BinaryReader reader, int positionVertices, int positionNormals)
            : base(reader, 52, 3, positionVertices, positionNormals, 44)
        {
        }

        protected override int? ColorElements { get; } = 3;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x34;

        protected override int? TextureElements { get; } = 3;

        protected override int? TexturePosition { get; } = 28;
    }
}