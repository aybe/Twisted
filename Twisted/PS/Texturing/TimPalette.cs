using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Twisted.PS.Texturing
{
    public sealed class TimPalette : IFrameBufferObject
    {
        public TimPalette(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var (position, size, data) = Tim.ReadFrameBufferObject(reader);

            Position = position;
            Size     = size;
            Data     = data;
        }

        public Point Position { get; }

        public Size Size { get; }

        public IReadOnlyList<byte> Data { get; }
    }
}