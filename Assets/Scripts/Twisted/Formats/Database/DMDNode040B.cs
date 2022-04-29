using System;
using System.IO;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode040B : DMDNode
    {
        public DMDNode040B(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(16);

            // TODO int32 x, y, z ???
            Assert.AreEqual(default, bytes[13]);

            var count = bytes[14];

            var addresses = ReadAddresses(reader, count);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}