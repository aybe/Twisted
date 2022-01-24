namespace Twisted.PS;

public sealed class DMDNode040B : DMDNode
{
    public DMDNode040B(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadBytes(16);

        var countMaybe = unknown2[14]; // TODO

        var addresses = ReadAddresses(reader, countMaybe);

        SetupBinaryObject(reader);

        ReadNodes(this, reader, addresses);
    }
}