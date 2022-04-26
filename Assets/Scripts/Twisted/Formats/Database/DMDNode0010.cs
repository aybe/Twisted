using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode0010 : DMDNode
    {
        [SuppressMessage("ReSharper", "UnusedVariable")]
        [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public DMDNode0010(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var addresses = default(uint[]?);

            var special = (NodeType & 0xFFFF) != 0;

            var bytes = reader.ReadBytes(special ? 4 : 44);

            if (special)
            {
                addresses = ReadAddresses(reader, 1);
            }

            SetupBinaryObject(reader);

            if (addresses != null)
            {
                ReadNodes(this, reader, addresses);
            }
        }
    }
}