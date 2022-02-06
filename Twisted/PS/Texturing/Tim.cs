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
    {
        public Tim(Stream stream)
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

            PixelMode = (TimPixelMode)(flags & 0b111);

            if ((flags & 0b1000) != default)
            {
                Palette = new TimPalette(reader);
            }

            PixelData = new TimPixelData(reader, PixelMode);

            Assert.AreEqual(stream.Length, stream.Position, "TIM file not fully read.");
        }

        public TimPalette? Palette { get; }

        public TimPixelMode PixelMode { get; }

        public TimPixelData PixelData { get; }

        public static (Point position, Size size, IReadOnlyList<byte> data) ReadFrameBufferObject(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var s = reader.ReadUInt32(Endianness.LE);
            var x = reader.ReadUInt16(Endianness.LE);
            var y = reader.ReadUInt16(Endianness.LE);
            var w = reader.ReadUInt16(Endianness.LE);
            var h = reader.ReadUInt16(Endianness.LE);

            var position = new Point(x, y);
            var size     = new Size(w, h);
            var data     = new ReadOnlyCollection<byte>(reader.ReadBytes((int)(s - 12)));

            return (position, size, data);
        }
    }
}