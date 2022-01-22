namespace Twisted.PS;

public sealed class DMDNode0107 : DMDNode
{
    public DMDNode0107(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadBytes(20);

        var count = unknown2[16];

        var addresses = ReadAddresses(reader, count);

        // TODO there may be 76 more bytes in here

        SetLength(reader);

        ReadNodes(this, reader, addresses);
    }
}