using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS;

public sealed class DMDNode050B : DMDNode
{
    public DMDNode050B(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var bytes = reader.ReadBytes(36);

        // TODO 32 bytes matrix ???

        // Assert.AreEqual(0, bytes[32]);
        
        Assert.AreEqual(0, bytes[33]);

        var count = bytes[34];

        // Assert.AreEqual(0, bytes[35]);

        var addresses = ReadAddresses(reader, count);

        SetupBinaryObject(reader);

        ReadNodes(this, reader, addresses);
    }
}