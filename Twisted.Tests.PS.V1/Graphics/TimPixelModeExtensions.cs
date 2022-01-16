using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Twisted.Tests.PS.V1.Graphics;

public static class TimPixelModeExtensions
{
    [SuppressMessage("Style",     "IDE0066:Convert switch statement to expression", Justification = "<Pending>")]
    [SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
    public static int GetClutWidth(this TimPixelMode pixelMode)
    {
        switch (pixelMode)
        {
            case TimPixelMode.Indexed4:
                return 16;
            case TimPixelMode.Indexed8:
                return 256;
            case TimPixelMode.Direct15:
                throw new InvalidDataException();
            case TimPixelMode.Direct24:
                throw new InvalidDataException();
            case TimPixelMode.Mixed:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(pixelMode), pixelMode, null);
        }
    }
}