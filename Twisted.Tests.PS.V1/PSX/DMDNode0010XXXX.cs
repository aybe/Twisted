using System;

namespace Twisted.Tests.PS.V1.PSX;

public sealed class DMDNode0010XXXX : DMDNode
{
    public DMDNode0010XXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        dmd.ReadInt32LE();

        dmd.Read(dmd.ReadInt16BE, 11); // vectors ?
    }
}