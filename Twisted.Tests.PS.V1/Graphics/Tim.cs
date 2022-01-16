using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Twisted.Tests.PS.V1.Graphics;

[SuppressMessage("ReSharper", "IdentifierTypo")]
public sealed class Tim
{
    public Tim(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        stream.Position = 0;

        using var reader = new BinaryReader(stream, Encoding.Default, true);

        var id = reader.ReadByte();
        if (id != 0x10)
            throw new InvalidDataException($"Invalid identifier: 0x{id:X2}.");

        var version = reader.ReadByte();
        if (version != 0x00)
            throw new InvalidDataException($"Invalid version: 0x{version:X2}.");

        reader.ReadInt16(); // reserved

        var flag = reader.ReadInt32();

        PixelMode = (TimPixelMode)(flag & 0b111);

        ClutSection = ((flag >> 3) & 0b1) != 0;

        if (ClutSection)
        {
            ClutBlock = new TimClutBlock(reader, PixelMode);
        }
    }


    public TimPixelMode PixelMode { get; }

    public bool ClutSection { get; }

    [CanBeNull]
    public TimClutBlock ClutBlock { get; }
}