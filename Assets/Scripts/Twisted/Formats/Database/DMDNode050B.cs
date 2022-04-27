using System;
using System.IO;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode050B : DMDNode
    {
        private readonly byte Unknown1;

        private readonly byte Unknown2;

        public DMDNode050B(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(36);

            // TODO 32 bytes matrix ???

            Unknown1 = bytes[32];

            Assert.AreEqual((byte)0, bytes[33]);

            var count = bytes[34];

            Unknown2 = bytes[35];

            var addresses = ReadAddresses(reader, count);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
        public IEnumerable<object> GetUnknowns()
        {
            yield return Unknown1;
            yield return Unknown2;
        }
    }
}