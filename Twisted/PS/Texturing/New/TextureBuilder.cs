using System;
using System.Linq;

namespace Twisted.PS.Texturing.New
{
    public class TextureBuilder
    {
        private FrameBufferObject FrameBufferObject;

        public static void GetTexture(TexturePageFormatKey key, FrameBufferObject obj)
        {
            // BUG use already written types?
            // BUG use 16-bit units on framebuffer

            var pageX = key.Page.X;
            var pageY = key.Page.Y;

            var pageWidth = key.PageFormat switch
            {
                TexturePageFormat.Indexed4 => 64,
                TexturePageFormat.Indexed8 => 128,
                TexturePageFormat.Direct15 => 256,
                TexturePageFormat.Direct24 => throw new NotSupportedException(),
                _ => throw new NotSupportedException()
            };

            switch (key.PageFormat)
            {
                case TexturePageFormat.Indexed4:
                    // var bytes = obj.Pixels as byte[] ?? obj.Pixels.ToArray();
                    var shorts = new short[obj.Pixels.Count / 2];
                    var index = pageY * 1024 + pageX;


                    break;
                case TexturePageFormat.Indexed8:
                    break;
                case TexturePageFormat.Direct15:
                    break;
                case TexturePageFormat.Direct24:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}