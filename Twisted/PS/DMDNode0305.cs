using System;
using System.IO;

namespace Twisted.PS
{
    public sealed class DMDNode0305 : DMDNode
    {
        public DMDNode0305(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var unknown2 = reader.ReadBytes(12);

            var count = unknown2[7];

            var addresses = ReadAddresses(reader, count);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}