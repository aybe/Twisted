using System.Numerics;
using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal sealed class Polygon04010B0C : PolygonQuad
{
    private readonly byte[] Bytes;

    public Polygon04010B0C(BinaryReader reader, int positionVertices, int positionNormals) : base(reader, positionVertices, 4)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Bytes = reader.ReadBytes(32);

        var normal1 = Bytes.ReadUInt16(24, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(26, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(28, Endianness.LE);
        var normal4 = Bytes.ReadUInt16(30, Endianness.LE);

        Normals = ReadNormals(reader, positionNormals, normal1, normal2, normal3, normal4);
    }

    public Vector4[] Normals { get; }
}