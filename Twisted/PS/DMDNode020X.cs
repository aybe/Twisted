using System;
using System.IO;

namespace Twisted.PS
{
    public sealed class DMDNode020X : DMDNode
    {
        public DMDNode020X(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(16);

            // TODO int32 x, y, z

            var count = bytes[12];

            var addresses = ReadAddresses(reader, count);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}