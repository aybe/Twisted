using System;

namespace Twisted.PS.Texturing
{
    public readonly struct FrameBufferRect // TODO?
    {
        public readonly int X;

        public readonly int Y;

        public readonly int Width;

        public readonly int Height;

        public readonly FrameBufferFormat Format;

        public FrameBufferRect(int x, int y, int width, int height, FrameBufferFormat format)
        {
            X      = x;
            Y      = y;
            Width  = width;
            Height = height;
            Format = format;
        }

        public int GetRenderWidth()
        {
            return Format switch
            {
                FrameBufferFormat.Indexed4 => Width / 4,
                FrameBufferFormat.Indexed8 => Width / 2,
                FrameBufferFormat.Mixed    => Width,
                FrameBufferFormat.Direct15 => Width,
                FrameBufferFormat.Direct24 => Width / 3 * 2,
                _                                => throw new ArgumentOutOfRangeException(nameof(Format), Format, null)
            };
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(Format)}: {Format}";
        }
    }
}