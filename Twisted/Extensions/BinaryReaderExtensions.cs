using System;
using System.IO;
using System.Text;

namespace Twisted.Extensions
{
    public static class BinaryReaderExtensions
    {
        /// <summary>
        ///     Gets if this instance can read the specified number of bytes from current position.
        /// </summary>
        /// <param name="reader">
        ///     The source reader.
        /// </param>
        /// <param name="count">
        ///     The number of bytes.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified number of bytes can be read from current position, otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="reader" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="count" /> is negative.
        /// </exception>
        public static bool CanRead(this BinaryReader reader, int count)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return reader.BaseStream.Length - reader.BaseStream.Position >= count;
        }

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
    }
}