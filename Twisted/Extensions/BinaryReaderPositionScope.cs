namespace Twisted.Extensions;

public readonly struct BinaryReaderPositionScope : IDisposable
{
    private BinaryReader Reader { get; }

    private long Position { get; }

    public BinaryReaderPositionScope(BinaryReader reader)
    {
        Reader   = reader ?? throw new ArgumentNullException(nameof(reader));
        Position = reader.BaseStream.Position;
    }

    public void Dispose()
    {
        Reader.BaseStream.Position = Position;
    }
}