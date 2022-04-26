using System;
using System.IO;
using Twisted.Formats.Binary;

namespace Twisted.Formats.Database
{
    public sealed class DMD : DMDNode
    {
        public const uint BaseAddress = 0x800188B8;

        public DMD(BinaryReader reader)
            : base(null, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (NodeType != 0x44585350)
                throw new InvalidDataException($"Invalid identifier: 0x{NodeType:X8}.");

            var version = reader.ReadInt32(Endianness.LE);
            if (version != 0x00000043)
                throw new InvalidDataException($"Invalid version: 0x{version:X8}.");

            DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt32(Endianness.LE));

            // TODO the base address could be passed to base node and benefit both versions

            var baseAddress = reader.ReadUInt32(Endianness.LE);
            if (baseAddress != BaseAddress)
                throw new InvalidDataException($"Invalid base address: 0x{baseAddress:X8}.");

            reader.BaseStream.Position = ReadAddress(reader);

            var addressesCount = reader.ReadInt32(Endianness.LE);
            var addresses      = ReadAddresses(reader, addressesCount);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}