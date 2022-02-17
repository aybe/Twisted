#define DEBUG_TEXTURES
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Twisted.PS.Polygons;
using Unity.Extensions.Graphics;
using UnityEngine;
using Object = UnityEngine.Object;

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

            var psx = FrameBuffer.CreatePlayStationVideoMemory();

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

            // generate the set of textures for the set of polygons

            var dictionary = new SortedDictionary<TextureInfo, Texture2D>(TextureInfoComparer.Instance);

            foreach (var polygon in polygons)
            {
                if (polygon is not Polygon04010B0C pt)
                    continue;

                var key = pt.TextureInfo;

                if (dictionary.ContainsKey(key))
                    continue;

                var value = GetTexture(psx, key, TransparentColorMode.None);

                dictionary.Add(key, value);
            }

#if DEBUG_TEXTURES

            if (dictionary.Count is 0)
                return;

            var directory = Path.GetFullPath(Path.Combine(Application.dataPath, "../.temp/TextureBuilder"));

            if (Directory.Exists(directory))
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            Directory.CreateDirectory(directory);

            var texture = FrameBuffer.GetTexture(psx.Format, psx, psx.Rect);
            var png     = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);

            File.WriteAllBytes(Path.Combine(directory, "TMS dump.BIN"), MemoryMarshal.Cast<short, byte>(psx.Pixels.ToArray()).ToArray());
            File.WriteAllBytes(Path.Combine(directory, "TMS dump.PNG"), png);

            var index = 0;

            foreach (var (key, value) in dictionary)
            {
                var name = $"Index = {index++}, " +
                           $"PageX = {key.Page.X}, " +
                           $"PageY = {key.Page.Y}, " +
                           $"PageColors = {key.Page.Colors}, " +
                           $"PaletteX = {key.Palette.X}, " +
                           $"PaletteY = {key.Palette.Y}";

                var path = Path.Combine(directory, Path.ChangeExtension(name, ".PNG"));

                File.WriteAllBytes(path, value.EncodeToPNG());
            }

            Debug.Log($"Generated {dictionary.Count} textures in {directory}.");

            if (!TextureAtlas.TryCreate(dictionary.Values.ToArray(), out var atlas, out var atlasTexture))
                throw new InvalidOperationException("Couldn't create texture atlas, try increase atlas size or reduce the number of textures.");

            File.WriteAllBytes(Path.Combine(directory, "TextureAtlas.png"), atlasTexture.EncodeToPNG());

            Object.DestroyImmediate(atlas);
#endif
        }

        public static Texture2D GetTexture(FrameBuffer buffer, TextureInfo tp, TransparentColorMode mode)
            // TODO wrong name // TODO move
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Format is not FrameBufferFormat.Direct15 && buffer.Rectangle.Width is not 1024 && buffer.Rectangle.Height is not 512)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            if (EqualityComparer<TextureInfo>.Default.Equals(tp, default))
                throw new ArgumentOutOfRangeException(nameof(tp));

            var colors = tp.Page.Colors;

            var format = colors switch
            {
                TexturePageColors.FourBits    => FrameBufferFormat.Indexed4,
                TexturePageColors.EightBits   => FrameBufferFormat.Indexed8,
                TexturePageColors.FifteenBits => FrameBufferFormat.Direct15,
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
            var texture = FrameBuffer.GetTexture(format, buffer, picRect, palRect, buffer, mode);

            return texture;
        }
    }
}