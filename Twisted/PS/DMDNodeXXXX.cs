using Twisted.Extensions;

namespace Twisted.PS;

public sealed class DMDNodeXXXX : DMDNode
{
    public DMDNodeXXXX(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadInt32(Endianness.LE);

        var addresses = ReadAddresses(reader, 1);

        SetupBinaryObject(reader);

        ReadNodes(this, reader, addresses);

        // TODO assert that children are 00FF or ???
    }
}