using Twisted.Extensions;

namespace Twisted.PS.V2;

public sealed class DMDNode050B : DMDNode
{
    public DMDNode050B(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown1 = reader.ReadInt16(Endianness.LittleEndian);
        var unknown2 = reader.ReadBytes(36);

        ReadAddressesThenNodes(reader, 1);
    }
}