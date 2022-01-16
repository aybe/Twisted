using System;
using System.IO;
using Twisted.Tests.PS.V2.Extensions;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

public sealed class Tim
{
    public Tim(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        using var stream = new MemoryStream(bytes);
        using var reader = new BinaryReader(stream);

        var id = reader.ReadByte();

        if (id != 0x10)
        {
            throw new InvalidDataException($"Invalid identifier: 0x{id:X2}");
        }

        var version = reader.ReadByte();

        if (version != 0x00)
        {
            throw new InvalidDataException($"Invalid version: 0x{id:X2}");
        }

        reader.ReadInt16(); // reserved

        var flag = reader.ReadInt32(Endianness.LittleEndian);

        var pixelMode = (TimPixelMode)(flag & 0b111);

        if (!Enum.IsDefined(typeof(TimPixelMode), pixelMode))
            throw new InvalidDataException($"Invalid pixel mode: {pixelMode}");

        // clut
        if ((flag & 0b1000) != 0)
        {
            var bnum = reader.ReadInt32(Endianness.LittleEndian);
            var dx   = reader.ReadInt16(Endianness.LittleEndian);
            var dy   = reader.ReadInt16(Endianness.LittleEndian);
            var w    = reader.ReadInt16(Endianness.LittleEndian);
            var h    = reader.ReadInt16(Endianness.LittleEndian);
            var data = reader.ReadBytes(bnum - 12);
        }

        // pixel data
        {
            var bnum = reader.ReadInt32(Endianness.LittleEndian);
            var dx   = reader.ReadInt16(Endianness.LittleEndian);
            var dy   = reader.ReadInt16(Endianness.LittleEndian);
            var w    = reader.ReadInt16(Endianness.LittleEndian);
            var h    = reader.ReadInt16(Endianness.LittleEndian);
            var data = reader.ReadBytes(bnum - 12);
        }
    }
}