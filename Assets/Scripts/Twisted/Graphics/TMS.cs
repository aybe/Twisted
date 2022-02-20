using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using JetBrains.Annotations;
using Unity.Extensions.Binary;
using Unity.PlayStation.Graphics;

namespace Twisted.Graphics
{
    public sealed class TMS : IReadOnlyList<Tim>
    {
        public TMS([NotNull] BinaryReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var identifier = reader.ReadInt32(Endianness.LE);

            if (identifier != 0x50535854)
            {
                throw new InvalidDataException($"Invalid identifier: 0x{identifier:X8}.");
            }

            var version = reader.ReadInt32(Endianness.LE);

            if (version != 0x00000043)
            {
                throw new InvalidDataException($"Invalid version: 0x{version:X8}.");
            }

            DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt32(Endianness.LE));

            var count = reader.ReadInt32(Endianness.LE);

            var tims = new Tim[count];

            for (var i = 0; i < count; i++)
            {
                var length = reader.ReadInt32(Endianness.LE);
                var bytes  = reader.ReadBytes(length);

                using var ms = new MemoryStream(bytes);

                var tim = new Tim(ms);

                tims[i] = tim;
            }

            Tims = new ReadOnlyCollection<Tim>(tims);
        }

        private IReadOnlyList<Tim> Tims { get; }

        public IEnumerator<Tim> GetEnumerator()
        {
            return Tims.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Tims).GetEnumerator();
        }

        public int Count => Tims.Count;

        public Tim this[int index] => Tims[index];
    }
}