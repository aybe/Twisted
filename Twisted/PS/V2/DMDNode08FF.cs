namespace Twisted.PS.V2;

public sealed class DMDNode08FF : DMDNode
{
    public DMDNode08FF(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadBytes(84);

        SetLength(reader);
    }
}