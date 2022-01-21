﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon03010A09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon03010A09(BinaryReader reader, int positionVertices) : base(reader, positionVertices, 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(28);
    }
}