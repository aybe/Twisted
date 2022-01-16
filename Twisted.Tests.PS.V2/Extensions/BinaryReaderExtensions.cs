using System;
using System.IO;
using JetBrains.Annotations;

namespace Twisted.Tests.PS.V2.Extensions;

public static class BinaryReaderExtensions
{
    [PublicAPI]
    [NotNull]
    public static T Peek<T>(this BinaryReader reader, Func<BinaryReader, T> func)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (func == null)
            throw new ArgumentNullException(nameof(func));

        var position = reader.BaseStream.Position;

        var value = func(reader);

        reader.BaseStream.Position = position;

        return value;
    }
}