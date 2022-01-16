using System;
using System.IO;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

internal sealed class Polygon00010605 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon00010605(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(12);
    }
}