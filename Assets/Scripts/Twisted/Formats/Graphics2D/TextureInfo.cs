using System;

namespace Twisted.Formats.Graphics2D
{
    public readonly struct TextureInfo : IComparable<TextureInfo>, IComparable, IEquatable<TextureInfo>
    {
        public TextureInfo(TexturePage page, TexturePalette palette, TextureWindow? window = null)
        {
            Page    = page;
            Palette = palette;
            Window  = window;
        }

        public TexturePage Page { get; }

        public TexturePalette Palette { get; }

        public TextureWindow? Window { get; }

        public override string ToString()
        {
            return $"{nameof(Page)}: {Page}, {nameof(Palette)}: {Palette}, {nameof(Window)}: {Window}";
        }

        public int CompareTo(TextureInfo other)
        {
            var pageComparison = Page.CompareTo(other.Page);

            if (pageComparison != 0)
            {
                return pageComparison;
            }

            var paletteComparison = Palette.CompareTo(other.Palette);

            if (paletteComparison != 0)
            {
                return paletteComparison;
            }

            var windowComparison = Nullable.Compare(Window, other.Window);

            if (windowComparison != 0)
            {
                return windowComparison;
            }

            return 0;
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return 1;

            return obj is TextureInfo other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TextureInfo)}");
        }

        public bool Equals(TextureInfo other)
        {
            return Page.Equals(other.Page) && Palette.Equals(other.Palette) && Nullable.Equals(Window, other.Window);
        }

        public override bool Equals(object? obj)
        {
            return obj is TextureInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Page, Palette, Window);
        }

        public static bool operator ==(TextureInfo left, TextureInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextureInfo left, TextureInfo right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(TextureInfo left, TextureInfo right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(TextureInfo left, TextureInfo right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(TextureInfo left, TextureInfo right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(TextureInfo left, TextureInfo right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}