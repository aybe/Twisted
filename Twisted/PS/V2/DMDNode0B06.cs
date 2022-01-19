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

        var count = unknown2[6];

        switch (count)
        {
            case 1:
                count = 3;
                break;
            case 2:
                count = 4;
                break;
            default:
                throw new InvalidDataException(Position.ToString());
        }

        var addresses = ReadAddresses(reader, count);

        SetLength(reader);

        ReadNodes(this, reader, addresses);
    }
}