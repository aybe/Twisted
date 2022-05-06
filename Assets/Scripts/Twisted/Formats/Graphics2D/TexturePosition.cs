using System;

namespace Twisted.Formats.Graphics2D
{
    public readonly struct TexturePosition : IEquatable<TexturePosition>, IComparable<TexturePosition>, IComparable
    {
        public int X { get; }

        public int Y { get; }

        public TexturePosition(int x, int y)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException(nameof(x), "Must be positive.");

            if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y), "Must be positive.");

            if (x % 64 != 0)
                throw new ArgumentOutOfRangeException(nameof(x), x, "Must be a multiple of 64.");

            if (y % 256 != 0)
                throw new ArgumentOutOfRangeException(nameof(x), y, "Must be a multiple of 256.");

            X = x;
            Y = y;
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return 1;

            return obj is TexturePosition other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TexturePosition)}");
        }

        public int CompareTo(TexturePosition other)
        {
            var xComparison = X.CompareTo(other.X);

            if (xComparison != 0)
            {
                return xComparison;
            }

            var yComparison = Y.CompareTo(other.Y);

            if (yComparison != 0)
            {
                return yComparison;
            }

            return 0;
        }

        public bool Equals(TexturePosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is TexturePosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        public static bool operator ==(TexturePosition left, TexturePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TexturePosition left, TexturePosition right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(TexturePosition left, TexturePosition right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(TexturePosition left, TexturePosition right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(TexturePosition left, TexturePosition right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(TexturePosition left, TexturePosition right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}