namespace Unity.PlayStation.Graphics
{
    public static class TransparentColorModeExtensions
    {
        public static bool HasFlagFast(this TransparentColorMode value, TransparentColorMode flag)
        {
            return (value & flag) != TransparentColorMode.None;
        }
    }
}