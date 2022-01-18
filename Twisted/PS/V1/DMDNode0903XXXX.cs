using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V1;

[Obsolete(null, true)]
public sealed class DMDNode0903XXXX : DMDNode
{
    public DMDNode0903XXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        Assert.AreEqual(0x0903, dmd.ReadInt16BE());

        var be = dmd.ReadInt16LE();

        var (byte1, byte2, byte3, byte4) = dmd.ReadInt32LE();

        DMDNodeReader.Read(dmd, this, byte3);
    }
}