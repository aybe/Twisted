namespace Twisted.Formats.Graphics2D
{
    public static class TextureTransformExtensions
    {
        public static bool HasFlagFast(this TextureTransform value, TextureTransform flag)
        {
            return (value & flag) != TextureTransform.None;
        }
    }
}