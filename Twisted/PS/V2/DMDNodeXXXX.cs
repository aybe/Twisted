using Twisted.Extensions;

namespace Twisted.PS.V2;

public sealed class DMDNodeXXXX : DMDNode
{
    public DMDNodeXXXX(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown1 = reader.ReadInt16(Endianness.LittleEndian);

        var unknown2 = reader.ReadInt32(Endianness.LittleEndian);

        ReadAddressesThenNodes(reader, 1);

        // TODO assert that children are 00FF or ???
    }
}