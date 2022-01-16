using Twisted.Extensions;

namespace Twisted.PS.V2.Polygons;

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
}