using JetBrains.Annotations;

namespace Unity.PlayStation.Graphics
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