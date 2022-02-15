using System;

namespace Twisted.PS.Texturing
{
    public readonly struct TexturePalette : IEquatable<TexturePalette>
    {
        public int X { get; }

        public int Y { get; }

        public TexturePalette(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
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

        public static bool operator ==(TexturePalette left, TexturePalette right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TexturePalette left, TexturePalette right)
        {
            return !left.Equals(right);
        }
    }
}