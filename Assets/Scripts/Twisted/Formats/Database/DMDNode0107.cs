using System;
using System.IO;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode0107 : DMDNode
    {
        public DMDNode0107(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(20);

            Assert.AreEqual((byte)0, bytes[19]);

            // TODO int32 x, y, z, unknown

            var count = bytes[16];

            var addresses = ReadAddresses(reader, count);

            // TODO there may be 76 more bytes in here

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}