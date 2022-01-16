using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PS.V1.Extensions;

public static class TestContextExtensions
{
    public static void WriteLine(this TestContext context, object value)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        context.WriteLine(value?.ToString());
    }
}