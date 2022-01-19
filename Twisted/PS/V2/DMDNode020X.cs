using Twisted.Extensions;

namespace Twisted.PS.V2;

public sealed class DMDNode020X : DMDNode
{
    public DMDNode020X(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadBytes(12);

        var count = reader.ReadInt32(Endianness.LittleEndian);
        ReadAddressesThenNodes(reader, count);
    }
}