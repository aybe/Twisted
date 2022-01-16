﻿using System;

namespace Twisted.Tests.PS.V1.PSX;

public sealed class DMDNode08FFXXXX : DMDNode
{
    public DMDNode08FFXXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        var code = dmd.ReadInt32BE();

        dmd.Read(dmd.ReadInt32LE, 20); // ?

        // NOTE this is some position or something, it's read multiple times in a file
    }
}