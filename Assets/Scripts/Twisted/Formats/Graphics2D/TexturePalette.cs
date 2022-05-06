using System;

namespace Twisted.Formats.Graphics2D
{
    public readonly struct TexturePalette : IEquatable<TexturePalette>, IComparable<TexturePalette>, IComparable
    {
        public int X { get; }

        public int Y { get; }

        public TexturePalette(int x, int y)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException(nameof(x), "Must be positive.");

            if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y), "Must be positive.");

            X = x;
            Y = y;
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return 1;

            return obj is TexturePalette other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TexturePalette)}");
        }

        public int CompareTo(TexturePalette other)
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

        public bool Equals(TexturePalette other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is TexturePalette other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        public static bool operator ==(TexturePalette left, TexturePalette right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TexturePalette left, TexturePalette right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(TexturePalette left, TexturePalette right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(TexturePalette left, TexturePalette right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(TexturePalette left, TexturePalette right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(TexturePalette left, TexturePalette right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}