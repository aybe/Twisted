using Twisted.Extensions;

namespace Twisted.PS.V2;

public sealed class DMDNode0107 : DMDNode
{
    public DMDNode0107(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown1 = reader.ReadInt16(Endianness.LittleEndian);
        var unknown2 = reader.ReadBytes(20);

        var count = unknown2[16];
        ReadAddressesThenNodes(reader, count);
    }
}