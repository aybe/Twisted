﻿using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;
using Twisted.IO;
using Twisted.PC;
using Twisted.PS.V1;

namespace Twisted.Tests;

public static class AutoGenerated
{
    public static void Test(TestContext context, string path)
    {
        using var stream = new BinaryStream(new MemoryStream(File.ReadAllBytes(path)));

        var extension = Path.GetExtension(path).ToUpperInvariant();

        switch (extension)
        {
            case ".DMD":
                DMDTester.Test(context, stream);
                break;
            case ".DPC":
                DPCTester.Test(context, stream);
                break;
            default: throw new NotSupportedException(extension);
        }
    }
}