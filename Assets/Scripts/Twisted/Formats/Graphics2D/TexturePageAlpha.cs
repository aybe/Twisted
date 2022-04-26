using JetBrains.Annotations;

namespace Twisted.Formats.Graphics2D
{
    [PublicAPI]
    public enum TexturePageAlpha
    {
        HalfBackPlusHalfFront = 0,
        BackPlusFront         = 1,
        BackMinusFront        = 2,
        BackPlusQuarterFront  = 3
    }
}