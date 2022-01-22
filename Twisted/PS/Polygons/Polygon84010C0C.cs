using System.Numerics;
using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal sealed class Polygon84010C0C : Polygon
{
    private readonly byte[] Bytes;

    public Polygon84010C0C(BinaryReader reader, int vertices, int positionNormals)
        : base(reader, vertices, 4, positionNormals:positionNormals, polygonSize: 48, polygonFaces: 4, normalsOffset: 40)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        return;
        Bytes = reader.ReadBytes(36);

        var normal1 = Bytes.ReadUInt16(28, Endianness.LE);
        var normal2 = Bytes.ReadUInt16(30, Endianness.LE);
        var normal3 = Bytes.ReadUInt16(32, Endianness.LE);
        var normal4 = Bytes.ReadUInt16(34, Endianness.LE);

        Normals = ReadNormals(reader, positionNormals, normal1, normal2, normal3, normal4);

        // TODO tex stuff ? 4 bytes
        // TODO ...
        // TODO tex stuff E2 ? 4 bytes
        // TODO normals ? 8 bytes
    }

    public Vector4[] Normals { get; }
}