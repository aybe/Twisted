using System.Collections.Immutable;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal abstract class Polygon : IPolygon
{
    protected Polygon( // TODO reorder signature
        BinaryReader reader,
        int positionVertices = -1,
        int countVertices = -1, // TODO useless, remove
        int positionNormals = -1, // TODO rename to normalsPosition
        int polygonSize = -1,
        int polygonFaces = -1,
        int normalsOffset = -1
    )
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (polygonSize != -1 && polygonSize < 12)
            throw new ArgumentOutOfRangeException(nameof(polygonSize), polygonSize, "Polygon size should be at least 12 bytes.");

        Position = reader.BaseStream.Position;

        // TODO this is only to silent warnings, delete these once porting is complete
        Data = Array.Empty<byte>();
#pragma warning disable CS0612 // Type or member is obsolete
        Indices = Array.Empty<int>();
#pragma warning restore CS0612 // Type or member is obsolete
        Vertices = Array.Empty<Vector4>();
        Normals  = Array.Empty<Vector4>();

        // TODO delete
        Assert.AreEqual(countVertices, polygonFaces);

        if (polygonSize != -1)
        {
            switch (polygonFaces)
            {
                case 3:
                case 4:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(polygonFaces), polygonFaces, "Polygon faces should be 3 or 4.");
            }

            Data = reader.ReadBytes(polygonSize);

            // read polygon type

            Type = Data.ReadUInt32(0, Endianness.BE);

            // read polygon vertices

            if (positionVertices != -1) // TODO this should always be true
            {
                Assert.IsFalse(countVertices <= 0, "countVertices <= 0"); // TODO delete

                var indices = new int[4]; // read everything

                for (var i = 0; i < 4; i++)
                {
                    indices[i] = Data.ReadUInt16(4 + i * sizeof(ushort), Endianness.LE);
                }

                if (polygonFaces == 3) // trivial check to assert we're good
                {
                    Assert.AreEqual(0, indices[3], "Triangle expected.");
                }

                var position = reader.BaseStream.Position; // TODO replace by scope

                {
                    var vertices = new Vector4[4]; // read everything

                    for (var i = 0; i < 4; i++) // TODO this logic should be reusable
                    {
                        var j = indices[i];
                        reader.BaseStream.Position = positionVertices + j * 8;
                        var v = ReadVector4Int16(reader, Endianness.LE);
                        vertices[i] = v;
                    }

                    Vertices = vertices.Take(polygonFaces).ToImmutableArray(); // only take what we need
                }

                reader.BaseStream.Position = position;
            }
            // ReSharper disable once RedundantIfElseBlock
            else
            {
                // TODO assert fail
                Vertices = Array.Empty<Vector4>();
            }

            // read polygon normals if any

            if (positionNormals != -1)
            {
                if (normalsOffset < 12)
                {
                    throw new ArgumentOutOfRangeException(nameof(normalsOffset), normalsOffset, "Polygon normals offset should be at least 12.");
                }

                if (normalsOffset - polygonSize > -8)
                {
                    throw new ArgumentOutOfRangeException(nameof(normalsOffset), normalsOffset,"Polygon normals offset points outside polygon.");
                }

                var indices = new int[4];

                for (var i = 0; i < 4; i++)
                {
                    indices[i] = Data.ReadUInt16(normalsOffset + i * sizeof(ushort), Endianness.LE);
                }

                var position = reader.BaseStream.Position; // TODO replace by scope

                {
                    var normals = new Vector4[4]; // read everything

                    for (var i = 0; i < 4; i++)
                    {
                        var j = indices[i];
                        reader.BaseStream.Position = positionNormals + j * 8;
                        var v = ReadVector4Int16(reader, Endianness.LE);
                        normals[i] = v;
                    }

                    Normals = normals.Take(polygonFaces).ToImmutableArray(); // only take what we need
                }

                reader.BaseStream.Position = position;
            }
            else
            {
                Normals = Array.Empty<Vector4>();
            }
        }
        // ReSharper disable once RedundantIfElseBlock
        else // TODO delete
        {
            Type = reader.ReadUInt32(Endianness.BigEndian);

            var indices = ReadIndices(reader, 4);

            Indices = indices.Select(s => (int)s).ToArray();

            if (positionVertices != -1)
            {
                Assert.IsFalse(countVertices <= 0, "countVertices <= 0");

                var position = reader.BaseStream.Position;

                foreach (var index in indices)
                {
                    reader.BaseStream.Position = positionVertices + index * 8;
                    var vertex = reader.ReadBytes(8);
                }

                reader.BaseStream.Position = position;
            }
            // ReSharper disable once RedundantIfElseBlock
            else
            {
                // TODO assert fail
            }
        }
    }

    protected byte[] Data { get; }

    public uint Type { get; }

    public IReadOnlyList<Vector4> Vertices { get; }

    public IReadOnlyList<Vector4> Normals { get; }

    [Obsolete]
    public IReadOnlyList<int> Indices { get; }

    public long Position { get; }

    private static short[] ReadIndices(BinaryReader reader, int count)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var indices = new short[count];

        for (var i = 0; i < count; i++)
        {
            indices[i] = reader.ReadInt16(Endianness.LittleEndian);
        }

        return indices;
    }

    [Obsolete("replace by ReadVector4Int16")]
    protected static Vector4[] ReadNormals(BinaryReader reader, int position, params ushort[] indices)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (position < 0 || position >= reader.BaseStream.Length)
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position is out of bounds.");

        if (indices == null)
            throw new ArgumentNullException(nameof(indices));

        using var scope = new BinaryReaderPositionScope(reader);

        var normals = new Vector4[indices.Length];

        for (var i = 0; i < indices.Length; i++)
        {
            var index = indices[i];

            reader.BaseStream.Position = position + index * 8;

            var x = reader.ReadInt16(Endianness.LE);
            var y = reader.ReadInt16(Endianness.LE);
            var z = reader.ReadInt16(Endianness.LE);
            var w = reader.ReadInt16(Endianness.LE);

            var v = new Vector4(x, y, z, w);

            normals[i] = v;
        }

        return normals;
    }

    private static Vector4 ReadVector4Int16(BinaryReader reader, Endianness endianness)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var x = reader.ReadInt16(endianness);
        var y = reader.ReadInt16(endianness);
        var z = reader.ReadInt16(endianness);
        var w = reader.ReadInt16(endianness);
        var v = new Vector4(x, y, z, w);

        return v;
    }

    public override string ToString()
    {
        return $"{nameof(Type)}: 0x{Type:X8} @ {Position}, {nameof(Indices)}: {string.Join(", ", Indices)}";
    }
}