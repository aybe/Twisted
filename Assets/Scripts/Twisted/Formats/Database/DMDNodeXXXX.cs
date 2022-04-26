using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Twisted.Formats.Binary;

namespace Twisted.Formats.Database
{
    public sealed class DMDNodeXXXX : DMDNode
    {
        [SuppressMessage("ReSharper", "UnusedVariable")]
        [SuppressMessage("Style",     "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
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