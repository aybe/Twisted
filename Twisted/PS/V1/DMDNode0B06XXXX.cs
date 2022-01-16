using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V1;

public sealed class DMDNode0B06XXXX : DMDNode
{
    public DMDNode0B06XXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        Assert.IsTrue(parent is DMDNode0107XXXX);

        Assert.AreEqual(0x0B060000, dmd.ReadInt32BE() & 0xFFFF0000);

        dmd.ReadInt32LE();

        var (byte1, byte2, byte3, byte4) = dmd.ReadInt32LE();

        switch (byte3)
        {
            case 1:
                DMDNodeReader.Read(dmd, this, 3);
                break;
            case 2:
                DMDNodeReader.Read(dmd, this, 4);
                break;
            default:
                throw new NotSupportedException($"{nameof(byte3)}: {byte3} at {Position}");
        }
    }
}