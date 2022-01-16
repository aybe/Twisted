using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS;

public sealed class DMDNode0107XXXX : DMDNode
{
    public DMDNode0107XXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        Assert.AreEqual(0x0107, dmd.ReadInt16BE());

        Unknown = dmd.ReadInt16LE();

        Ints = dmd.Read(dmd.ReadInt32LE, 4);

        var (byte1, byte2, byte3, byte4) = dmd.ReadInt32LE();

        var count = byte1;

        DMDNodeReader.Read(dmd, this, count);
    }

    public int[] Ints { get; }

    public short Unknown { get; }
}