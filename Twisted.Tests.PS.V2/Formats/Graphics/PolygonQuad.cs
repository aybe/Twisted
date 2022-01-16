using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

internal abstract class PolygonQuad : Polygon
{
    protected PolygonQuad(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var indices = ReadIndices(4);

        Indices = indices.Select(s => (int)s).ToArray();
    }

    public override IReadOnlyList<int> Indices { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(Indices)}: {string.Join(", ", Indices)}";
    }
}