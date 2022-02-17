using System;
using System.IO;

namespace Unity.Extensions.Binary
{
    public readonly struct BinaryReaderPositionScope : IDisposable
    {
        private BinaryReader Reader { get; }

        private long Position { get; }

        public BinaryReaderPositionScope(BinaryReader reader, long? position = default)
        {
            Reader   = reader ?? throw new ArgumentNullException(nameof(reader));
            Position = reader.BaseStream.Position;

            reader.BaseStream.Position = position ?? Position;
        }

        public void Dispose()
        {
            Reader.BaseStream.Position = Position;
        }
    }
}