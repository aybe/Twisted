using System;
using System.Collections.Generic;

namespace Unity.PlayStation.Graphics
{
    public readonly struct TextureInfo // TODO rename because it's weird
        // TODO texture window?
    {
        public TextureInfo(TexturePage page, TexturePalette palette, IReadOnlyList<TextureUV> uvs)
        {
            if (uvs == null)
                throw new ArgumentNullException(nameof(uvs));

            if (uvs.Count is not (3 or 4))
                throw new ArgumentOutOfRangeException(nameof(uvs), "3 or 4 UVs expected.");

            Page    = page;
            Palette = palette;
            UVs     = uvs ?? throw new ArgumentNullException(nameof(uvs));
        }

        public TexturePage Page { get; }

        public TexturePalette Palette { get; }

        public IReadOnlyList<TextureUV> UVs { get; }

        public override string ToString()
        {
            return $"{nameof(Page)}: {Page}, {nameof(Palette)}: {Palette}";
        }
    }
}