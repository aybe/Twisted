using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode07FF : DMDNode
    {
        [SuppressMessage("ReSharper", "UnusedVariable")]
        [SuppressMessage("Style",     "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
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