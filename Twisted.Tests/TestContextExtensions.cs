using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests;

public static class TestContextExtensions
{
    public static void WriteLine(this TestContext context)
    {
        context.WriteLine(string.Empty);
    }
}