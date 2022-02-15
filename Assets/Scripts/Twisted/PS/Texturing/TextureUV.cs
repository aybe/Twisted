using System;

namespace Twisted.PS.Texturing
{
    public readonly struct TextureUV : IEquatable<TextureUV>
    {
        public int U { get; }

        public int V { get; }

        public TextureUV(int u, int v)
        {
            U = u;
            V = v;
        }

        public void Deconstruct(out int x, out int y)
        {
            x = U;
            y = V;
        }

        public override string ToString()
        {
            return $"{nameof(U)}: {U}, {nameof(V)}: {V}";
        }

        public bool Equals(TextureUV other)
        {
            return U == other.U && V == other.V;
        }

        public override bool Equals(object? obj)
        {
            return obj is TextureUV other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(U, V);
        }

        public static bool operator ==(TextureUV left, TextureUV right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextureUV left, TextureUV right)
        {
            return !left.Equals(right);
        }
    }
}