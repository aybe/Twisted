using System;
using System.Diagnostics;

namespace Twisted.Tests.PS.V1.PSX;

[DebuggerDisplay("0x{Value.ToString(\"X8\"),nq}")]
public struct DMDPointer : IEquatable<DMDPointer>
{
    private DMDPointer(in uint value)
    {
        Value = value;
    }

    private uint Value { get; }

    public static implicit operator uint(DMDPointer pointer)
    {
        return pointer.Value;
    }

    public static implicit operator DMDPointer(uint value)
    {
        return new DMDPointer(value);
    }

    public bool Equals(DMDPointer other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        return obj is DMDPointer other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (int)Value;
    }

    public static bool operator ==(DMDPointer left, DMDPointer right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DMDPointer left, DMDPointer right)
    {
        return !left.Equals(right);
    }
}