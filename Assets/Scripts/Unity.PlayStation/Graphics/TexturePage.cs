using System;
using System.ComponentModel;
using UnityEngine;

namespace Unity.PlayStation.Graphics
{
    public readonly struct TexturePage
    {
        public Vector2Int Position { get; }

        public TexturePageAlpha Alpha { get; }

        public TexturePageColors Colors { get; }

        public TexturePageDisable Disable { get; }

        public TexturePage(Vector2Int position, TexturePageAlpha alpha, TexturePageColors colors, TexturePageDisable disable)
        {
            if (position.x < 0)
                throw new ArgumentOutOfRangeException(nameof(position.x), "Must be positive.");

            if (position.y < 0)
                throw new ArgumentOutOfRangeException(nameof(position.y), "Must be positive.");

            if (position.x % 64 != 0)
                throw new ArgumentOutOfRangeException(nameof(position.x), position.x, "Not a multiple of 64.");

            if (position.y % 256 != 0)
                throw new ArgumentOutOfRangeException(nameof(position.x), position.y, "Not a multiple of 256.");

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

            var xMax = 1024 - GetWidth(this);

            if (position.x > xMax)
                throw new ArgumentOutOfRangeException(nameof(position.x), $"Must not be greater than {xMax}.");

            var yMax = 256;

            if (position.y > yMax)
                throw new ArgumentOutOfRangeException(nameof(position.y), $"Must not be greater than {yMax}.");
        }

        public override string ToString()
        {
            return $"{nameof(Position)}: {Position}, {nameof(Colors)}: {Colors}";
        }

        public static int GetIndex(TexturePage page)
        {
            return page.Position.y % 256 * 16 + page.Position.x % 64;
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