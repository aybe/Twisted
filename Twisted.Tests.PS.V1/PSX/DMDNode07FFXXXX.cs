using System;

namespace Twisted.Tests.PS.V1.PSX;

public sealed class DMDNode07FFXXXX : DMDNode
{
    public DMDNode07FFXXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        var code = dmd.ReadInt32BE();

        dmd.Read(dmd.ReadInt32LE, 13); // vectors and normals ?
    }
}