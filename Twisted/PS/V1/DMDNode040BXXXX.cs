using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V1;

[Obsolete(null, true)]
public sealed class DMDNode040BXXXX : DMDNode
{
    public DMDNode040BXXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        Assert.AreEqual(0x040B0000, dmd.ReadInt32BE() & 0xFFFF0000);
        dmd.ReadInt32LE();
        dmd.ReadInt32LE();
        dmd.ReadInt32LE();

        var (byte1, byte2, byte3, byte4) = dmd.ReadInt32LE();

        var addresses = byte3;

        DMDNodeReader.Read(dmd, this, addresses);
    }
}