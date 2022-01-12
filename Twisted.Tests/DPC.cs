using System;
using System.IO;

namespace Twisted.Tests;

public sealed class DPC
{
    public DPC(Stream stream)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        Reader = new BinaryReader(stream);

        if (Reader.ReadInt32(Endianness.BE) is not 0x4443504D)
            throw new InvalidDataException("Unknown header.");

        if (Reader.ReadInt32(Endianness.LE) is not 0x00000043)
            throw new InvalidDataException("Unknown version.");

        DateTimeOffset.FromUnixTimeSeconds(Reader.ReadInt32(Endianness.LE));

        Offset = Reader.ReadInt32(Endianness.LE);
    }

    private int Offset { get; }

    private BinaryReader Reader { get; }

    public int ReadPosition(bool validate = true)
    {
        var position1 = Reader.ReadInt32(Endianness.LE);
        var position2 = position1 - Offset;

        if (validate && (position2 < 0 || position2 >= Reader.BaseStream.Length))
        {
            throw new InvalidDataException();
        }

        return position2;
    }
}