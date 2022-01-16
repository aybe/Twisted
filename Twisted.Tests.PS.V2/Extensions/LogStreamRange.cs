using System;

namespace Twisted.Tests.PS.V2.Extensions;

public readonly struct LogStreamRange : IEquatable<LogStreamRange>, IComparable<LogStreamRange>, IComparable
{
    public long Start { get; }

    public long End { get; }

    public long Length => End - Start;

    public LogStreamRange(long start, long end)
    {
        Start = start;
        End   = end;
    }

    public override string ToString()
    {
        return $"{nameof(Start)}: {Start}, {nameof(End)}: {End}, {nameof(Length)}: {Length}";
    }

    #region Equality members

    public bool Equals(LogStreamRange other)
    {
        return Start == other.Start && End == other.End;
    }

    public override bool Equals(object obj)
    {
        return obj is LogStreamRange other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Start.GetHashCode() * 397) ^ End.GetHashCode();
        }
    }

    public static bool operator ==(LogStreamRange left, LogStreamRange right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LogStreamRange left, LogStreamRange right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Relational members

    public int CompareTo(LogStreamRange other)
    {
        var startComparison = Start.CompareTo(other.Start);
        if (startComparison != 0)
            return startComparison;

        return End.CompareTo(other.End);
    }

    public int CompareTo(object obj)
    {
        if (ReferenceEquals(null, obj))
            return 1;

        return obj is LogStreamRange other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(LogStreamRange)}");
    }

    public static bool operator <(LogStreamRange left, LogStreamRange right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(LogStreamRange left, LogStreamRange right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(LogStreamRange left, LogStreamRange right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(LogStreamRange left, LogStreamRange right)
    {
        return left.CompareTo(right) >= 0;
    }

    #endregion
}