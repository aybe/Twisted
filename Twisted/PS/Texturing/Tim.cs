using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using Twisted.Extensions;

namespace Twisted.PS.Texturing
{
    public sealed class Tim
        // all existing scenarios covered: with or without palettes and/or picture (tested on +10K files)
    {
        public Tim(Stream stream)
            // applying the FrameBufferObject paradigm everywhere possible
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using var reader = new BinaryReader(stream, Encoding.Default, true);

            var identifier = reader.ReadInt32(Endianness.LE);

            if (identifier != 0x00000010)
            {
                throw new InvalidDataException($"Invalid identifier: 0x{identifier:X8}.");
            }

            var flags = reader.ReadInt32(Endianness.LE);

            Format = (FrameBufferObjectFormat)(flags & 0b111);

            if ((flags & 0b1000) != default)
            {
                if (TryReadBlock(reader, out var palBlock))
                {
                    var rect = palBlock.Rectangle;

                    var palettes = new FrameBufferObject[rect.Height];

                    for (var i = 0; i < palettes.Length; i++)
                    {
                        const FrameBufferObjectFormat format = FrameBufferObjectFormat.Direct15;

                        var palRect = new Rectangle(rect.X, rect.Y + i, rect.Width, 1);

                        var palData = palBlock.Pixels.AsSpan(0, rect.Width * 2).ToArray();

                        palettes[i] = new FrameBufferObject(format, palRect, palData);
                    }

                    Palettes = new ReadOnlyCollection<FrameBufferObject>(palettes);
                }
            }

            if (!TryReadBlock(reader, out var picBlock))
                return;

            var picRect = picBlock.Rectangle;

            var picData = picBlock.Pixels.AsSpan(picRect.Width * 2 * picRect.Height).ToArray();

            Picture = new FrameBufferObject(Format, picRect, picData);
        }

        public FrameBufferObjectFormat Format { get; }

        public IReadOnlyList<FrameBufferObject>? Palettes { get; }

        public FrameBufferObject? Picture { get; }

        private static bool TryReadBlock(BinaryReader reader, out TimBlock result)
            // some geniuses out there thought it'd be nice to have incomplete blocks
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            result = default!;

            if (!reader.TryRead(s => s.ReadInt32(Endianness.LE), out var n))
                return false;

            if (!reader.TryRead(s => s.ReadInt16(Endianness.LE), out var x))
                return false;

            if (!reader.TryRead(s => s.ReadInt16(Endianness.LE), out var y))
                return false;

            if (!reader.TryRead(s => s.ReadInt16(Endianness.LE), out var w))
                return false;

            if (!reader.TryRead(s => s.ReadInt16(Endianness.LE), out var h))
                return false;

            if (!reader.TryRead(s => s.ReadBytes(n - 12), out var p))
                return false;

            result = new TimBlock(n, p, new Rectangle(x, y, w, h));

            return true;
        }

        private sealed class TimBlock
        {
            public TimBlock(int length, byte[] pixels, Rectangle rectangle)
            {
                Length    = length;
                Pixels    = pixels;
                Rectangle = rectangle;
            }

            public int Length { get; }

            public Rectangle Rectangle { get; }

            public byte[] Pixels { get; }
        }
    }
}