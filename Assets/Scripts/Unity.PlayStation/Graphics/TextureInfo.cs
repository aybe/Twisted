using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.PlayStation.Graphics
{
    public readonly struct TextureInfo // TODO rename because it's weird
        // TODO texture window?
    {
        public TextureInfo(TexturePage page, Vector2Int palette, IReadOnlyList<Vector2Int> uvs)
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

        public Vector2Int Palette { get; }

        public IReadOnlyList<Vector2Int> UVs { get; }

        public override string ToString()
        {
            return $"{nameof(Page)}: {Page}, {nameof(Palette)}: {Palette}";
        }
    }
}