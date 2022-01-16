using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Twisted.Tests.PS.V1.Graphics;

public struct TimClutEntry : IEquatable<TimClutEntry>
{
    public byte R { get; }

    public byte G { get; }

    public byte B { get; }

    public bool S { get; }

    [SuppressMessage("ReSharper", "ShiftExpressionRealShiftCountIsZero")]
    public TimClutEntry(BinaryReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var i = reader.ReadInt32();

        R = (byte)(((i >> 00) & 0b11111) * 255 / 31);
        G = (byte)(((i >> 05) & 0b11111) * 255 / 31);
        B = (byte)(((i >> 10) & 0b11111) * 255 / 31);

        S = ((i >> 15) & 0b1) != 0;
    }

    public override string ToString()
    {
        return $"{nameof(R)}: {R}, {nameof(G)}: {G}, {nameof(B)}: {B}, {nameof(S)}: {S}";
    }

    #region IEquatable<TimClutEntry> Members

    public bool Equals(TimClutEntry other)
    {
        return R == other.R && G == other.G && B == other.B && S == other.S;
    }

    public override bool Equals(object obj)
    {
        return obj is TimClutEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, S);
    }

    public static bool operator ==(TimClutEntry left, TimClutEntry right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TimClutEntry left, TimClutEntry right)
    {
        return !left.Equals(right);
    }

    #endregion
}