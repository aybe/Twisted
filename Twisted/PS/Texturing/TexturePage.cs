namespace Twisted.PS.Texturing
{
    public readonly struct TexturePage
    {
        public int X { get; }

        public int Y { get; }

        public TexturePageAlpha Alpha { get; }

        public TexturePageColors Colors { get; }

        public TexturePageDisable Disable { get; }

        public TexturePage(int x, int y, TexturePageAlpha alpha, TexturePageColors colors, TexturePageDisable disable)
        {
            X       = x;
            Y       = y;
            Alpha   = alpha;
            Colors  = colors;
            Disable = disable;
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Colors)}: {Colors}";
        }
    }
}