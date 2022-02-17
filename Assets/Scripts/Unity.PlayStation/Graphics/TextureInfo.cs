using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity.PlayStation.Graphics
{
    [PublicAPI]
    public readonly struct TextureInfo
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

        public IReadOnlyList<Vector2Int> UVs { get; } // BUG this has nothing to do in here

        public override string ToString()
        {
            return $"{nameof(Page)}: {Page}, {nameof(Palette)}: {Palette}";
        }
    }
}