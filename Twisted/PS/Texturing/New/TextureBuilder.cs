using System;
using System.Collections.Generic;
using System.IO;
using Twisted.Extensions;
using Twisted.PS.Polygons;
using UnityEngine;

namespace Twisted.PS.Texturing.New
{
    public static class TextureBuilder
    {
        public static void GetTexture(IReadOnlyList<Polygon> polygons)
        {
            if (polygons is null)
                throw new ArgumentNullException(nameof(polygons));

            if (polygons.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(polygons));

            // TODO this shouldn't be hardcoded

            using var str = new MemoryStream(File.ReadAllBytes(@"C:\Users\aybe\source\repos\.twisted\TM1PSJAP\UADMD\CARS.TMS"));

            // create a frame buffer out of TMS

            var tms = new Tms(str); // TODO extract method, pass as param

            var psx = FrameBufferObject.CreatePlayStationVideoMemory();

            foreach (var tim in tms)
            {
                if (tim.Picture is not null)
                {
                    psx.Blit(tim.Picture);
                }

                if (tim.Palettes is null)
                    continue;

                foreach (var palette in tim.Palettes)
                {
                    psx.Blit(palette);
                }
            }

            {
                using var bin = File.Create(@"C:\temp\cars.bin");
                using var tga = File.Create(@"C:\temp\cars.tga");
                FrameBufferObject.WriteRaw(bin, psx);
                FrameBufferObject.WriteTga(tga, psx); // TODO delete this frame buffer debugging
            }

            // generate the set of textures for the set of polygons

            var set = new HashSet<TexturePageFormatKey>(); // TODO add equality comparer

            foreach (var polygon in polygons)
            {
                if (polygon is Polygon04010B0C pt)
                {
                    var texture = pt.Texture;

                    Debug.WriteLine(texture.ToString());

                    var image = GetTexture(psx, texture);

                    // BUG there should be no unity here
                    var pw      = image.PixelWidth;
                    var ph      = image.PixelHeight;
                    var surface = new Texture2D(pw, ph);
                    var colors  = new Color32[pw * ph];

                    // BUG why is black shown as white?
                    // BUG semi-transparency not set

                    for (var y = 0; y < ph; y++) // stupid Unity is from bottom to top
                    {
                        for (var x = 0; x < pw; x++)
                        {
                            var i = image.PixelData[(ph - 1 - y) * pw + x];
                            colors[y * pw + x] = i.ToColor32();
                        }
                    }
                    surface.SetPixels32(colors);
                    surface.Apply();

                    File.WriteAllBytes(@"C:\temp\cars.png", surface.EncodeToPNG());
                    // new TexturePageFormatKey(texture.Page, (TexturePageFormat)texture.Page.Colors); // BUG

                    break;
                }
            }
        }

        public static TextureImage GetTexture(FrameBufferObject obj, Texture texture)
            // TODO wrong name // TODO move
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (obj.Format is not FrameBufferObjectFormat.Direct15 && obj.Rectangle.Width is not 1024 && obj.Rectangle.Height is not 512)
                throw new ArgumentOutOfRangeException(nameof(obj));

            if (EqualityComparer<Texture>.Default.Equals(texture, default))
                throw new ArgumentOutOfRangeException(nameof(texture));

            var tpc = texture.Page.Colors;
            var tpx = texture.Palette.X;
            var tpy = texture.Palette.Y;

            var pixels = new TransparentColor[256 * 256];
            var offset = 0;
            var extent = tpc switch
            {
                TexturePageColors.FourBits    => 64,
                TexturePageColors.EightBits   => 128,
                TexturePageColors.FifteenBits => 256,
                TexturePageColors.Reserved    => throw new NotSupportedException(tpc.ToString()),
                _                             => throw new NotSupportedException(tpc.ToString())
            };
            var colors = tpc switch
            {
                TexturePageColors.FourBits    => FrameBufferObject.GetPalette(obj, tpx, tpy, 16),
                TexturePageColors.EightBits   => FrameBufferObject.GetPalette(obj, tpx, tpy, 256),
                TexturePageColors.FifteenBits => Array.Empty<TransparentColor>(),
                TexturePageColors.Reserved    => throw new NotSupportedException(tpc.ToString()),
                _                             => throw new NotSupportedException(tpc.ToString())
            };

            for (var y = 0; y < 256; y++)
            {
                for (var x = 0; x < extent; x++)
                {
                    var s = obj.Pixels[(texture.Page.Y + y) * 1024 + texture.Page.X + x];

                    switch (tpc)
                    {
                        case TexturePageColors.FourBits:
                            pixels[offset++] = colors[s.GetNibble(0)];
                            pixels[offset++] = colors[s.GetNibble(1)];
                            pixels[offset++] = colors[s.GetNibble(2)];
                            pixels[offset++] = colors[s.GetNibble(3)];
                            break;
                        case TexturePageColors.EightBits:
                            pixels[offset++] = colors[s.GetByte(0)];
                            pixels[offset++] = colors[s.GetByte(1)];
                            break;
                        case TexturePageColors.FifteenBits:
                            pixels[offset++] = new TransparentColor(obj.Pixels[y * 1024 + x]);
                            break;
                        case TexturePageColors.Reserved:
                            throw new NotSupportedException(tpc.ToString());
                        default:
                            throw new NotSupportedException(tpc.ToString());
                    }
                }
            }

            return new TextureImage(256, 256, pixels);
        }
    }
}