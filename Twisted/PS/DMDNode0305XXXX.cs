using Twisted.Extensions;

namespace Twisted.PS;

public sealed class DMDNode0305XXXX : DMDNode
{
    public DMDNode0305XXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        dmd.ReadInt32BE();

        dmd.ReadInt32LE();

        var (byte1, byte2, byte3, byte4) = dmd.ReadInt32LE();

        dmd.ReadInt32LE();

        var count = byte4;

        DMDNodeReader.Read(dmd, this, count);
    }
}