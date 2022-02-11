using System;
using System.IO;

namespace Twisted.PS.Texturing
{
    public static class TimExtensions
    {
        public static void WriteTga(this Tim tim, Stream stream, FrameBufferObject? palette = null, bool translucency = false)
        {
            if (tim is null)
                throw new ArgumentNullException(nameof(tim));

            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var picture = tim.Picture;

            if (picture is not null && picture.Format != tim.Format)
            {
                throw new ArgumentOutOfRangeException(nameof(picture), "TIM picture format does not match TIM format.");
            }

            if (picture is not null)
            {
                if (tim.Format is FrameBufferObjectFormat.Indexed4 or FrameBufferObjectFormat.Indexed8)
                {
                    palette ??= tim.Palettes?[0] ?? throw new ArgumentNullException(nameof(palette));
                }

                FrameBufferObject.WriteTga(stream, picture, palette, translucency);
            }
            else
            {
                if (palette is not null)
                {
                    FrameBufferObject.WriteTga(stream, palette, null, translucency);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(tim), "TIM neither has a picture nor a palette.");
                }
            }
        }
    }
}