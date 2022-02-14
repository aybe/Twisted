using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Twisted.Extensions;

namespace Twisted.PS.Texturing
{
    /// <summary>
    ///     Base class for a frame buffer object.
    /// </summary>
    public sealed class FrameBufferObject
    {
        public FrameBufferObject(FrameBufferObjectFormat format, Rectangle rectangle, IReadOnlyList<short> pixels)
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
        /// <seealso cref="RenderSize" />
        public Rectangle Rectangle { get; }

        public IReadOnlyList<short> Pixels { get; }

        /// <summary>
        ///     Gets the render size for this instance (see Remarks).
        /// </summary>
        /// <remarks>
        ///     Use this property to get the actual render size.
        /// </remarks>
        /// <seealso cref="Rectangle" />
        public Size RenderSize
        {
            get
            {
                var width = Rectangle.Width;

                width = Format switch
                {
                    FrameBufferObjectFormat.Indexed4 => width * 4,
                    FrameBufferObjectFormat.Indexed8 => width * 2,
                    FrameBufferObjectFormat.Mixed    => width, // special case, 16-bit page with with mixed content
                    FrameBufferObjectFormat.Direct15 => width,
                    FrameBufferObjectFormat.Direct24 => width * 2 / 3,
                    _                                => throw new InvalidOperationException($"Unknown pixel format: {Format}.")
                };

                return new Size(width, Rectangle.Height);
            }
        }

        public void Blit(FrameBufferObject source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (Pixels is not short[] pixels)
                throw new InvalidOperationException("This instance's pixel data cannot be written to.");

            var x = source.Rectangle.Location.X;
            var y = source.Rectangle.Location.Y;
            var w = source.Rectangle.Size.Width;
            var h = source.Rectangle.Size.Height;

            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < w; j++)
                {
                    pixels[(y + i) * 1024 + x + j] = source.Pixels[i * w + j];
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(Format)}: {Format}, {nameof(Rectangle)}: {Rectangle}, {nameof(Pixels)}: {Pixels.Count}";
        }

        /// <summary>
        ///     Creates an instance the size of the PlayStation video memory.
        /// </summary>
        /// <returns>
        ///     The created frame buffer object.
        /// </returns>
        public static FrameBufferObject CreatePlayStationVideoMemory()
        {
            return new FrameBufferObject(FrameBufferObjectFormat.Direct15, new Rectangle(0, 0, 1024, 512), new short[1024 * 512]);
        }

        public static int GetColorCount(FrameBufferObjectFormat format)
        {
            if (!Enum.IsDefined(typeof(FrameBufferObjectFormat), format))
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(FrameBufferObjectFormat));

            return format switch
            {
                FrameBufferObjectFormat.Indexed4 => 16,
                FrameBufferObjectFormat.Indexed8 => 256,
                FrameBufferObjectFormat.Mixed    => 65536,
                FrameBufferObjectFormat.Direct15 => 65536,
                FrameBufferObjectFormat.Direct24 => 16777216,
                _                                => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        public static void WriteTga(Stream stream, FrameBufferObject picture, FrameBufferObject? palette = null, TransparentColorMode mode = TransparentColorMode.None)
            // http://www.paulbourke.net/dataformats/tga/
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (picture is null)
                throw new ArgumentNullException(nameof(picture));

            // check that picture is a known format

            switch (picture.Format)
            {
                case FrameBufferObjectFormat.Indexed4:
                case FrameBufferObjectFormat.Indexed8:
                case FrameBufferObjectFormat.Direct15:
                case FrameBufferObjectFormat.Direct24:
                case FrameBufferObjectFormat.Mixed:
                    break;
                default:
                    throw new NotSupportedException(picture.Format.ToString());
            }

            // check that picture, if indexed, has a palette with correct format and color count

            if (picture.Format is FrameBufferObjectFormat.Indexed4 or FrameBufferObjectFormat.Indexed8)
            {
                if (palette is null)
                {
                    throw new ArgumentNullException(nameof(palette), $"Palette is required for picture with format {picture.Format}.");
                }

                if (palette.Format is not FrameBufferObjectFormat.Direct15)
                {
                    throw new ArgumentOutOfRangeException(nameof(palette), $"Palette format is not {FrameBufferObjectFormat.Direct15}.");
                }

                var colors = 1 << (picture.Format == FrameBufferObjectFormat.Indexed4 ? 4 : 8);

                if (palette.RenderSize.Width != colors)
                {
                    throw new ArgumentOutOfRangeException(nameof(palette), $"Palette color must be exactly {colors}.");
                }
            }

            // check if TGA really needs transparency, if it does then we'll go 32-bit otherwise we'll go 16-bit as it's supported

            var transparencyColor = mode.HasFlagFast(TransparentColorMode.Color);
            var transparencyBlack = mode.HasFlagFast(TransparentColorMode.Black);
            var transparencyFinal = transparencyColor || transparencyBlack;

            if (transparencyFinal)
            {
                var pixels = picture.Format switch
                {
                    FrameBufferObjectFormat.Indexed4 => palette?.Pixels ?? throw new ArgumentNullException(nameof(palette)),
                    FrameBufferObjectFormat.Indexed8 => palette?.Pixels ?? throw new ArgumentNullException(nameof(palette)),
                    FrameBufferObjectFormat.Direct15 => picture.Pixels,
                    FrameBufferObjectFormat.Mixed    => picture.Pixels,
                    FrameBufferObjectFormat.Direct24 => null,
                    _                                => throw new NotSupportedException(picture.Format.ToString())
                };

                transparencyFinal = pixels?.Any(s => new TransparentColor(s).A) ?? false;

                transparencyFinal |= transparencyBlack;
            }

            // TGA file header

            const byte idLength = 0;

            var colorMapType = (byte)(palette is null ? 0 : 1);

            var imageType = picture.Format switch
            {
                FrameBufferObjectFormat.Indexed4 => (byte)1,
                FrameBufferObjectFormat.Indexed8 => (byte)1,
                FrameBufferObjectFormat.Mixed    => (byte)2,
                FrameBufferObjectFormat.Direct15 => (byte)2,
                FrameBufferObjectFormat.Direct24 => (byte)2,
                _                                => throw new NotSupportedException(picture.Format.ToString())
            };

            const short firstEntryIndex = 0;

            var colorMapLength = (short)(palette is null ? 0 : palette.RenderSize.Width);

            var colorMapEntrySize = (byte)(palette is null ? 0 : transparencyFinal ? 32 : 16);

            const short xOrigin = 0;
            const short yOrigin = 0;

            var imageWidth  = (short)picture.RenderSize.Width;
            var imageHeight = (short)picture.RenderSize.Height;

            var pixelDepth = picture.Format switch
            {
                FrameBufferObjectFormat.Indexed4 => (byte)8,
                FrameBufferObjectFormat.Indexed8 => (byte)8,
                FrameBufferObjectFormat.Mixed    => (byte)(transparencyFinal ? 32 : 16),
                FrameBufferObjectFormat.Direct15 => (byte)(transparencyFinal ? 32 : 16),
                FrameBufferObjectFormat.Direct24 => (byte)24,
                _                                => throw new NotSupportedException(picture.Format.ToString())
            };

            var imageDescriptor = (byte)0b00100000; // top/left

            if (picture is { Format: not FrameBufferObjectFormat.Direct24 })
            {
                if (transparencyFinal)
                {
                    imageDescriptor |= 0b00000011; // alpha channel
                }
            }

            using var writer = new BinaryWriter(stream, Encoding.Default, true);

            writer.Write(idLength);
            writer.Write(colorMapType);
            writer.Write(imageType);
            writer.Write(firstEntryIndex, Endianness.LE);
            writer.Write(colorMapLength,  Endianness.LE);
            writer.Write(colorMapEntrySize);
            writer.Write(xOrigin,     Endianness.LE);
            writer.Write(yOrigin,     Endianness.LE);
            writer.Write(imageWidth,  Endianness.LE);
            writer.Write(imageHeight, Endianness.LE);
            writer.Write(pixelDepth);
            writer.Write(imageDescriptor);

            // Color map data

            if (palette is not null)
            {
                if (transparencyFinal)
                {
                    foreach (var p in palette.Pixels)
                    {
                        var color1 = new TransparentColor(p);
                        var color2 = color1.ToColor(mode);
                        var color3 = color2.ToArgb();
                        writer.Write(color3);
                    }
                }
                else
                {
                    foreach (var p in palette.Pixels) // R5G5B5A1 -> B5G5R5A1
                    {
                        writer.Write((short)((p & 0x83E0) | ((p >> 10) & 0x1F) | (p << 10)), Endianness.LE);
                    }
                }
            }

            // Image data

            switch (picture.Format)
            {
                case FrameBufferObjectFormat.Indexed4:
                    foreach (var p in picture.Pixels)
                    {
                        writer.Write((byte)((p >> 00) & 0xF));
                        writer.Write((byte)((p >> 04) & 0xF));
                        writer.Write((byte)((p >> 08) & 0xF));
                        writer.Write((byte)((p >> 12) & 0xF));
                    }
                    break;
                case FrameBufferObjectFormat.Indexed8:
                    foreach (var p in picture.Pixels)
                    {
                        writer.Write(p, Endianness.LE);
                    }
                    break;
                case FrameBufferObjectFormat.Direct15:
                case FrameBufferObjectFormat.Mixed:
                    if (transparencyFinal)
                    {
                        foreach (var p in picture.Pixels)
                        {
                            var color1 = new TransparentColor(p);
                            var color2 = color1.ToColor(mode);
                            var color3 = color2.ToArgb();
                            writer.Write(color3, Endianness.LE);
                        }
                    }
                    else
                    {
                        foreach (var p in picture.Pixels) // R5G5B5A1 -> B5G5R5A1
                        {
                            writer.Write((short)((p & 0x83E0) | ((p >> 10) & 0x1F) | (p << 10)), Endianness.LE);
                        }
                    }
                    break;
                case FrameBufferObjectFormat.Direct24:
                    for (var i = 0; i < picture.Pixels.Count; i += 3) // R8G8B8 -> B8G8R8
                    {
                        var p1 = picture.Pixels[i + 0];
                        var p2 = picture.Pixels[i + 1];
                        var p3 = picture.Pixels[i + 2];
                        writer.Write((byte)((p2 >> 0) & 0xFF));
                        writer.Write((byte)((p1 >> 8) & 0xFF));
                        writer.Write((byte)((p1 >> 0) & 0xFF));
                        writer.Write((byte)((p3 >> 8) & 0xFF));
                        writer.Write((byte)((p3 >> 0) & 0xFF));
                        writer.Write((byte)((p2 >> 8) & 0xFF));
                    }
                    break;
                default:
                    throw new NotSupportedException(picture.Format.ToString());
            }
        }
    }
}