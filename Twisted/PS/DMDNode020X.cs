namespace Twisted.PS;

public sealed class DMDNode020X : DMDNode
{
    public DMDNode020X(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadBytes(16);

        var count = unknown2[12];

        var addresses = ReadAddresses(reader, count);

        SetupBinaryObject(reader);

        ReadNodes(this, reader, addresses);
    }
}