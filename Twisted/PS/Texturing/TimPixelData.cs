using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Twisted.PS.Texturing
{
    public sealed class TimPixelData : IFrameBufferObject
    {
        public TimPixelData(BinaryReader reader, TimPixelMode mode)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var (position, size, data) = Tim.ReadFrameBufferObject(reader);

            var width = mode switch
            {
                TimPixelMode.Indexed4 => size.Width * 4,
                TimPixelMode.Indexed8 => size.Width * 8,
                TimPixelMode.Direct15 => size.Width,
                TimPixelMode.Direct24 => size.Width * 2 / 3,
                TimPixelMode.Mixed    => size.Width, // don't change anything for this special case
                _                     => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };

            size = new Size(width, size.Height);

            Position = position;
            Size     = size;
            Data     = data;
        }

        public Point Position { get; }

        public Size Size { get; }

        public IReadOnlyList<byte> Data { get; }
    }
}