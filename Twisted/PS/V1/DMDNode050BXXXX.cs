using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V1;

public sealed class DMDNode050BXXXX : DMDNode
{
    public DMDNode050BXXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        Assert.AreEqual(0x050B0000, dmd.ReadInt32BE() & 0xFFFF0000);

        dmd.ReadInt32LE();
        dmd.ReadInt32LE();
        dmd.ReadInt32LE();
        dmd.ReadInt32LE();
        dmd.ReadInt32LE();

        dmd.ReadInt32LE(); // x ?
        dmd.ReadInt32LE(); // y ?
        dmd.ReadInt32LE(); // z ?

        var (byte1, byte2, byte3, byte4) = dmd.ReadInt32LE();

        var addresses = byte3;

        DMDNodeReader.Read(dmd, this, addresses);
    }
}