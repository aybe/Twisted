using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Twisted.Extensions;

namespace Twisted.PS.Texturing
{
    /// <summary>
    ///     Base class for a frame buffer object.
    /// </summary>
    public sealed class FrameBufferObject
    {
        public FrameBufferObject(FrameBufferObjectFormat format, Rectangle rectangle, IReadOnlyList<byte> pixels)
        {
            Format    = format;
            Rectangle = rectangle;
            Pixels    = pixels;

            var width = Rectangle.Width;

            width = Format switch
            {
                FrameBufferObjectFormat.Indexed4 => width * 4,
                FrameBufferObjectFormat.Indexed8 => width * 2,
                FrameBufferObjectFormat.Direct15 => width,
                FrameBufferObjectFormat.Direct24 => width * 2 / 3,
                FrameBufferObjectFormat.Mixed    => width, // special case, leave alone
                _                                => throw new InvalidOperationException($"Unknown format: {Format}.")
            };

            RenderSize = new Size(width, Rectangle.Height);
        }

        /// <summary>
        ///     Gets the pixel format for this instance.
        /// </summary>
        /// <seealso cref="Rectangle" />
        /// <seealso cref="RenderSize" />
        public FrameBufferObjectFormat Format { get; }

        /// <summary>
        ///     Gets the rectangle for this instance (see Remarks).
        /// </summary>
        /// <remarks>
        ///     The horizontal axis is expressed as 16-bit units; depending <see cref="Format" />, the value may differ from actual render size.
        /// </remarks>
        /// <seealso cref="RenderSize" />
        public Rectangle Rectangle { get; }

        public IReadOnlyList<byte> Pixels { get; }

        /// <summary>
        ///     Gets the render size for this instance (see Remarks).
        /// </summary>
        /// <remarks>
        ///     This property returns the render size in pixels for this instance.
        /// </remarks>
        /// <seealso cref="Rectangle" />
        public Size RenderSize { get; }

        /// <summary>
        ///     Creates an instance the size of the PlayStation video memory.
        /// </summary>
        /// <returns>
        ///     The created frame buffer object.
        /// </returns>
        public static FrameBufferObject CreatePlayStationVideoMemory()
        {
            return new FrameBufferObject(FrameBufferObjectFormat.Direct15, new Rectangle(0, 0, 1024, 512), new byte[1024 * 512 * 2]);
        }

        public override string ToString()
        {
            return $"{nameof(Format)}: {Format}, {nameof(Pixels)}: {Pixels.Count}, {nameof(Rectangle)}: {Rectangle}";
        }

        public void Blit(FrameBufferObject source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (Pixels is not byte[] pixels)
                throw new InvalidOperationException("This instance's pixel data cannot be written to.");

            var x = source.Rectangle.Location.X;
            var y = source.Rectangle.Location.Y;
            var w = source.Rectangle.Size.Width;
            var h = source.Rectangle.Size.Height;
            var p = source.Format switch
            {
                FrameBufferObjectFormat.Indexed4 => w / 2,
                FrameBufferObjectFormat.Indexed8 => w,
                FrameBufferObjectFormat.Direct15 => w * 2,
                FrameBufferObjectFormat.Direct24 => throw new ArgumentOutOfRangeException(),
                FrameBufferObjectFormat.Mixed    => throw new ArgumentOutOfRangeException(),
                _                                => throw new ArgumentOutOfRangeException()
            };

            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < p; j++)
                {
                    pixels[(y + i) * 2048 + x * 2 + j] = source.Pixels[i * p + j];
                }
            }
        }

        public Color[] ToColorArray(Rectangle rectangle, bool translucency)
        {
            var x = rectangle.Location.X;
            var y = rectangle.Location.Y;
            var w = rectangle.Size.Width;
            var h = rectangle.Size.Height;

            var pixels = Pixels as byte[] ?? Pixels.ToArray();
            var colors = new Color[w * h];

            for (var i = 0; i < w * h; i++)
            {
                var u = i % w;
                var v = i / w;
                var p = pixels.ReadUInt16((y + v) * 2048 + (x + u) * 2, Endianness.LE);
                var c = new TimColor(p);
                var d = c.ToColor(translucency);
                colors[v * w + u] = d;
            }

            return colors;
        }

        public static void WriteTga(FrameBufferObject source, Stream destination, FrameBufferObjectFormat format) // TODO this can benefit any object actually
        {
            throw new NotImplementedException();

            var hasColorMap = format switch
            {
                FrameBufferObjectFormat.Indexed4 => true,
                FrameBufferObjectFormat.Indexed8 => true,
                FrameBufferObjectFormat.Direct15 => false,
                FrameBufferObjectFormat.Direct24 => false,
                _                                => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };

            var colorMapEntries = format switch
            {
                FrameBufferObjectFormat.Indexed4 => 16,
                FrameBufferObjectFormat.Indexed8 => 256,
                FrameBufferObjectFormat.Direct15 => 0,
                FrameBufferObjectFormat.Direct24 => 0,
                _                                => throw new ArgumentOutOfRangeException(nameof(format), format, null)
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

            var pixelDepth = format switch
            {
                FrameBufferObjectFormat.Indexed4 => 8,
                FrameBufferObjectFormat.Indexed8 => 8,
                FrameBufferObjectFormat.Direct15 => 16,
                FrameBufferObjectFormat.Direct24 => 24,
                _                                => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };

            writer.Write((ushort)0,                            Endianness.LE); // X-origin of image
            writer.Write((ushort)0,                            Endianness.LE); // Y-origin of image
            writer.Write((ushort)source.Rectangle.Size.Width,  Endianness.LE); // Image width
            writer.Write((ushort)source.Rectangle.Size.Height, Endianness.LE); // Image height
            writer.Write((byte)pixelDepth,                     Endianness.LE); // Pixel depth


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