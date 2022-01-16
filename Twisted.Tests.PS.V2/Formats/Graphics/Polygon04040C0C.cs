﻿using System;
using System.IO;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

internal sealed class Polygon04040C0C : PolygonQuad
{
    private byte[] Bytes;

    public Polygon04040C0C(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(36);
    }
}