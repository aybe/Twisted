﻿namespace Twisted.PS.Polygons;

internal sealed class Polygon84010B09 : Polygon
{
    private byte[] Bytes;

    public Polygon84010B09(BinaryReader reader, int positionVertices)
        : base(reader, positionVertices, 4, polygonSize: 44, polygonFaces: 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;
        Bytes = reader.ReadBytes(32);
    }
}