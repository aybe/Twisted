using System;
using System.Collections.Generic;

namespace Twisted.PS.Texturing
{
    internal class Comparers
    {
        /// <summary>
        ///     Compares by page position and colors.
        /// </summary>
        public sealed class TexturePageEqualityComparer : IEqualityComparer<TexturePage>
        {
            public static IEqualityComparer<TexturePage> Instance { get; } = new TexturePageEqualityComparer();

            public bool Equals(TexturePage x, TexturePage y)
            {
                return x.X == y.X && x.Y == y.Y && x.Colors == y.Colors;
            }

            public int GetHashCode(TexturePage obj)
            {
                return HashCode.Combine(obj.X, obj.Y, (int)obj.Colors);
            }
        }

        /// <summary>
        ///     Compares by page position and colors.
        /// </summary>
        public sealed class TexturePageComparer : IComparer<TexturePage>
        {
            public static IComparer<TexturePage> Instance { get; } = new TexturePageComparer();

            public int Compare(TexturePage x, TexturePage y)
            {
                var xComparison = x.X.CompareTo(y.X);
                if (xComparison != 0)
                    return xComparison;

                var yComparison = x.Y.CompareTo(y.Y);
                if (yComparison != 0)
                    return yComparison;

                return x.Colors.CompareTo(y.Colors);
            }
        }
    }
}