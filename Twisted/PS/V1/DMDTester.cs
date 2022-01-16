using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V1;

public static class DMDTester
{
    public static void Test(TestContext context, string path)
    {
        using var stream = new MemoryStream(File.ReadAllBytes(path));

        var dmd = new DMD(stream);

        context.WriteLine(dmd.BaseAddress.ToString());
    }
}