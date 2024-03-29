﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Twisted.Formats.Graphics2D
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

        public static Texture2D GetTexture(
            FrameBufferFormat picFormat,
            FrameBuffer picBuffer,
            RectInt picRect,
            FrameBuffer? palBuffer = null,
            RectInt? palRect = null,
            TransparentColorMode mode = TransparentColorMode.None)
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

            var picWidth = picFormat switch
            {
                FrameBufferFormat.Indexed4 => picRect.width * 4,
                FrameBufferFormat.Indexed8 => picRect.width * 2,
                FrameBufferFormat.Direct15 => picRect.width,
                FrameBufferFormat.Direct24 => picRect.width * 2 / 3,
                _                          => throw new ArgumentOutOfRangeException(nameof(picFormat), picFormat, null)
            };

            var texSize = new Vector2Int(picWidth, picRect.height);

            var xSize  = texSize.x;            // do not inline!
            var ySize  = texSize.y;            // do not inline!
            var width  = picBuffer.Rect.width; // do not inline!
            var pixels = picBuffer.Pixels;     // do not inline!

            var texData = new Color32[xSize * ySize];
            var picPosX = picRect.position.x;
            var picPosY = picRect.position.y;
            var palTint = palTex != null ? palTex.GetPixels32() : Array.Empty<Color32>();

            switch (picFormat)
            {
                case FrameBufferFormat.Indexed4:
                    for (var y = 0; y < ySize; y++)
                    {
                        for (var x = 0; x < xSize; x++)
                        {
                            var i = (picPosY + y) * width + picPosX + x / 4;
                            var j = pixels[i];
                            texData[(ySize - 1 - y) * xSize + x] = palTint[(j >> (x % 4 * 4)) & 0xF];
                        }
                    }
                    break;
                case FrameBufferFormat.Indexed8:
                    for (var y = 0; y < ySize; y++)
                    {
                        for (var x = 0; x < xSize; x++)
                        {
                            var i = (picPosY + y) * width + picPosX + x / 2;
                            var j = pixels[i];
                            texData[(ySize - 1 - y) * xSize + x] = palTint[(j >> (x % 2 * 8)) & 0xFF];
                        }
                    }
                    break;
                case FrameBufferFormat.Direct15:
                    for (var y = 0; y < ySize; y++)
                    {
                        for (var x = 0; x < xSize; x++)
                        {
                            var i = (picPosY + y) * width + picPosX + x;
                            var j = pixels[i];
                            texData[(ySize - 1 - y) * xSize + x] = new TransparentColor(j).ToColor(mode);
                        }
                    }
                    break;
                case FrameBufferFormat.Direct24:
                    for (var y = 0; y < ySize; y++)
                    {
                        for (var x = 0; x < xSize; x++)
                        {
                            var i = (picPosY + y) * width + picPosX + x * 3 / 2;
                            var j = pixels[i];
                            var k = pixels[i + 1];

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

                            texData[(ySize - 1 - y) * xSize + x] = new Color32(r, g, b, byte.MaxValue);
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

            var texture = new Texture2D(xSize, ySize);

            texture.SetPixels32(texData);
            texture.Apply();

            return texture;
        }
    }
}