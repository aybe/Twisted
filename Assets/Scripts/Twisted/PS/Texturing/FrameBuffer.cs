using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Twisted.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Twisted.PS.Texturing
{
    /// <summary>
    ///     Base class for a frame buffer object.
    /// </summary>
    public sealed class FrameBuffer
    {
        public FrameBuffer(FrameBufferObjectFormat format, Rectangle rectangle, IReadOnlyList<short> pixels)
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
        [Obsolete("Use Rect instead.")]
        public Rectangle Rectangle { get; }

        /// <summary>
        ///     Gets the rectangle for this instance (see Remarks).
        /// </summary>
        /// <remarks>
        ///     The horizontal axis is expressed as 16-bit units.
        /// </remarks>
        /// <seealso cref="RenderSize" />
#pragma warning disable CS0618
        public RectInt Rect => new(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
#pragma warning restore CS0618

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

        public void Blit(FrameBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (Pixels is not short[] pixels)
                throw new InvalidOperationException("This instance's pixel data cannot be written to.");

            var x = buffer.Rectangle.Location.X;
            var y = buffer.Rectangle.Location.Y;
            var w = buffer.Rectangle.Size.Width;
            var h = buffer.Rectangle.Size.Height;

            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < w; j++)
                {
                    pixels[(y + i) * 1024 + x + j] = buffer.Pixels[i * w + j];
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
        public static FrameBuffer CreatePlayStationVideoMemory()
        {
            return new FrameBuffer(FrameBufferObjectFormat.Direct15, new Rectangle(0, 0, 1024, 512), new short[1024 * 512]);
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

        [Obsolete("Use overload")]
        public static TransparentColor[] GetPalette(FrameBuffer buffer, int x, int y, int width)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Format is not FrameBufferObjectFormat.Direct15)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            if (x < 0 || x >= buffer.Rectangle.Width)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0 || y >= buffer.Rectangle.Height)
                throw new ArgumentOutOfRangeException(nameof(y));

            if (width <= 0 || width + x >= buffer.Rectangle.Width)
                throw new ArgumentOutOfRangeException(nameof(width));

            var colors = new TransparentColor[width];

            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = new TransparentColor(buffer.Pixels[y * buffer.Rectangle.Width + x + i]);
            }

            return colors;
        }

        public static void WriteTga(Stream stream, FrameBuffer picture, FrameBuffer? palette = null, TransparentColorMode mode = TransparentColorMode.None)
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

                if (palette.RenderSize.Width != colors) // TGA is strict about that
                {
                    throw new ArgumentOutOfRangeException(nameof(palette), $"Palette color count must be {colors}.");
                }
            }

            // check if TGA really needs transparency, if it does then we'll go 32-bit otherwise we'll go 16-bit as it's supported

            var transparencyColor = mode.HasFlagFast(TransparentColorMode.Color);
            var transparencyBlack = mode.HasFlagFast(TransparentColorMode.Black);
            var transparency      = transparencyColor || transparencyBlack;

            if (transparency)
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

                transparency = pixels?.Any(s => new TransparentColor(s).A) ?? false;

                transparency |= transparencyBlack; // allow this as it's useful
            }

            // TGA file header

            var imageType = picture.Format switch
            {
                FrameBufferObjectFormat.Indexed4 => 1,
                FrameBufferObjectFormat.Indexed8 => 1,
                FrameBufferObjectFormat.Mixed    => 2,
                FrameBufferObjectFormat.Direct15 => 2,
                FrameBufferObjectFormat.Direct24 => 2,
                _                                => throw new NotSupportedException(picture.Format.ToString())
            };

            var pixelDepth = picture.Format switch
            {
                FrameBufferObjectFormat.Indexed4 => 8,
                FrameBufferObjectFormat.Indexed8 => 8,
                FrameBufferObjectFormat.Mixed    => transparency ? 32 : 16,
                FrameBufferObjectFormat.Direct15 => transparency ? 32 : 16,
                FrameBufferObjectFormat.Direct24 => 24,
                _                                => throw new NotSupportedException(picture.Format.ToString())
            };

            var imageDescriptor = 0b00100000; // top/left

            if (picture.Format is not FrameBufferObjectFormat.Direct24 && transparency)
            {
                imageDescriptor |= 0b00000011; // alpha channel
            }

            using var writer = new BinaryWriter(stream, Encoding.Default, true);

            writer.Write((byte)0);                                                                // ID length
            writer.Write((byte)(palette is null ? 0 : 1));                                        // Color map type
            writer.Write((byte)imageType);                                                        //
            writer.Write((short)0, Endianness.LE);                                                // First entry index
            writer.Write((short)(palette is null ? 0 : palette.RenderSize.Width), Endianness.LE); // Color map length
            writer.Write((byte)(palette is null ? 0 : transparency ? 32 : 16));                   // Color map entry size
            writer.Write((short)0, Endianness.LE);                                                // X-origin
            writer.Write((short)0, Endianness.LE);                                                // Y-origin
            writer.Write((short)picture.RenderSize.Width, Endianness.LE);                         // Image width
            writer.Write((short)picture.RenderSize.Height, Endianness.LE);                        // Image height
            writer.Write((byte)pixelDepth);                                                       //
            writer.Write((byte)imageDescriptor);                                                  //

            // Color map data

            if (palette is not null)
            {
                if (transparency) // we must use 32-bit palette entries
                {
                    foreach (var p in palette.Pixels)
                    {
                        var color1 = new TransparentColor(p);
                        var color2 = color1.ToColor(mode);
                        var color3 = color2.ToArgb();
                        writer.Write(color3);
                    }
                }
                else // we can keep 16-bit palette entries
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
                    if (transparency)
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

        /// <summary>
        ///     Writes a frame buffer object instance to a stream.
        /// </summary>
        /// <param name="stream">
        ///     The destination stream.
        /// </param>
        /// <param name="buffer">
        ///     The frame buffer object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="stream" /> or <paramref name="buffer" /> is <c>null</c>.
        /// </exception>
        public static void WriteRaw(Stream stream, FrameBuffer buffer)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            using var writer = new BinaryWriter(stream, Encoding.Default, true);

            foreach (var pixel in buffer.Pixels)
            {
                writer.Write(pixel, Endianness.LE);
            }
        }

        public static Texture2D GetTexture(
            FrameBufferObjectFormat picFormat,
            FrameBuffer             picBuffer,
            RectInt                 picRect,
            RectInt?                palRect   = null,
            FrameBuffer?            palBuffer = null,
            TransparentColorMode    mode      = TransparentColorMode.None)
        {
            if (picBuffer is null)
                throw new ArgumentNullException(nameof(picBuffer));

            if (!Enum.IsDefined(typeof(FrameBufferObjectFormat), picFormat))
                throw new InvalidEnumArgumentException(nameof(picFormat), (int)picFormat, typeof(FrameBufferObjectFormat));

            Texture2D palTex;

            switch (picFormat)
            {
                case FrameBufferObjectFormat.Indexed4:
                case FrameBufferObjectFormat.Indexed8:
                    if (palBuffer is null)
                        throw new ArgumentNullException(nameof(palBuffer));

                    if (palBuffer.Format is not FrameBufferObjectFormat.Direct15)
                        throw new ArgumentOutOfRangeException(nameof(palBuffer));

                    if (palRect is null)
                        throw new ArgumentNullException(nameof(palRect));

                    palTex = GetTexture(palBuffer.Format, palBuffer, palRect.Value, null, null, mode);
                    break;
                case FrameBufferObjectFormat.Mixed:
                case FrameBufferObjectFormat.Direct15:
                case FrameBufferObjectFormat.Direct24:
                    palTex = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(picFormat), picFormat, null);
            }

            var texSize = new Vector2Int(GetWidth(picRect.width, picFormat), picRect.height);
            var texData = new Color32[texSize.x * texSize.y];
            var picPosX = picRect.position.x;
            var picPosY = picRect.position.y;
            var palTint = palTex != null ? palTex.GetPixels32() : Array.Empty<Color32>();

            switch (picFormat)
            {
                case FrameBufferObjectFormat.Indexed4:
                    for (var y = 0; y < texSize.y; y++)
                    {
                        for (var x = 0; x < texSize.x; x++)
                        {
                            var i = (picPosY + y) * picBuffer.Rect.width + picPosX + x / 4;
                            var j = picBuffer.Pixels[i];
                            texData[(texSize.y - 1 - y) * texSize.x + x] = palTint[(j >> (x % 4 * 4)) & 0xF];
                        }
                    }
                    break;
                case FrameBufferObjectFormat.Indexed8:
                    for (var y = 0; y < texSize.y; y++)
                    {
                        for (var x = 0; x < texSize.x; x++)
                        {
                            var i = (picPosY + y) * picBuffer.Rect.width + picPosX + x / 2;
                            var j = picBuffer.Pixels[i];
                            texData[(texSize.y - 1 - y) * texSize.x + x] = palTint[(j >> (x % 2 * 8)) & 0xFF];
                        }
                    }
                    break;
                case FrameBufferObjectFormat.Direct15:
                case FrameBufferObjectFormat.Mixed:

                    for (var y = 0; y < texSize.y; y++)
                    {
                        for (var x = 0; x < texSize.x; x++)
                        {
                            var i = (picPosY + y) * picBuffer.Rect.width + picPosX + x;
                            var j = picBuffer.Pixels[i];
                            texData[(texSize.y - 1 - y) * texSize.x + x] = new TransparentColor(j).ToColor32(mode);
                        }
                    }
                    break;
                case FrameBufferObjectFormat.Direct24:
                    for (var y = 0; y < texSize.y; y++)
                    {
                        for (var x = 0; x < texSize.x; x++)
                        {
                            var i = (picPosY + y) * picBuffer.Rect.width + picPosX + x * 3 / 2;
                            var j = picBuffer.Pixels[i];
                            var k = picBuffer.Pixels[i + 1];

                            byte r, g, b;

                            if (x % 2 == 0)
                            {
                                r = (byte)((j >> 0) & 0xFF);
                                g = (byte)((j >> 8) & 0xFF);
                                b = (byte)((k >> 0) & 0xFF);
                            }
                            else
                            {
                                r = (byte)((j >> 8) & 0xFF);
                                g = (byte)((k >> 0) & 0xFF);
                                b = (byte)((k >> 8) & 0xFF);
                            }

                            texData[(texSize.y - 1 - y) * texSize.x + x] = new Color32(r, g, b, byte.MaxValue);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(picFormat), picFormat, null);
            }

            if (palTex != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(palTex);
                }
                else
                {
                    Object.Destroy(palTex);
                }
            }

            var texture = new Texture2D(texSize.x, texSize.y);

            texture.SetPixels32(texData);
            texture.Apply();

            return texture;
        }

        public static int GetWidth(int width, FrameBufferObjectFormat format) // TODO reuse this method
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            return format switch
            {
                FrameBufferObjectFormat.Indexed4 => width * 4,
                FrameBufferObjectFormat.Indexed8 => width * 2,
                FrameBufferObjectFormat.Mixed    => width,
                FrameBufferObjectFormat.Direct15 => width,
                FrameBufferObjectFormat.Direct24 => width * 2 / 3,
                _                                => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }
}