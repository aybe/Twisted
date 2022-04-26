using System;
using JetBrains.Annotations;

namespace Twisted.Formats.Graphics2D
{
    [Flags]
    [PublicAPI]
    public enum TransparentColorMode
    {
        /// <summary>
        ///     Disable semi-transparency processing.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Enable semi-transparency processing for color (required).
        /// </summary>
        Color = 1 << 0,

        /// <summary>
        ///     Enable semi-transparency processing for black (optional).
        /// </summary>
        Black = 1 << 1
    }
}