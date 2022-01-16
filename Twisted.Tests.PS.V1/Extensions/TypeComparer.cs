using System;
using System.Collections.Generic;

namespace Twisted.Tests.PS.V1.Extensions;

public sealed class TypeComparer : Comparer<Type>
{
    public static TypeComparer Instance { get; } = new();

    public override int Compare(Type x, Type y)
    {
        if (x == null && y == null)
            return 0;

        if (x == null)
            return -1;

        if (y == null)
            return +1;

        var compare = string.Compare(x.Name, y.Name, StringComparison.Ordinal);

        return compare;
    }
}