namespace Twisted.PS;

public sealed class DMDNode07FF : DMDNode
{
    public DMDNode07FF(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadBytes(52);

        SetupBinaryObject(reader);
    }
}