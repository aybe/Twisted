using System;
using System.Collections.Generic;

namespace Twisted.PS.Texturing
{
    public readonly struct Texture
        // TODO texture window?
    {
        public Texture(IReadOnlyList<TextureUV> uvs, TexturePage page, TexturePalette palette)
        {
            if (uvs == null)
                throw new ArgumentNullException(nameof(uvs));

            if (uvs.Count is not (3 or 4))
                throw new ArgumentOutOfRangeException(nameof(uvs), "3 or 4 UVs expected.");

            UVs     = uvs ?? throw new ArgumentNullException(nameof(uvs));
            Page    = page;
            Palette = palette;
        }

        public IReadOnlyList<TextureUV> UVs { get; }

        public TexturePage Page { get; }

        public TexturePalette Palette { get; }
    }
}