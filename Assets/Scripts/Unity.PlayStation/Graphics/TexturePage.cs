using System;
using System.ComponentModel;

namespace Unity.PlayStation.Graphics
{
    public readonly struct TexturePage
    {
        public int X { get; }

        public int Y { get; }

        public TexturePageAlpha Alpha { get; }

        public TexturePageColors Colors { get; }

        public TexturePageDisable Disable { get; }

        public TexturePage(int x, int y, TexturePageAlpha alpha, TexturePageColors colors, TexturePageDisable disable)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException(nameof(x), "Must be positive.");

            if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y), "Must be positive.");

            if (x % 64 != 0)
                throw new ArgumentOutOfRangeException(nameof(x), x, "Not a multiple of 64.");

            if (y % 256 != 0)
                throw new ArgumentOutOfRangeException(nameof(x), y, "Not a multiple of 256.");

            if (!Enum.IsDefined(typeof(TexturePageAlpha), alpha))
                throw new InvalidEnumArgumentException(nameof(alpha), (int)alpha, typeof(TexturePageAlpha));

            if (!Enum.IsDefined(typeof(TexturePageColors), colors))
                throw new InvalidEnumArgumentException(nameof(colors), (int)colors, typeof(TexturePageColors));

            if (!Enum.IsDefined(typeof(TexturePageDisable), disable))
                throw new InvalidEnumArgumentException(nameof(disable), (int)disable, typeof(TexturePageDisable));

            X       = x;
            Y       = y;
            Alpha   = alpha;
            Colors  = colors;
            Disable = disable;

            var xMax = 1024 - GetWidth(this);

            if (x > xMax)
                throw new ArgumentOutOfRangeException(nameof(x), $"Must not be greater than {xMax}.");

            var yMax = 256;

            if (y > yMax)
                throw new ArgumentOutOfRangeException(nameof(y), $"Must not be greater than {yMax}.");
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Colors)}: {Colors}";
        }

        public static int GetIndex(TexturePage page)
        {
            return page.Y % 256 * 16 + page.X % 64;
        }

        public static int GetWidth(TexturePage page)
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
    }
}