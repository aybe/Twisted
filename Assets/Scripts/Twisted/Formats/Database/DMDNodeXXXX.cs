using System;
using System.IO;
using Unity.Extensions.Binary;

namespace Twisted.Formats.Database
{
    public sealed class DMDNodeXXXX : DMDNode
    {
        public DMDNodeXXXX(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var unknown = reader.ReadInt32(Endianness.LE); // TODO this can be a multiple of 10

            var addresses = ReadAddresses(reader, 1);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);

            // TODO assert that children are 00FF or ???
        }
    }
}