﻿#define DEBUG_TEXTURES
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Extensions.Graphics;
using Unity.PlayStation.Graphics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Twisted.PS.Texturing.New
{
    public static class TextureBuilder
    {
        public static void GetTexture(TextureInfo[] infos)
        {
            if (infos is null)
                throw new ArgumentNullException(nameof(infos));

            if (infos.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(infos));

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

            // generate the set of textures for the set of texture info

            var dictionary = new SortedDictionary<TextureInfo, Texture2D>(TextureInfoComparer.Instance);

            foreach (var info in infos)
            {
                if (dictionary.ContainsKey(info))
                    continue;

                var value = GetTexture(psx, info, TransparentColorMode.None);

                dictionary.Add(info, value);
            }

#if DEBUG_TEXTURES

            if (dictionary.Count is 0)
            {
                Debug.LogWarning("No textures were generated for this model because it doesn't have any textured polygon.");
                return;
            }

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
                           $"PageX = {key.Page.Position.x}, " +
                           $"PageY = {key.Page.Position.y}, " +
                           $"PageColors = {key.Page.Colors}, " +
                           $"PaletteX = {key.Palette.x}, " +
                           $"PaletteY = {key.Palette.y}";

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

            if (buffer.Format is not FrameBufferFormat.Direct15 && buffer.Rect.width is not 1024 && buffer.Rect.height is not 512)
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

            var picRect = new RectInt(tp.Page.Position.x, tp.Page.Position.y, pageWidth, 256);
            var palRect = new RectInt(tp.Palette.x, tp.Palette.y, paletteWidth, 1);
            var texture = FrameBuffer.GetTexture(format, buffer, picRect, palRect, buffer, mode);

            return texture;
        }
    }
}