using JetBrains.Annotations;
using Twisted.Extensions;

namespace Twisted.PS.V2;

public sealed class DMD : DMDNode
{
    public DMD(BinaryReader reader) : base(null, reader, 0xBEEF)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        reader.BaseStream.Position = 0;

        var magic = reader.ReadInt32(Endianness.LE);

        if (magic != 0x50535844)
            throw new InvalidDataException("Not a DMD file.");

        var version = reader.ReadInt32(Endianness.LE);
        if (version != 0x00000043)
            throw new NotSupportedException($"Version not supported: 0x{version:X8}.");

        Time = DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt32(Endianness.LE)).ToLocalTime();

        var @base = reader.ReadUInt32(Endianness.LE);
        if (@base != 0x800188B8)
            throw new InvalidDataException($"Invalid base address: 0x{@base:X8}.");

        var fat = ReadAddress(reader);

        reader.BaseStream.Position = fat;

        ReadAddressesThenNodes(reader, reader.ReadInt32(Endianness.LE));
    }

    [PublicAPI]
    public DateTimeOffset Time { get; }
}