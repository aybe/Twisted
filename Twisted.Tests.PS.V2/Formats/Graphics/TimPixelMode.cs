using JetBrains.Annotations;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

[PublicAPI]
public enum TimPixelMode
{
    Indexed4 = 0,
    Indexed8 = 1,
    Direct15 = 2,
    Direct24 = 3,
    Mixed    = 4
}