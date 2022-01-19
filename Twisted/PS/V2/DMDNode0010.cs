namespace Twisted.PS.V2;

public sealed class DMDNode0010 : DMDNode
{
    public DMDNode0010(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        // var unknown2 = reader.ReadBytes(44); // BUG this doesn't makes sense at all
    }
}