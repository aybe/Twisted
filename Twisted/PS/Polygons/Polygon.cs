using System.Numerics;
using Twisted.Extensions;

namespace Twisted.PS.Polygons;

internal abstract class Polygon : IPolygon
{
    protected Polygon(BinaryReader reader)
    {
        Reader   = reader ?? throw new ArgumentNullException(nameof(reader));
        Position = reader.BaseStream.Position;
        Type     = reader.ReadUInt32(Endianness.BigEndian);
    }

    private BinaryReader Reader { get; }

    public uint Type { get; }

    public long Position { get; }

    public abstract IReadOnlyList<int> Indices { get; }

    public override string ToString()
    {
        return $"{nameof(Type)}: 0x{Type:X8} @ {Position}";
    }

    protected short[] ReadIndices(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var indices = new short[count];

        for (var i = 0; i < count; i++)
        {
            indices[i] = Reader.ReadInt16(Endianness.LittleEndian);
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