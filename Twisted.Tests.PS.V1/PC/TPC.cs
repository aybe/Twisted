using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;
using Twisted.Tests.PS.V1.Extensions;
using Twisted.Tests.PS.V1.Graphics;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Twisted.Tests.PS.V1.PC;

public sealed class TPC
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public TPC(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        using var reader = new BinaryReader(stream, Encoding.Default, true);

        var magic = reader.ReadStringAscii(4);
        if (magic != "TCPM")
            throw new InvalidDataException("Not a TPC file.");

        var version = reader.ReadInt32();
        if (version != 0x00000043)
            throw new InvalidDataException($"Invalid version: 0x{version:X8}");

        Time = DateTime.UnixEpoch.AddSeconds(reader.ReadInt32());

        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var       bytes = reader.ReadBytes(reader.ReadInt32());
            using var ms    = new MemoryStream(bytes);
            new Tim(ms);
        }

        Assert.AreEqual(stream.Length, stream.Position);
    }

    public DateTime Time { get; }
}