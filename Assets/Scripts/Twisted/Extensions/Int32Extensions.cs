namespace Twisted.Extensions
{
    public static class Int32Extensions
    {
        public static void Deconstruct(this int value, out byte byte1, out byte byte2, out byte byte3, out byte byte4)
        {
            byte1 =   (byte)(value & 0xFF);
            value >>= 8;
            byte2 =   (byte)(value & 0xFF);
            value >>= 8;
            byte3 =   (byte)(value & 0xFF);
            value >>= 8;
            byte4 =   (byte)(value & 0xFF);
        }
    }
}