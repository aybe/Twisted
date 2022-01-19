using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V2;

public sealed class DMDNode0B06 : DMDNode
{
    public DMDNode0B06(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        Assert.IsTrue(parent is DMDNode0107);

        var unknown2 = reader.ReadBytes(8);

        var countMaybe = unknown2[6]; // TODO if 1 then 3, if 2 then 4

        ReadAddressesThenNodes(reader, 3);
    }
}