using System;

namespace Twisted.Formats.Binary
{
    public readonly struct BinaryStreamRegion : IComparable<BinaryStreamRegion>, IEquatable<BinaryStreamRegion>
    {
        public readonly long Position;
        public readonly long Length;

        internal BinaryStreamRegion(long position, long length)
        {
            Position = position;
            Length   = length;
        }

        public override string ToString()
        {
            return $"{nameof(Position)}: {Position}, {nameof(Length)}: {Length}";
        }

        #region IComparable

        public int CompareTo(BinaryStreamRegion other)
        {
            var position = Position.CompareTo(other.Position);

            if (position != 0)
            {
                return position;
            }

            var length = Length.CompareTo(other.Length);

            if (length != 0)
            {
                return length;
            }

            return 0;
        }

        public static bool operator <(BinaryStreamRegion left, BinaryStreamRegion right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(BinaryStreamRegion left, BinaryStreamRegion right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(BinaryStreamRegion left, BinaryStreamRegion right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(BinaryStreamRegion left, BinaryStreamRegion right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion

        #region IEquatable

        public bool Equals(BinaryStreamRegion other)
        {
            return Position == other.Position && Length == other.Length;
        }

        public override bool Equals(object? obj)
        {
            return obj is BinaryStreamRegion other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Length);
        }

        public static bool operator ==(BinaryStreamRegion left, BinaryStreamRegion right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BinaryStreamRegion left, BinaryStreamRegion right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}