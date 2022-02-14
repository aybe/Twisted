using System;
using System.Collections.Generic;

namespace Twisted.PS.Texturing
{
    public readonly struct Texture
        // TODO texture window?
    {
        public Texture(TexturePage page, TexturePalette palette, IReadOnlyList<TextureUV> uvs)
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