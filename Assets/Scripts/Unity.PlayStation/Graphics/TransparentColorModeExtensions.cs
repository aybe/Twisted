using JetBrains.Annotations;

namespace Unity.PlayStation.Graphics
{
    [PublicAPI]
    public static class TransparentColorModeExtensions
    {
        public static bool HasFlagFast(this TransparentColorMode value, TransparentColorMode flag)
        {
            return (value & flag) != TransparentColorMode.None;
        }
    }
}