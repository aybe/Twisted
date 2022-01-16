using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Extensions;

public static class TestContextExtensions
{
    public static void WriteLine(this TestContext context)
    {
        context.WriteLine(string.Empty);
    }
}