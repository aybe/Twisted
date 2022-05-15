using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Twisted.Formats.Graphics2D;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Twisted.Editor
{
    internal sealed class ViewerFactory
    {
        public ViewerFactory(BinaryReader reader)
        {
            var tms = new Tms(reader);

            var buffer = FrameBuffer.CreatePlayStationVideoMemory();

            foreach (var tim in tms)
            {
                if (tim.Picture is not null)
                {
                    buffer.Blit(tim.Picture);
                }

                if (tim.Palettes is null)
                {
                    continue;
                }

                foreach (var palette in tim.Palettes)
                {
                    buffer.Blit(palette);
                }
            }

            Buffer = buffer;
        }

        private FrameBuffer Buffer { get; }

        public ViewerTexturing GetTextureAtlas(TextureInfo[] infos, Progress? progress = null)
        {
            // initialize VRAM, generate the set of textures for this set of texture info

            var list = new SortedList<TextureInfo, Texture2D>();

            for (var i = 0; i < infos.Length; i++)
            {
                progress?.Report(1.0f / infos.Length * (i + 1));

                var info = infos[i];

                if (list.ContainsKey(info))
                    continue;

                var value = GetTexture(info, TransparentColorMode.None);

                list.Add(info, value);
            }

            var textures = list.Values.ToArray();

            if (!TextureAtlas.TryCreate(textures, out var atlas, out var atlasTexture))
                throw new InvalidOperationException(
                    "Couldn't create texture atlas, try increase atlas size or reduce the number of textures."
                );

            var indices = new ReadOnlyDictionary<TextureInfo, int>(list.ToDictionary(s => s.Key, s => list.IndexOfKey(s.Key))); // TODO

            return new ViewerTexturing(atlas, new Dictionary<TextureInfo, int>(indices), atlasTexture, new Dictionary<TextureInfo, Texture2D>(list));
        }

        public void ExportData(ViewerTexturing texturing, string directory, Progress? progress = null)
        {
            Directory.CreateDirectory(directory);

            File.WriteAllBytes(Path.Combine(directory, "TMS buffer.BIN"), MemoryMarshal.Cast<short, byte>(Buffer.Pixels.ToArray()).ToArray());

            var texture = FrameBuffer.GetTexture(Buffer.Format, Buffer, Buffer.Rect);

            File.WriteAllBytes(Path.Combine(directory, "TMS buffer.PNG"), texture.EncodeToPNG());

            Object.DestroyImmediate(texture);

            var index    = 0;
            var textures = texturing.AtlasTextures;

            foreach (var (key, value) in textures)
            {
                var name = $"Index = {index++:D3}, PX = {key.Page.Position.X}, PY = {key.Page.Position.Y}, PC = {key.Page.Colors}, CX = {key.Palette.X}, CY = {key.Palette.Y}";

                if (key.Window.HasValue)
                {
                    name += $", MX = {key.Window.Value.MaskX}, MY = {key.Window.Value.MaskY}, OX = {key.Window.Value.OffsetX}, OY = {key.Window.Value.OffsetY}";
                }

                var path = Path.Combine(directory, Path.ChangeExtension(name, ".PNG"));

                File.WriteAllBytes(path, value.EncodeToPNG());

                progress?.Report(1.0f / textures.Count * index);
            }

            File.WriteAllBytes(Path.Combine(directory, "TMS atlas.png"), texturing.AtlasTexture.EncodeToPNG());
        }

        private Texture2D GetTexture(TextureInfo info, TransparentColorMode mode)
        {
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

            var picRect = new RectInt(info.Page.Position.X, info.Page.Position.Y, picWidth, 256);
            var palRect = new RectInt(info.Palette.X,       info.Palette.Y,       palWidth, 1);
            var texture = FrameBuffer.GetTexture(picFormat, Buffer, picRect, Buffer, palRect, mode);

            if (info.Window == null)
            {
                return texture;
            }

            var textureWidth  = texture.width;
            var textureHeight = texture.height;
            var textureSource = texture.GetPixels32();
            var textureTarget = new Color32[textureSource.Length];
            var window        = info.Window.Value;
            var windowMaskX   = window.MaskX;
            var windowMaskY   = window.MaskY;
            var windowOffsetX = window.OffsetX;
            var windowOffsetY = window.OffsetY;

            for (var y = 0; y < textureHeight; y++)
            {
                for (var x = 0; x < textureWidth; x++)
                {
                    var u = TextureWindow.Transform(x, windowMaskX, windowOffsetX);
                    var v = TextureWindow.Transform(y, windowMaskY, windowOffsetY);
                    var i = (textureHeight - v - 1) * textureWidth + u;
                    var j = (textureHeight - y - 1) * textureWidth + x;

                    textureTarget[j] = textureSource[i];
                }
            }

            texture.SetPixels32(textureTarget);
            texture.Apply();

            return texture;
        }
    }
}