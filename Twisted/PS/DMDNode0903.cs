using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        // Assert.AreEqual(0, b1);

        Assert.AreEqual(0, b2);
        
        Assert.AreNotEqual(0, b3);

        Assert.AreEqual(0, b4);

        var addresses = ReadAddresses(reader, b3);

        SetupBinaryObject(reader);

        ReadNodes(this, reader, addresses);
    }
}