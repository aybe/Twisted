namespace Twisted.PS;

public sealed class DMDNode0903 : DMDNode
{
    public DMDNode0903(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        var addresses = ReadAddresses(reader, b3);

        SetLength(reader);

        ReadNodes(this, reader, addresses);
    }
}