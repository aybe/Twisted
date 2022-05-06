// #define DMD_DEBUG_POLYGON_TEXTURE // TODO this should be toggleable from editor

using System;
using System.ComponentModel;

namespace Twisted.Formats.Graphics2D
{
    public readonly struct TexturePage : IEquatable<TexturePage>, IComparable<TexturePage>, IComparable
    {
        public TexturePosition Position { get; }

        public TexturePageAlpha Alpha { get; }

        public TexturePageColors Colors { get; }

        public TexturePageDisable Disable { get; }

        public int Index => Position.Y % 256 * 16 + Position.X % 64;

        public TexturePage(TexturePosition position, TexturePageAlpha alpha, TexturePageColors colors, TexturePageDisable disable)
        {
            if (!Enum.IsDefined(typeof(TexturePageAlpha), alpha))
                throw new InvalidEnumArgumentException(nameof(alpha), (int)alpha, typeof(TexturePageAlpha));

            if (!Enum.IsDefined(typeof(TexturePageColors), colors))
                throw new InvalidEnumArgumentException(nameof(colors), (int)colors, typeof(TexturePageColors));

            if (!Enum.IsDefined(typeof(TexturePageDisable), disable))
                throw new InvalidEnumArgumentException(nameof(disable), (int)disable, typeof(TexturePageDisable));

            Position = position;
            Alpha    = alpha;
            Colors   = colors;
            Disable  = disable;

            // BUG there are non-standard texture pages, only log an error and see how texture atlas behaves then

            var xMax = 1024 - GetPixelWidth(this);

            if (position.X > xMax)
            {
                // throw new ArgumentOutOfRangeException(nameof(position.X), $"Must not be greater than {xMax}.");
#if DMD_DEBUG_POLYGON_TEXTURE // very slow
                Debug.LogError($"X must not be greater than {xMax}.");
#endif
            }

            const int yMax = 256;

            if (position.Y > yMax)
            {
                // throw new ArgumentOutOfRangeException(nameof(position.Y), $"Must not be greater than {yMax}.");
#if DMD_DEBUG_POLYGON_TEXTURE // very slow
                Debug.LogError($"Y must not be greater than {yMax}.");
#endif
            }
        }

        public int CompareTo(TexturePage other)
        {
            var positionComparison = Position.CompareTo(other.Position);

            if (positionComparison != 0)
            {
                return positionComparison;
            }

            var alphaComparison = Alpha.CompareTo(other.Alpha);

            if (alphaComparison != 0)
            {
                return alphaComparison;
            }

            var colorsComparison = Colors.CompareTo(other.Colors);

            if (colorsComparison != 0)
            {
                return colorsComparison;
            }

            var disableComparison = Disable.CompareTo(other.Disable);

            if (disableComparison != 0)
            {
                return disableComparison;
            }

            return 0;
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return 1;

            return obj is TexturePage other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TexturePage)}");
        }

        public bool Equals(TexturePage other)
        {
            return Position.Equals(other.Position) && Alpha == other.Alpha && Colors == other.Colors && Disable == other.Disable;
        }

        public override bool Equals(object? obj)
        {
            return obj is TexturePage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, (int)Alpha, (int)Colors, (int)Disable);
        }

        public override string ToString()
        {
            return $"{nameof(Position)}: {Position}, {nameof(Colors)}: {Colors}";
        }

        public static int GetPixelWidth(TexturePage page)
        {
            var colors = page.Colors;

            return colors switch
            {
                TexturePageColors.FourBits    => 64,
                TexturePageColors.EightBits   => 128,
                TexturePageColors.FifteenBits => 256,
                TexturePageColors.Reserved    => throw new NotSupportedException(colors.ToString()),
                _                             => throw new NotSupportedException(colors.ToString())
            };
        }

        public static bool operator ==(TexturePage left, TexturePage right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TexturePage left, TexturePage right)
        {
            return !left.Equals(right);
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
    }
}