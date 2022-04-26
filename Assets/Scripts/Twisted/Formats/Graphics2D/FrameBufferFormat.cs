using JetBrains.Annotations;

namespace Twisted.Formats.Graphics2D
{
    /// <summary>
    ///     Specifies the pixel format for a frame buffer object.
    /// </summary>
    [PublicAPI]
    public enum FrameBufferFormat
    {
        Indexed4,
        Indexed8,
        Direct15,
        Direct24
    }
}