using JetBrains.Annotations;
using UnityEngine;

namespace Unity.PlayStation.Graphics
{
    [PublicAPI]
    public readonly struct TextureInfo
    {
        public TextureInfo(TexturePage page, Vector2Int palette)
        {
            Page    = page;
            Palette = palette;
        }

        public TexturePage Page { get; }

        public Vector2Int Palette { get; }

        public override string ToString()
        {
            return $"{nameof(Page)}: {Page}, {nameof(Palette)}: {Palette}";
        }
    }
}