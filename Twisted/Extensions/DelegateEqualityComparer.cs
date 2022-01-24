namespace Twisted.Extensions;

public sealed class DelegateEqualityComparer<T> : EqualityComparer<T>
{
    public DelegateEqualityComparer(Func<T?, T?, bool> equals, Func<T, int>? getHashCode = default)
    {
        EqualsFunc      = equals ?? throw new ArgumentNullException(nameof(equals));
        GetHashCodeFunc = getHashCode ?? (s => s?.GetHashCode() ?? default);
    }

    private Func<T?, T?, bool> EqualsFunc { get; }

    private Func<T, int> GetHashCodeFunc { get; }

    public override bool Equals(T? x, T? y)
    {
        return EqualsFunc(x, y);
    }

    public override int GetHashCode(T obj)
    {
        return GetHashCodeFunc(obj);
    }
}