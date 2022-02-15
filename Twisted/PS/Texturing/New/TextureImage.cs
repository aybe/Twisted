using System;
using System.Collections.Generic;
using Twisted.Extensions;

namespace Twisted.PS.Texturing.New
{
    public sealed class TextureImage
    {
        public TextureImage(int pixelWidth, int pixelHeight, TransparentColor[] pixelData)
        {
            if (pixelWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(pixelWidth));

            if (pixelHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(pixelHeight));

            if (pixelData is null)
                throw new ArgumentNullException(nameof(pixelData));

            if (pixelData.Length != pixelWidth * pixelHeight)
                throw new ArgumentOutOfRangeException(nameof(pixelData));

            PixelWidth  = pixelWidth;
            PixelHeight = pixelHeight;
            PixelData   = pixelData.AsReadOnly();
        }

        public int PixelWidth { get; }

        public int PixelHeight { get; }

        public IReadOnlyList<TransparentColor> PixelData { get; }
    }
}