namespace Twisted.Extensions;

public sealed class TypeComparer : Comparer<Type>
{
    public static TypeComparer Instance { get; } = new();

    public override int Compare(Type? x, Type? y)
    {
        if (x == null && y == null)
            return default;

        if (x == null)
            return int.MinValue;

        if (y == null)
            return int.MaxValue;

        var compare = string.Compare(x.FullName, y.FullName, StringComparison.Ordinal);

        return compare;
    }
}