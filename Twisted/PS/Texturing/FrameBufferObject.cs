using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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
        public Rectangle Rectangle { get; }

        public IReadOnlyList<short> Pixels { get; }

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
            
            var pixels = Pixels as short[] ?? Pixels.ToArray();

            var data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            
            bitmap.UnlockBits(data);

            if (Format is FrameBufferObjectFormat.Indexed4 or FrameBufferObjectFormat.Indexed8)
            {
                if (paletteObject != null)
                {
                    var palette     = bitmap.Palette;
                    var paletteData = paletteObject.Pixels as short[] ?? paletteObject.Pixels.ToArray();
                    var paletteSize = paletteObject.GetWidth();

                    for (var i = 0; i < paletteSize; i++)
                    {
                        var u = paletteData[i]; // BUG this is little-endian
                        var v = new TimColor(u);
                        var w = v.ToColor(translucency);
                        palette.Entries[i] = w;
                    }

                    bitmap.Palette = palette;
                }
            }

            return bitmap;
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
    }
}