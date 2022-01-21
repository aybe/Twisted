﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon83010B09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon83010B09(BinaryReader reader) : base(reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.AreEqual(0, Indices[3]); // TODO this should be a triangle

        Bytes = reader.ReadBytes(32);
    }
}