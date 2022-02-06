using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Twisted.Extensions;

namespace Twisted.PS.Texturing
{
    public sealed class FrameBuffer : IFrameBufferObject // TODO this could be a base class
    {
        private byte[] Data { get; } = new byte[1048576];

        Point IFrameBufferObject.Position { get; } = new(0, 0);

        Size IFrameBufferObject.Size { get; } = new(1024, 512);

        IReadOnlyList<byte> IFrameBufferObject.Data => Data.ToArray();

        public void Blit(IFrameBufferObject o) // TODO this can benefit any object actually BUT only if writable!
        {
            if (o == null)
                throw new ArgumentNullException(nameof(o));

            for (var y = 0; y < o.Size.Height; y++)
            {
                for (var x = 0; x < o.Size.Width; x++)
                {
                    Data[(y + o.Position.Y) * 2048 + (x + o.Position.X) * 2] = o.Data[y * o.Size.Width + x];
                }
            }
        }

        public Color[] ToColor(Point position, Size size, bool translucency) // TODO this can benefit any object actually
        {
            var colors = new Color[size.Width * size.Height];

            for (var y = 0; y < size.Height; y++)
            {
                for (var x = 0; x < size.Width; x++)
                {
                    var u = Data.ReadUInt16((position.Y + y) * 2048 + (position.X + x) * 2, Endianness.LE);
                    var v = new TimColor(u);
                    var w = v.ToColor(translucency);
                    colors[y * size.Width + x] = w;
                }
            }

            return colors;
        }

        public static void WriteTga(IFrameBufferObject source, Stream destination, PixelFormat pixelFormat)
        {
            var hasColorMap = pixelFormat switch
            {
                PixelFormat.Indexed4 => true,
                PixelFormat.Indexed8 => true,
                PixelFormat.Direct15 => false,
                PixelFormat.Direct24 => false,
                _                    => throw new ArgumentOutOfRangeException(nameof(pixelFormat), pixelFormat, null)
            };

            var colorMapEntries = pixelFormat switch
            {
                PixelFormat.Indexed4 => 16,
                PixelFormat.Indexed8 => 256,
                PixelFormat.Direct15 => 0,
                PixelFormat.Direct24 => 0,
                _                    => throw new ArgumentOutOfRangeException(nameof(pixelFormat), pixelFormat, null)
            };

            var writer = new BinaryWriter(new MemoryStream()); // todo

            writer.Write((byte)0);                     // ID length
            writer.Write((byte)(hasColorMap ? 1 : 0)); // Color map type
            writer.Write((byte)(hasColorMap ? 1 : 2)); // Image type

            // Color map specification

            if (hasColorMap)
            {
                writer.Write((ushort)0,               Endianness.LE); // First entry index
                writer.Write((ushort)colorMapEntries, Endianness.LE); // Color map length
                writer.Write((ushort)32,              Endianness.LE); // Color map entry size
            }
            else
            {
                for (var i = 0; i < 5; i++)
                {
                    writer.Write((byte)0);
                }
            }

            // Image specification

            var pixelDepth = pixelFormat switch
            {
                PixelFormat.Indexed4 => 8,
                PixelFormat.Indexed8 => 8,
                PixelFormat.Direct15 => 16,
                PixelFormat.Direct24 => 24,
                _                    => throw new ArgumentOutOfRangeException(nameof(pixelFormat), pixelFormat, null)
            };

            writer.Write((ushort)0,                  Endianness.LE); // X-origin of image
            writer.Write((ushort)0,                  Endianness.LE); // Y-origin of image
            writer.Write((ushort)source.Size.Width,  Endianness.LE); // Image width
            writer.Write((ushort)source.Size.Height, Endianness.LE); // Image height
            writer.Write((byte)pixelDepth,           Endianness.LE); // Pixel depth


            writer.Write((byte)0b00100011, Endianness.LE); // Image descriptor: top-left, alpha present // BUG there should be no alpha for 15/24

            // NOTE: no Image ID - Field 6

            // Color map data

            if (hasColorMap)
            {
                // TODO source needs a palette
            }

            // Image data

            // TODO

            // NOTE: no Developer Data - Field 9

            // writer.Write(Encoding.ASCII.GetBytes("TRUEVISION-XFILE.\0"));
        }
    }
}