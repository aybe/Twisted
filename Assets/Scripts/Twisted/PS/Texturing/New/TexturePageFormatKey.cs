using System;

namespace Twisted.PS.Texturing.New
{
    [Obsolete]
    public readonly struct TexturePageFormatKey : IEquatable<TexturePageFormatKey>
    {
        public TexturePage Page { get; }

        public TexturePageFormat PageFormat { get; }

        public TexturePageFormatKey(TexturePage page, TexturePageFormat pageFormat)
        {
            Page       = page;
            PageFormat = pageFormat;
        }

        #region IEquatable

        public bool Equals(TexturePageFormatKey other)
        {
            return Page.Equals(other.Page) && PageFormat == other.PageFormat;
        }

        public override bool Equals(object? obj)
        {
            return obj is TexturePageFormatKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Page, (int)PageFormat);
        }

        public static bool operator ==(TexturePageFormatKey left, TexturePageFormatKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TexturePageFormatKey left, TexturePageFormatKey right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}