using System;
using System.IO;
using System.Text;
using Twisted.Extensions;

namespace Twisted.PS.Texturing
{
    public sealed class Tim
        // NOTE: this can't be a FrameBufferObject itself, but the picture and palette are
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

            Format = (FrameBufferObjectFormat)(flags & 0b111);

            if ((flags & 0b1000) != default)
            {
                Palette = new FrameBufferObject(reader, FrameBufferObjectFormat.Direct15);
            }

            Picture = new FrameBufferObject(reader, Format, true);

            Assert.AreEqual(stream.Length, stream.Position, "TIM file was not fully read.");
        }

        public FrameBufferObjectFormat Format { get; }

        public FrameBufferObject? Palette { get; }

        public FrameBufferObject Picture { get; }
    }
}