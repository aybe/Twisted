using Twisted.Extensions;

namespace Twisted.PS.V2;

public sealed class DMDNode0903 : DMDNode
{
    public DMDNode0903(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown1 = reader.ReadInt16(Endianness.LittleEndian);
        var b1       = reader.ReadByte();
        var b2       = reader.ReadByte();
        var b3       = reader.ReadByte();
        var b4       = reader.ReadByte();

        ReadAddressesThenNodes(reader, b3);
    }
}