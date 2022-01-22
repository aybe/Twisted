﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.Polygons;

internal sealed class Polygon03010807 : Polygon
{
    private byte[] Bytes;

    public Polygon03010807(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 3, polygonSize: 32, polygonFaces: 3)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;
        Assert.AreEqual(0, Indices[3], "Triangle expected.");

        Bytes = reader.ReadBytes(20);
    }
}