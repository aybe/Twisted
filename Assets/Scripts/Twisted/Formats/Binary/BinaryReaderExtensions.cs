using System;
using System.IO;
using System.Text;

namespace Twisted.Formats.Binary
{
    public static class BinaryReaderExtensions
    {
        public static string ReadStringAscii(this BinaryReader reader, int length)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var bytes = reader.ReadBytes(length);
            if (bytes.Length != length)
                throw new EndOfStreamException();

            var ascii = Encoding.ASCII.GetString(bytes);

            return ascii;
        }

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

        public static T[] ReadObjects<T>(this BinaryReader reader, Func<BinaryReader, T> func, int count)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var items = new T[count];

            for (var i = 0; i < count; i++)
            {
                items[i] = func(reader);
            }

            return items;
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