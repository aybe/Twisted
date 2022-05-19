using System;
using UnityEngine.Assertions;

namespace Twisted.Formats.Graphics2D
{
    public readonly struct TextureWindow : IEquatable<TextureWindow>, IComparable<TextureWindow>, IComparable
    {
        public int MaskX { get; }

        public int MaskY { get; }

        public int OffsetX { get; }

        public int OffsetY { get; }

        public TextureWindow(int data)
        {
            MaskX   = (data >> 00) & 0x1F;
            MaskY   = (data >> 05) & 0x1F;
            OffsetX = (data >> 10) & 0x1F;
            OffsetY = (data >> 15) & 0x1F;

            Assert.AreEqual(0xE2, (data >> 24) & 0xFF);
        }

        public TextureWindow(int maskX, int maskY, int offsetX, int offsetY)
        {
            if (maskX is < 0 or > 31)
                throw new ArgumentOutOfRangeException(nameof(maskX));

            if (maskY is < 0 or > 31)
                throw new ArgumentOutOfRangeException(nameof(maskX));

            if (offsetX is < 0 or > 31)
                throw new ArgumentOutOfRangeException(nameof(maskX));

            if (offsetY is < 0 or > 31)
                throw new ArgumentOutOfRangeException(nameof(maskX));

            MaskX   = maskX;
            MaskY   = maskY;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public int CompareTo(TextureWindow other)
        {
            var maskXComparison = MaskX.CompareTo(other.MaskX);

            if (maskXComparison != 0)
            {
                return maskXComparison;
            }

            var maskYComparison = MaskY.CompareTo(other.MaskY);

            if (maskYComparison != 0)
            {
                return maskYComparison;
            }

            var offsetXComparison = OffsetX.CompareTo(other.OffsetX);

            if (offsetXComparison != 0)
            {
                return offsetXComparison;
            }

            var offsetYComparison = OffsetY.CompareTo(other.OffsetY);

            if (offsetYComparison != 0)
            {
                return offsetYComparison;
            }

            return 0;
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return 1;

            return obj is TextureWindow other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TextureWindow)}");
        }

        public bool Equals(TextureWindow other)
        {
            return MaskX == other.MaskX && MaskY == other.MaskY && OffsetX == other.OffsetX && OffsetY == other.OffsetY;
        }

        public override bool Equals(object? obj)
        {
            return obj is TextureWindow other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MaskX, MaskY, OffsetX, OffsetY);
        }

        public override string ToString()
        {
            return $"{nameof(MaskX)}: {MaskX}, {nameof(MaskY)}: {MaskY}, {nameof(OffsetX)}: {OffsetX}, {nameof(OffsetY)}: {OffsetY}";
        }

        public static int Transform(int value, int mask, int offset)
        {
            return (value & ~(mask * 8)) | ((offset & mask) * 8);
        }

        public static bool operator ==(TextureWindow left, TextureWindow right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextureWindow left, TextureWindow right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(TextureWindow left, TextureWindow right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(TextureWindow left, TextureWindow right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(TextureWindow left, TextureWindow right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(TextureWindow left, TextureWindow right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}