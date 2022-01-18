using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V1;

public static class DMDTester
{
    [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
    public static void Test(TestContext context, Stream stream)
    {
        if (false)
        {
            // var v1 = new DMD(stream);
        }
        else
        {
            var v2 = new V2.DMD(new BinaryReader(stream));
        }
    }
}