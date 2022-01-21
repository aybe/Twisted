﻿namespace Twisted.PS.V2.Polygons;

internal sealed class Polygon84010B09 : PolygonQuad
{
    private byte[] Bytes;

    public Polygon84010B09(BinaryReader reader, int positionVertices) : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(32);
    }
}