using System;
using System.IO;

namespace Twisted.PS
{
    public sealed class DMDNode08FF : DMDNode
    {
        public DMDNode08FF(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(84);

            SetupBinaryObject(reader);
        }
    }
}