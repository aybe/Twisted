using Twisted.Extensions;

namespace Twisted.PS;

public sealed class DMDNode050B : DMDNode
{
    public DMDNode050B(DMDNode? parent, BinaryReader reader)
        : base(parent, reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var unknown2 = reader.ReadBytes(36);

        var x = unknown2.ReadInt32(20, Endianness.LE); // TODO
        var y = unknown2.ReadInt32(24, Endianness.LE); // TODO
        var z = unknown2.ReadInt32(28, Endianness.LE); // TODO

        var countMaybe = unknown2[34]; // TODO

        var addresses = ReadAddresses(reader, countMaybe);

        SetLength(reader);

        ReadNodes(this, reader, addresses);
    }
}