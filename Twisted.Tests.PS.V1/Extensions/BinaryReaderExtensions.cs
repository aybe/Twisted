using System;
using System.IO;
using System.Text;

namespace Twisted.Tests.PS.V1.Extensions;

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

    public static T[] Read<T>(this BinaryReader reader, Func<BinaryReader, T> func, int count)
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