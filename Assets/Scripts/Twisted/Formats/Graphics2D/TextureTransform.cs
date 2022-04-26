using System;

namespace Twisted.Formats.Graphics2D
{
    [Flags]
    public enum TextureTransform
    {
        None = 0,

        FlipX = 1 << 0,

        FlipY = 1 << 1,

        Rotate90 = 1 << 2,

        Rotate180 = 1 << 3,

        Rotate270 = 1 << 4
    }
}