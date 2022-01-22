using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal abstract class Polygon : IPolygon
{
    protected Polygon(
        BinaryReader reader,
        int positionVertices = -1,
        int countVertices = -1,
        int positionNormals = -1,
        int countNormals = -1,
        int polygonSize = -1
    )
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (polygonSize != -1 && polygonSize < 12)
            throw new ArgumentOutOfRangeException(nameof(polygonSize), polygonSize, "Polygon size should be at least 12 bytes.");

        Position = reader.BaseStream.Position;
        Type     = reader.ReadUInt32(Endianness.BigEndian);

        if (polygonSize != -1)
        {
            throw new NotImplementedException();
        }

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

    public uint Type { get; }

    public IReadOnlyList<int> Indices { get; }

    public long Position { get; }

    public override string ToString()
    {
        return $"{nameof(Type)}: 0x{Type:X8} @ {Position}, {nameof(Indices)}: {string.Join(", ", Indices)}";
    }

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
}