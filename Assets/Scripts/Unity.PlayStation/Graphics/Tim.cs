using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using Unity.Extensions.Binary;
using Unity.Extensions.General;
using UnityEngine;

namespace Unity.PlayStation.Graphics
{
    public sealed class Tim
        // all existing scenarios covered: with or without palettes and/or picture (tested on +10K files)
    {
        public Tim(Stream stream)
            // applying the FrameBuffer paradigm everywhere possible
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

            Format = (FrameBufferFormat)(flags & 0b111);

            if ((flags & 0b1000) != default)
            {
                if (TryReadBlock(reader, out var palBlock))
                {
                    var rect = palBlock.Rect;

                    var colors   = FrameBuffer.GetColorCount(Format);
                    var columns  = rect.width / colors;
                    var palettes = new FrameBuffer[columns * rect.height];

                    for (var i = 0; i < palettes.Length; i++)
                    {
                        var palRect = new RectInt(rect.x + i % columns * colors, rect.y + i / columns, colors, 1);
                        var palData = palBlock.Pixels.AsSpan(i * palRect.width, colors).ToArray();

                        palettes[i] = new FrameBuffer(FrameBufferFormat.Direct15, palRect, palData);
                    }

                    Palettes = new ReadOnlyCollection<FrameBuffer>(palettes);
                }
            }

            if (!TryReadBlock(reader, out var picBlock))
                return;

            var picRect = picBlock.Rect;

            var picData = picBlock.Pixels.AsSpan(0, picRect.width * picRect.height).ToArray();

            Picture = new FrameBuffer(Format, picRect, picData);
        }

        public FrameBufferFormat Format { get; }

        public IReadOnlyList<FrameBuffer>? Palettes { get; }

        public FrameBuffer? Picture { get; }

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

            if (!reader.TryRead(s => s.ReadInt16(Endianness.LE), out var p, (n - 12) / 2))
                return false;

            result = new TimBlock(n, p, new RectInt(x, y, w, h));

            return true;
        }

        private sealed class TimBlock
        {
            public TimBlock(int length, short[] pixels, RectInt rect)
            {
                Length    = length;
                Pixels    = pixels;
                Rect = rect;
            }

            public int Length { get; }

            public RectInt Rect { get; }

            public short[] Pixels { get; }
        }
    }
}