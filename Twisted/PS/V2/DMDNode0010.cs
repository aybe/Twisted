using JetBrains.Annotations;
using Twisted.Extensions;

namespace Twisted.PS.V2;

public sealed class DMDNode0010 : DMDNode
{
    public DMDNode0010([CanBeNull] DMDNode parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown1 = reader.ReadInt16(Endianness.LittleEndian);
        var unknown2 = reader.ReadBytes(44);
    }
}