using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.PlayStation.Graphics
{
    /// <summary>
    ///     Compares by page position, page colors, palette position.
    /// </summary>
    [PublicAPI]
    public sealed class TextureInfoComparer : Comparer<TextureInfo>
    {
        public static TextureInfoComparer Instance { get; } = new();

        public override int Compare(TextureInfo x, TextureInfo y)
        {
            var pageX = x.Page.Position.x.CompareTo(y.Page.Position.x);
            if (pageX != 0)
                return pageX;

            var pageY = x.Page.Position.y.CompareTo(y.Page.Position.y);
            if (pageY != 0)
                return pageY;

            var pageColors = x.Page.Colors.CompareTo(y.Page.Colors);
            if (pageColors != 0)
                return pageColors;

            var paletteX = x.Palette.x.CompareTo(y.Palette.x);
            if (paletteX != 0)
                return paletteX;

            var paletteY = x.Palette.y.CompareTo(y.Palette.y);
            if (paletteY != 0)
                return paletteY;

            return 0;
        }
    }
}