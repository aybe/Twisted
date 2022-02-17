using System;

namespace Unity.Extensions.General
{
    public static class IntegerExtensions // TODO implement other primitives
    {
        public static byte GetNibble(this short value, int index)
        {
            if (index is < 0 or > 3)
                throw new ArgumentOutOfRangeException(nameof(index));

            return (byte)((value >> (index * 4)) & 0xF);
        }

        public static byte GetByte(this short value, int index)
        {
            if (index is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(index));

            return (byte)((value >> (index * 8)) & 0xFF);
        }
    }
}