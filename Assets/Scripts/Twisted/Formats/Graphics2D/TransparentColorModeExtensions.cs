using JetBrains.Annotations;

namespace Twisted.Formats.Graphics2D
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