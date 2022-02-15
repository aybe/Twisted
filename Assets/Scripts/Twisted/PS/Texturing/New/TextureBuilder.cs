using System;
using System.Collections.Generic;
using System.IO;
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
                // TODO delete this frame buffer debugging
                var temp = Directory.CreateDirectory(Path.Combine(Application.dataPath, "../.temp")).FullName;

                using var bin = File.Create(Path.Combine(temp, "cars.bin"));
                using var tga = File.Create(Path.Combine(temp, "cars.tga"));
                FrameBufferObject.WriteRaw(bin, psx);
                FrameBufferObject.WriteTga(tga, psx);
            }

            // generate the set of textures for the set of polygons

            var set = new HashSet<TexturePageFormatKey>(); // TODO add equality comparer

            foreach (var polygon in polygons)
            {
                if (polygon is Polygon04010B0C pt)
                {
                    Debug.Log(pt.Texture);

                    var texture = GetTexture(psx, pt.Texture, TransparentColorMode.None);

                    // BUG why is black shown as white?
                    // BUG semi-transparency not set
                    File.WriteAllBytes(@"C:\temp\cars.png", texture.EncodeToPNG());
                    // BUG new TexturePageFormatKey(texture.Page, (TexturePageFormat)texture.Page.Colors);
                    break;
                }
            }
        }

        public static Texture2D GetTexture(FrameBufferObject obj, Texture tp, TransparentColorMode mode)
            // TODO wrong name // TODO move
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (obj.Format is not FrameBufferObjectFormat.Direct15 && obj.Rectangle.Width is not 1024 && obj.Rectangle.Height is not 512)
                throw new ArgumentOutOfRangeException(nameof(obj));

            if (EqualityComparer<Texture>.Default.Equals(tp, default))
                throw new ArgumentOutOfRangeException(nameof(tp));

            var colors = tp.Page.Colors;

            var format = colors switch
            {
                TexturePageColors.FourBits    => FrameBufferObjectFormat.Indexed4,
                TexturePageColors.EightBits   => FrameBufferObjectFormat.Indexed8,
                TexturePageColors.FifteenBits => FrameBufferObjectFormat.Direct15,
                TexturePageColors.Reserved    => throw new NotSupportedException(colors.ToString()),
                _                             => throw new NotSupportedException(colors.ToString())
            };

            var pageWidth = colors switch
            {
                TexturePageColors.FourBits    => 64,
                TexturePageColors.EightBits   => 128,
                TexturePageColors.FifteenBits => 256,
                TexturePageColors.Reserved    => throw new NotSupportedException(colors.ToString()),
                _                             => throw new NotSupportedException(colors.ToString())
            };

            var paletteWidth = colors switch
            {
                TexturePageColors.FourBits    => 16,
                TexturePageColors.EightBits   => 256,
                TexturePageColors.FifteenBits => 0,
                TexturePageColors.Reserved    => throw new NotSupportedException(colors.ToString()),
                _                             => throw new NotSupportedException(colors.ToString())
            };

            var picRect = new RectInt(tp.Page.X, tp.Page.Y, pageWidth, 256);
            var palRect = new RectInt(tp.Palette.X, tp.Palette.Y, paletteWidth, 1);
            var texture = FrameBufferObject.GetTexture(format, obj, picRect, palRect, obj, mode);

            return texture;
        }
    }
}