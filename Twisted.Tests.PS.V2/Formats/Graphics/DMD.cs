using System;
using System.IO;
using JetBrains.Annotations;
using Twisted.Tests.PS.V2.Extensions;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

public sealed class DMD : DMDNode
{
    public DMD(BinaryReader reader) : base(null, reader, 0xBEEF)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        reader.BaseStream.Position = 0;

        var magic = reader.ReadInt32(Endianness.LittleEndian);

        if (magic != 0x50535844) // PSXD
            throw new InvalidDataException("Not a DMD file.");

        var version = reader.ReadInt32(Endianness.LittleEndian);
        if (version != 0x00000043)
            throw new NotSupportedException($"Version not supported: 0x{version:X8}.");

        Time = reader.ReadUnixTime(Endianness.LittleEndian).ToLocalTime();

        var @base = reader.ReadUInt32(Endianness.LittleEndian);
        if (@base != 0x800188B8)
            throw new InvalidDataException($"Invalid base address: 0x{@base:X8}.");

        var fat = ReadAddress(reader);

        reader.BaseStream.Position = fat;

        ReadAddressesThenNodes(reader, reader.ReadInt32(Endianness.LittleEndian));
    }

    [PublicAPI]
    public DateTimeOffset Time { get; }
}