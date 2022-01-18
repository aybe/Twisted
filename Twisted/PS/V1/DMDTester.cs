using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V1;

public static class DMDTester
{
    public static void Test(TestContext context, Stream stream)
    {
        var dmd = new DMD(stream);

        context.WriteLine(dmd.BaseAddress.ToString());
    }
}