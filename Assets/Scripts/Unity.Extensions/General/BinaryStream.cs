using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Unity.Extensions.General
{
    public partial class BinaryStream : Stream
    {
        public BinaryStream(Stream input, Encoding? encoding = default, bool leaveOpen = default)
        {
            Stream = input ?? throw new ArgumentNullException(nameof(input));

            if (Stream.CanRead)
            {
                StreamReader = new BinaryReader(this, encoding ?? Encoding.Default);
            }

            if (Stream.CanWrite)
            {
                StreamWriter = new BinaryWriter(this, encoding ?? Encoding.Default);
            }

            StreamDispose = leaveOpen is false;
        }

        private SortedSet<BinaryStreamRegion> RegionsReading { get; } = new SortedSet<BinaryStreamRegion>();

        private SortedSet<BinaryStreamRegion> RegionsWriting { get; } = new SortedSet<BinaryStreamRegion>();

        private Stream Stream { get; }

        private bool StreamDispose { get; }

        private BinaryReader? StreamReader { get; }

        private BinaryWriter? StreamWriter { get; }

        public override bool CanRead => Stream.CanRead;

        public override bool CanSeek => Stream.CanSeek;

        public override bool CanWrite => Stream.CanWrite;

        public override long Length => Stream.Length;

        public override long Position
        {
            get => Stream.Position;
            set => Stream.Position = value;
        }

        private bool IsDisposed { get; set; }

        public Endianness? Endianness { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (StreamDispose)
                {
                    Stream.Dispose();
                }
            }

            base.Dispose(disposing);

            IsDisposed = true;
        }

        public override void Flush()
        {
            Stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var position = Position;

            if (position >= Length)
            {
                throw new EndOfStreamException();
            }

            var read = Stream.Read(buffer, offset, count);

            RegionsReading.Add(new BinaryStreamRegion(position, read));

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var seek = Stream.Seek(offset, origin);

            return seek;
        }

        public override void SetLength(long value)
        {
            Stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var position = Position;

            Stream.Write(buffer, offset, count);

            RegionsWriting.Add(new BinaryStreamRegion(position, count));
        }

        public IEnumerable<BinaryStreamRegion> GetRegions(BinaryStreamRegionKind kind, BinaryStreamRegionType type)
        {
            IEnumerable<BinaryStreamRegion> regions = kind switch
            {
                BinaryStreamRegionKind.Reading => RegionsReading,
                BinaryStreamRegionKind.Writing => RegionsWriting,
                _                              => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };

            regions = type switch
            {
                BinaryStreamRegionType.Ignored => GetRegionsIgnored(regions, Length),
                BinaryStreamRegionType.Visited => GetRegionsVisited(regions),
                _                              => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return regions;
        }

        private static IEnumerable<BinaryStreamRegion> GetRegionsIgnored(IEnumerable<BinaryStreamRegion> regions, long length)
        {
            regions = GetRegionsVisited(regions); // first, merge the regions

            using var enumerator = regions.GetEnumerator();

            if (!enumerator.MoveNext()) // empty sequence, negate
            {
                yield return new BinaryStreamRegion(0, length);
                yield break;
            }

            var region = enumerator.Current;

            if (region.Position > 0) // gap at start
            {
                yield return new BinaryStreamRegion(0, region.Position);
            }

            while (enumerator.MoveNext())
            {
                var pos = region.Position + region.Length;
                var len = enumerator.Current.Position - pos;

                if (len > 0) // gap at middle
                {
                    yield return new BinaryStreamRegion(pos, len);
                }

                region = enumerator.Current;
            }

            {
                var pos = region.Position + region.Length;
                var len = length - pos;

                if (pos < length) // gap at end
                {
                    yield return new BinaryStreamRegion(pos, len);
                }
            }
        }

        private static IEnumerable<BinaryStreamRegion> GetRegionsVisited(IEnumerable<BinaryStreamRegion> regions)
        {
            using var enumerator = regions.GetEnumerator();

            if (!enumerator.MoveNext())
                yield break;

            var origin = enumerator.Current.Position;
            var ending = enumerator.Current.Position + enumerator.Current.Length;

            while (enumerator.MoveNext())
            {
                var pos = enumerator.Current.Position;
                var len = enumerator.Current.Length;

                if (pos > ending) // gap
                {
                    yield return new BinaryStreamRegion(origin, ending - origin);
                    origin = pos;
                    ending = pos + len;
                }
                else
                {
                    ending = Math.Max(pos + len, ending);
                }
            }

            yield return new BinaryStreamRegion(origin, ending - origin);
        }
    }

    public partial class BinaryStream
    {
        private const string TheStreamIsNotReadable = "The stream is not readable.";

        public T ReadEnum<T>() where T : Enum, IConvertible
        {
            return ReadEnum<T>(Endianness ?? EndiannessExtensions.Endianness);
        }

        public T ReadEnum<T>(Endianness endianness) where T : Enum, IConvertible
        {
            if (StreamReader is null)
            {
                throw new InvalidOperationException(TheStreamIsNotReadable);
            }

            return StreamReader.ReadEnum<T>(endianness);
        }

        public short ReadInt16()
        {
            return ReadInt16(Endianness ?? EndiannessExtensions.Endianness);
        }

        public short ReadInt16(Endianness endianness)
        {
            if (StreamReader is null)
            {
                throw new InvalidOperationException(TheStreamIsNotReadable);
            }

            return StreamReader.ReadInt16(endianness);
        }

        public int ReadInt32()
        {
            return ReadInt32(Endianness ?? EndiannessExtensions.Endianness);
        }

        public int ReadInt32(Endianness endianness)
        {
            if (StreamReader is null)
            {
                throw new InvalidOperationException(TheStreamIsNotReadable);
            }

            return StreamReader.ReadInt32(endianness);
        }

        public long ReadInt64()
        {
            return ReadInt64(Endianness ?? EndiannessExtensions.Endianness);
        }

        public long ReadInt64(Endianness endianness)
        {
            if (StreamReader is null)
            {
                throw new InvalidOperationException(TheStreamIsNotReadable);
            }

            return StreamReader.ReadInt64(endianness);
        }

        public ushort ReadUInt16()
        {
            return ReadUInt16(Endianness ?? EndiannessExtensions.Endianness);
        }

        public ushort ReadUInt16(Endianness endianness)
        {
            if (StreamReader is null)
            {
                throw new InvalidOperationException(TheStreamIsNotReadable);
            }

            return StreamReader.ReadUInt16(endianness);
        }

        public uint ReadUInt32()
        {
            return ReadUInt32(Endianness ?? EndiannessExtensions.Endianness);
        }

        public uint ReadUInt32(Endianness endianness)
        {
            if (StreamReader is null)
            {
                throw new InvalidOperationException(TheStreamIsNotReadable);
            }

            return StreamReader.ReadUInt32(endianness);
        }

        public ulong ReadUInt64()
        {
            return ReadUInt64(Endianness ?? EndiannessExtensions.Endianness);
        }

        public ulong ReadUInt64(Endianness endianness)
        {
            if (StreamReader is null)
            {
                throw new InvalidOperationException(TheStreamIsNotReadable);
            }

            return StreamReader.ReadUInt64(endianness);
        }
    }

    public partial class BinaryStream
    {
        private const string TheStreamIsNotWritable = "The stream is not writable.";

        public void WriteEnum<T>(T value) where T : Enum, IConvertible
        {
            WriteEnum(value, Endianness ?? EndiannessExtensions.Endianness);
        }

        public void WriteEnum<T>(T value, Endianness endianness) where T : Enum, IConvertible
        {
            if (StreamWriter is null)
            {
                throw new InvalidOperationException(TheStreamIsNotWritable);
            }

            StreamWriter.WriteEnum(value, endianness);
        }

        public void Write(short value)
        {
            Write(value, Endianness ?? EndiannessExtensions.Endianness);
        }

        public void Write(short value, Endianness endianness)
        {
            if (StreamWriter is null)
            {
                throw new InvalidOperationException(TheStreamIsNotWritable);
            }

            StreamWriter.Write(value, endianness);
        }

        public void Write(int value)
        {
            Write(value, Endianness ?? EndiannessExtensions.Endianness);
        }

        public void Write(int value, Endianness endianness)
        {
            if (StreamWriter is null)
            {
                throw new InvalidOperationException(TheStreamIsNotWritable);
            }

            StreamWriter.Write(value, endianness);
        }

        public void Write(long value)
        {
            Write(value, Endianness ?? EndiannessExtensions.Endianness);
        }

        public void Write(long value, Endianness endianness)
        {
            if (StreamWriter is null)
            {
                throw new InvalidOperationException(TheStreamIsNotWritable);
            }

            StreamWriter.Write(value, endianness);
        }

        public void Write(ushort value)
        {
            Write(value, Endianness ?? EndiannessExtensions.Endianness);
        }

        public void Write(ushort value, Endianness endianness)
        {
            if (StreamWriter is null)
            {
                throw new InvalidOperationException(TheStreamIsNotWritable);
            }

            StreamWriter.Write(value, endianness);
        }

        public void Write(uint value)
        {
            Write(value, Endianness ?? EndiannessExtensions.Endianness);
        }

        public void Write(uint value, Endianness endianness)
        {
            if (StreamWriter is null)
            {
                throw new InvalidOperationException(TheStreamIsNotWritable);
            }

            StreamWriter.Write(value, endianness);
        }

        public void Write(ulong value)
        {
            Write(value, Endianness ?? EndiannessExtensions.Endianness);
        }

        public void Write(ulong value, Endianness endianness)
        {
            if (StreamWriter is null)
            {
                throw new InvalidOperationException(TheStreamIsNotWritable);
            }

            StreamWriter.Write(value, endianness);
        }
    }
}