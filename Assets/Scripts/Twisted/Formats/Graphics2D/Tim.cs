using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Unity.Extensions.Binary;
using RectInt = UnityEngine.RectInt;

namespace Twisted.Formats.Graphics2D
{
    [PublicAPI]
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

            var format = flags & 0b111;

            Format = format switch
            {
                0 => FrameBufferFormat.Indexed4,
                1 => FrameBufferFormat.Indexed8,
                2 => FrameBufferFormat.Direct15,
                3 => FrameBufferFormat.Direct24,
                4 => FrameBufferFormat.Direct15,
                _ => throw new InvalidDataException($"Invalid pixel format: 0x{format}.")
            };

            if ((flags & 0b1000) != default)
            {
                if (TryReadBlock(reader, out var palBlock))
                {
                    var rect = palBlock.Rect;

                    var colors = Format switch
                    {
                        FrameBufferFormat.Indexed4 => 16,
                        FrameBufferFormat.Indexed8 => 256,
                        FrameBufferFormat.Direct15 => throw new InvalidDataException($"Invalid palette block found for pixel format: {Format}."),
                        FrameBufferFormat.Direct24 => throw new InvalidDataException($"Invalid palette block found for pixel format: {Format}."),
                        _                          => throw new InvalidDataException($"Invalid palette block found for pixel format: {Format}.")
                    };

                    var columns = rect.width / colors;

                    var palettes = new FrameBuffer[columns * rect.height];

                    for (var i = 0; i < palettes.Length; i++)
                    {
                        var palRect = new RectInt(rect.x + i % columns * colors, rect.y + i / columns, colors, 1);
                        var palData = palBlock.Data.AsSpan(i * palRect.width, colors).ToArray();

                        palettes[i] = new FrameBuffer(FrameBufferFormat.Direct15, palRect, palData);
                    }

                    Palettes = new ReadOnlyCollection<FrameBuffer>(palettes);
                }
            }

            if (!TryReadBlock(reader, out var picBlock))
                return;

            var picRect = picBlock.Rect;

            var picData = picBlock.Data.AsSpan(0, picRect.width * picRect.height).ToArray();

            Picture = new FrameBuffer(Format, picRect, picData);
        }

        public FrameBufferFormat Format { get; }

        public IReadOnlyList<FrameBuffer>? Palettes { get; }

        public FrameBuffer? Picture { get; }

        private static bool TryReadBlock(BinaryReader reader, out (int Size, short[] Data, RectInt Rect) result)
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

            result = (n, p, new RectInt(x, y, w, h));

            for (var i = 0; i < result.Size - 12 - p.Length * sizeof(short); i++)
            {
                reader.BaseStream.Position++; // skip potential junk after payload
            }

            return true;
        }
    }
}