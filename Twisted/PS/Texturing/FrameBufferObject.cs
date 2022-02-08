using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
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
        }

        /// <summary>
        ///     Gets the pixel format for this instance.
        /// </summary>
        /// <seealso cref="Rectangle" />
        public FrameBufferObjectFormat Format { get; }

        /// <summary>
        ///     Gets the rectangle for this instance (see Remarks).
        /// </summary>
        /// <remarks>
        ///     The horizontal axis is expressed as 16-bit units.
        /// </remarks>
        public Rectangle Rectangle { get; }

        public IReadOnlyList<byte> Pixels { get; }

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

            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < w * 2; j++)
                {
                    pixels[(y + i) * 2048 + x * 2 + j] = source.Pixels[i * w * 2 + j];
                }
            }
        }

        /// <summary>
        ///     Gets the actual width for this instance.
        /// </summary>
        /// <returns>
        ///     The actual width according <see cref="Format" />.
        /// </returns>
        public int GetWidth()
        {
            var width = Rectangle.Width;

            return Format switch
            {
                FrameBufferObjectFormat.Indexed4 => width * 4,
                FrameBufferObjectFormat.Indexed8 => width * 2,
                FrameBufferObjectFormat.Direct15 => width,
                FrameBufferObjectFormat.Direct24 => width * 2 / 3,
                FrameBufferObjectFormat.Mixed    => width, // special case
                _                                => throw new InvalidOperationException($"Unknown format: {Format}.")
            };
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

        [SupportedOSPlatform("windows")]
        public Bitmap ToBitmap(FrameBufferObject? paletteObject, bool translucency)
        {
            var bitmap = new Bitmap(
                GetWidth(),
                Rectangle.Height,
                Format switch
                {
                    FrameBufferObjectFormat.Indexed4 => PixelFormat.Format4bppIndexed,
                    FrameBufferObjectFormat.Indexed8 => PixelFormat.Format8bppIndexed,
                    FrameBufferObjectFormat.Direct15 => PixelFormat.Format16bppRgb555,
                    FrameBufferObjectFormat.Direct24 => PixelFormat.Format24bppRgb,
                    FrameBufferObjectFormat.Mixed    => throw new NotSupportedException(),
                    _                                => throw new ArgumentOutOfRangeException()
                }
            );
            
            var pixels = Pixels as byte[] ?? Pixels.ToArray();

            var data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            
            bitmap.UnlockBits(data);

            if (Format is FrameBufferObjectFormat.Indexed4 or FrameBufferObjectFormat.Indexed8)
            {
                if (paletteObject != null)
                {
                    var palette     = bitmap.Palette;
                    var paletteData = paletteObject.Pixels as byte[] ?? paletteObject.Pixels.ToArray();
                    var paletteSize = paletteObject.GetWidth();

                    for (var i = 0; i < paletteSize; i++)
                    {
                        var u = paletteData.ReadUInt16(i * 2, Endianness.LE);
                        var v = new TimColor(u);
                        var w = v.ToColor(translucency);
                        palette.Entries[i] = w;
                    }

                    bitmap.Palette = palette;
                }
            }

            return bitmap;

            switch (Format)
            {
                case FrameBufferObjectFormat.Indexed4:
                    for (var y = 0; y < Rectangle.Height; y++)
                    {
                        for (var x = 0; x < Rectangle.Width; x++)
                        {
                            for (int p = 0; p < 4; p++)
                            {
                                var i  = y * Rectangle.Width * 2 + x * 2 /*+ p / 2*/;
                                var b  = this.Pixels[i];
                                var u  = pixels.ReadInt16(i, Endianness.LE);
                                var v = (u >> p) & 0b1111;
                                // 0 >> 0
                                // 1 >> 4
                                // 2 >> 0
                                // 3 >> 4
                            }
                        }
                    }

                    break;
                case FrameBufferObjectFormat.Indexed8:
                    break;
                case FrameBufferObjectFormat.Direct15:
                    break;
                case FrameBufferObjectFormat.Direct24:
                    break;
                case FrameBufferObjectFormat.Mixed:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return bitmap;
        }
        public override string ToString()
        {
            return $"{nameof(Format)}: {Format}, {nameof(Rectangle)}: {Rectangle}, {nameof(Pixels)}: {Pixels.Count}";
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
    }
}