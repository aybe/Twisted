#define DEBUG_TEXTURES
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Twisted.Graphics;
using Unity.Extensions.Graphics;
using Unity.PlayStation.Graphics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Twisted
{
    public sealed class DMDFactory
    {
        private DMDFactory([NotNull] DMD dmd, [NotNull] TMS tms)
        {
            DMD = dmd ?? throw new ArgumentNullException(nameof(dmd));
            TMS = tms ?? throw new ArgumentNullException(nameof(tms));
        }

        public DMD DMD { get; }

        public TMS TMS { get; }

        public FrameBuffer FrameBuffer { get; set; }

        public static DMDFactory Create([NotNull] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            using var reader1 = new BinaryReader(new MemoryStream(File.ReadAllBytes(Path.ChangeExtension(path, ".DMD"))));
            using var reader2 = new BinaryReader(new MemoryStream(File.ReadAllBytes(Path.ChangeExtension(path, ".TMS"))));

            var dmd = new DMD(reader1);
            var tms = new TMS(reader2);

            return new DMDFactory(dmd, tms);
        }

        public void GetTextureAtlas(TextureInfo[] infos, out TextureAtlas atlas, out Texture2D atlasTexture, out IReadOnlyDictionary<TextureInfo, int> atlasIndices)
        {
            atlas        = default;
            atlasTexture = default;
            atlasIndices = default;

            if (infos is null)
                throw new ArgumentNullException(nameof(infos));

            if (infos.Length is 0)
                Debug.LogWarning("No textures were generated for this model because it doesn't have any textured polygon.");

            if (infos.Length is 0)
                return;

            // initialize VRAM, generate the set of textures for this set of texture info

            var buffer = FrameBuffer ??= GetBuffer();

            var list = new SortedList<TextureInfo, Texture2D>(TextureInfoComparer.Instance);

            foreach (var info in infos)
            {
                if (list.ContainsKey(info))
                    continue;

                var value = GetTexture(buffer, info, TransparentColorMode.None);

                list.Add(info, value);
            }

            var textures = list.Values.ToArray();

            if (!TextureAtlas.TryCreate(textures, out atlas, out atlasTexture))
                throw new InvalidOperationException("Couldn't create texture atlas, try increase atlas size or reduce the number of textures.");

            atlasIndices = new ReadOnlyDictionary<TextureInfo, int>(list.ToDictionary(s => s.Key, s => list.IndexOfKey(s.Key)));

#if DEBUG_TEXTURES // TODO delete this texture debugging code

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

            var texture = FrameBuffer.GetTexture(buffer.Format, buffer, buffer.Rect);
            var png     = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);

            File.WriteAllBytes(Path.Combine(directory, "TMS dump.BIN"), MemoryMarshal.Cast<short, byte>(buffer.Pixels.ToArray()).ToArray());
            File.WriteAllBytes(Path.Combine(directory, "TMS dump.PNG"), png);

            var index = 0;

            foreach (var (key, value) in list)
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

            Debug.Log($"Generated {list.Count} textures in {directory}.");

            File.WriteAllBytes(Path.Combine(directory, "TextureAtlas.png"), atlasTexture.EncodeToPNG());
#endif
        }

        private FrameBuffer GetBuffer()
        {
            var buffer = FrameBuffer.CreatePlayStationVideoMemory();

            foreach (var tim in TMS)
            {
                if (tim.Picture is not null)
                {
                    buffer.Blit(tim.Picture);
                }

                if (tim.Palettes is null)
                    continue;

                foreach (var palette in tim.Palettes)
                {
                    buffer.Blit(palette);
                }
            }

            return buffer;
        }

        private static Texture2D GetTexture(FrameBuffer buffer, TextureInfo info, TransparentColorMode mode)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Format is not FrameBufferFormat.Direct15 && buffer.Rect.width is not 1024 && buffer.Rect.height is not 512)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            if (EqualityComparer<TextureInfo>.Default.Equals(info, default))
                throw new ArgumentOutOfRangeException(nameof(info));

            var colors = info.Page.Colors;

            var picFormat = colors switch
            {
                TexturePageColors.FourBits    => FrameBufferFormat.Indexed4,
                TexturePageColors.EightBits   => FrameBufferFormat.Indexed8,
                TexturePageColors.FifteenBits => FrameBufferFormat.Direct15,
                TexturePageColors.Reserved    => throw new NotSupportedException(colors.ToString()),
                _                             => throw new NotSupportedException(colors.ToString())
            };

            var picWidth = TexturePage.GetPixelWidth(info.Page);

            var palWidth = colors switch
            {
                TexturePageColors.FourBits    => 16,
                TexturePageColors.EightBits   => 256,
                TexturePageColors.FifteenBits => 0,
                TexturePageColors.Reserved    => throw new NotSupportedException(colors.ToString()),
                _                             => throw new NotSupportedException(colors.ToString())
            };

            var picRect = new RectInt(info.Page.Position.x, info.Page.Position.y, picWidth, 256);
            var palRect = new RectInt(info.Palette.x, info.Palette.y, palWidth, 1);
            var texture = FrameBuffer.GetTexture(picFormat, buffer, picRect, buffer, palRect, mode);

            return texture;
        }
    }
}