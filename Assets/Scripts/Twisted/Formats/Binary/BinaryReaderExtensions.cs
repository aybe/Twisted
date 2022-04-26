using System;
using System.IO;

namespace Twisted.Formats.Binary
{
    public static class BinaryReaderExtensions
    {
        public static T ReadObject<T>(this BinaryReader reader, Func<BinaryReader, T> func, long position)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            if (position < 0 || position >= reader.BaseStream.Length)
                throw new ArgumentOutOfRangeException(nameof(position));

            using var scope = new BinaryReaderPositionScope(reader, position);

            var value = func(reader);

            return value;
        }

        public static bool TryRead<T>(this BinaryReader reader, Func<BinaryReader, T> func, out T result)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            result = default!;

            var position = reader.BaseStream.Position;

            try
            {
                result = func(reader);
                return true;
            }
            catch (Exception)
            {
                reader.BaseStream.Position = position;
                return false;
            }
        }

        public static bool TryRead<T>(this BinaryReader reader, Func<BinaryReader, T> func, out T[] result, int count)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var position = reader.BaseStream.Position;

            try
            {
                result = new T[count];

                for (var i = 0; i < count; i++)
                {
                    result[i] = func(reader);
                }

                return true;
            }
            catch (Exception)
            {
                result = default!;

                reader.BaseStream.Position = position;

                return false;
            }
        }
    }
}