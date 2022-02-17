using System.Collections.Generic;

namespace Unity.PlayStation.Graphics
{
    /// <summary>
    ///     Compares by page position, page colors, palette position.
    /// </summary>
    public sealed class TextureInfoComparer : Comparer<TextureInfo>
    {
        public static TextureInfoComparer Instance { get; } = new();

        public override int Compare(TextureInfo x, TextureInfo y)
        {
            var pageX = x.Page.X.CompareTo(y.Page.X);
            if (pageX != 0)
                return pageX;

            var pageY = x.Page.Y.CompareTo(y.Page.Y);
            if (pageY != 0)
                return pageY;

            var pageColors = x.Page.Colors.CompareTo(y.Page.Colors);
            if (pageColors != 0)
                return pageColors;

            var paletteX = x.Palette.X.CompareTo(y.Palette.X);
            if (paletteX != 0)
                return paletteX;

            var paletteY = x.Palette.Y.CompareTo(y.Palette.Y);
            if (paletteY != 0)
                return paletteY;

            return 0;
        }
    }
}