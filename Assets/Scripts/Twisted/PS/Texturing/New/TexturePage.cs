using System;

namespace Twisted.PS.Texturing.New
{
    public readonly struct TexturePage : IComparable, IComparable<TexturePage>, IEquatable<TexturePage>
    {
        public int Index { get; }

        public int X => Index % 16 * 64;

        public int Y => Index / 16 * 256;

        public TexturePage(int index)
        {
            if (index is < 0 or > 31)
                throw new ArgumentOutOfRangeException(nameof(index));

            Index = index;
        }

        public override string ToString()
        {
            return $"{nameof(Index)}: {Index}, {nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        #region IComparable

        public int CompareTo(TexturePage other)
        {
            return Index.CompareTo(other.Index);
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return 1;

            return obj is TexturePage other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TexturePage)}");
        }

        public static bool operator <(TexturePage left, TexturePage right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(TexturePage left, TexturePage right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(TexturePage left, TexturePage right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(TexturePage left, TexturePage right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion

        #region IEquatable

        public bool Equals(TexturePage other)
        {
            return Index == other.Index;
        }

        public override bool Equals(object? obj)
        {
            return obj is TexturePage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Index;
        }

        public static bool operator ==(TexturePage left, TexturePage right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TexturePage left, TexturePage right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}