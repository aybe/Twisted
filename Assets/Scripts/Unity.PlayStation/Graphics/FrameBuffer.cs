using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.PlayStation.Graphics
{
    /// <summary>
    ///     Base class for a frame buffer object.
    /// </summary>
    [PublicAPI]
    public sealed class FrameBuffer
    {
        public FrameBuffer(FrameBufferFormat format, RectInt rect, IReadOnlyList<short> pixels)
        {
            Format = format;
            Rect   = rect;
            Pixels = pixels;
        }

        /// <summary>
        ///     Gets the pixel format for this instance.
        /// </summary>
        /// <seealso cref="Rect" />
        public FrameBufferFormat Format { get; }

        /// <summary>
        ///     Gets the rectangle for this instance (see Remarks).
        /// </summary>
        /// <remarks>
        ///     The horizontal axis is expressed as 16-bit units.
        /// </remarks>
        public RectInt Rect { get; }

        public IReadOnlyList<short> Pixels { get; }

        public void Blit(FrameBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (Pixels is not short[] pixels)
                throw new InvalidOperationException("This instance's pixel data cannot be written to.");

            var x = buffer.Rect.position.x;
            var y = buffer.Rect.position.y;
            var w = buffer.Rect.size.x;
            var h = buffer.Rect.size.y;

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
            return $"{nameof(Format)}: {Format}, {nameof(Rect)}: {Rect}, {nameof(Pixels)}: {Pixels.Count}";
        }

        /// <summary>
        ///     Creates an instance the size of the PlayStation video memory.
        /// </summary>
        /// <returns>
        ///     The created frame buffer object.
        /// </returns>
        public static FrameBuffer CreatePlayStationVideoMemory()
        {
            return new FrameBuffer(FrameBufferFormat.Direct15, new RectInt(0, 0, 1024, 512), new short[1024 * 512]);
        }

        public static int GetColorCount(FrameBufferFormat format)
        {
            if (!Enum.IsDefined(typeof(FrameBufferFormat), format))
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(FrameBufferFormat));

            return format switch
            {
                FrameBufferFormat.Indexed4 => 16,
                FrameBufferFormat.Indexed8 => 256,
                FrameBufferFormat.Direct15 => 65536,
                FrameBufferFormat.Direct24 => 16777216,
                _                          => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        public static Texture2D GetTexture(
            FrameBufferFormat    picFormat,
            FrameBuffer          picBuffer,
            RectInt              picRect,
            RectInt?             palRect   = null,
            FrameBuffer?         palBuffer = null,
            TransparentColorMode mode      = TransparentColorMode.None)
        {
            if (picBuffer is null)
                throw new ArgumentNullException(nameof(picBuffer));

            if (!Enum.IsDefined(typeof(FrameBufferFormat), picFormat))
                throw new InvalidEnumArgumentException(nameof(picFormat), (int)picFormat, typeof(FrameBufferFormat));

            Texture2D palTex;

            switch (picFormat)
            {
                case FrameBufferFormat.Indexed4:
                case FrameBufferFormat.Indexed8:
                    if (palBuffer is null)
                        throw new ArgumentNullException(nameof(palBuffer));

                    if (palBuffer.Format is not FrameBufferFormat.Direct15)
                        throw new ArgumentOutOfRangeException(nameof(palBuffer));

                    if (palRect is null)
                        throw new ArgumentNullException(nameof(palRect));

                    palTex = GetTexture(palBuffer.Format, palBuffer, palRect.Value, null, null, mode);
                    break;
                case FrameBufferFormat.Direct15:
                case FrameBufferFormat.Direct24:
                    palTex = null!;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(picFormat), picFormat, null);
            }

            var texSize = new Vector2Int(GetRenderWidth(picRect.width, picFormat), picRect.height);
            var texData = new Color32[texSize.x * texSize.y];
            var picPosX = picRect.position.x;
            var picPosY = picRect.position.y;
            var palTint = palTex != null ? palTex.GetPixels32() : Array.Empty<Color32>();

            switch (picFormat)
            {
                case FrameBufferFormat.Indexed4:
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
                case FrameBufferFormat.Indexed8:
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
                case FrameBufferFormat.Direct15:
                    for (var y = 0; y < texSize.y; y++)
                    {
                        for (var x = 0; x < texSize.x; x++)
                        {
                            var i = (picPosY + y) * picBuffer.Rect.width + picPosX + x;
                            var j = picBuffer.Pixels[i];
                            texData[(texSize.y - 1 - y) * texSize.x + x] = new TransparentColor(j).ToColor(mode);
                        }
                    }
                    break;
                case FrameBufferFormat.Direct24:
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

        public static int GetRenderWidth(int width, FrameBufferFormat format)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            return format switch
            {
                FrameBufferFormat.Indexed4 => width * 4,
                FrameBufferFormat.Indexed8 => width * 2,
                FrameBufferFormat.Direct15 => width,
                FrameBufferFormat.Direct24 => width * 2 / 3,
                _                          => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }
}