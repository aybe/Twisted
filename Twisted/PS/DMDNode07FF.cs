using System;
using System.IO;

namespace Twisted.PS
{
    public sealed class DMDNode07FF : DMDNode
    {
        public DMDNode07FF(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(52);

            // TODO matrix ???

            SetupBinaryObject(reader);
        }
    }
}