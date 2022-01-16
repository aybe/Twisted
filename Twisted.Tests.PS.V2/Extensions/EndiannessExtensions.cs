using System;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Twisted.Tests.PS.V2.Extensions;

public static class EndiannessExtensions
{
    [PublicAPI]
    public static Endianness Endianness { get; } = BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;

    [PublicAPI]
    public static DateTimeOffset ReadUnixTime(this BinaryReader reader, Endianness endianness)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var seconds = reader.ReadInt32(endianness);
        var offset  = DateTimeOffset.FromUnixTimeSeconds(seconds);

        return offset;
    }

    [PublicAPI]
    public static short ReadInt16(this BinaryReader reader, Endianness endianness)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadInt16();

        if (IsEquivalent(endianness))
            return value;

        value = ReverseEndianness(value);

        return value;
    }

    [PublicAPI]
    public static ushort ReadUInt16(this BinaryReader reader, Endianness endianness)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadUInt16();

        if (IsEquivalent(endianness))
            return value;

        value = ReverseEndianness(value);

        return value;
    }

    [PublicAPI]
    public static int ReadInt32(this BinaryReader reader, Endianness endianness)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadInt32();

        if (IsEquivalent(endianness))
            return value;

        value = ReverseEndianness(value);

        return value;
    }

    [PublicAPI]
    public static uint ReadUInt32(this BinaryReader reader, Endianness endianness)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadUInt32();

        if (IsEquivalent(endianness))
            return value;

        value = ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEquivalent(Endianness endianness)
    {
        return endianness == Endianness || endianness == Endianness.Native;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static short ReverseEndianness(in short value)
    {
        var a = (byte)((value >> 08) & 0xFF);
        var b = (byte)((value >> 00) & 0xFF);
        var c = (short)((b << 08) | (a << 00));

        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort ReverseEndianness(in ushort value)
    {
        var a = (byte)((value >> 08) & 0xFF);
        var b = (byte)((value >> 00) & 0xFF);
        var c = (ushort)((b << 08) | (a << 00));

        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ReverseEndianness(in int value)
    {
        var a = (byte)((value >> 24) & 0xFF);
        var b = (byte)((value >> 16) & 0xFF);
        var c = (byte)((value >> 08) & 0xFF);
        var d = (byte)((value >> 00) & 0xFF);
        var e = (d << 24) | (c << 16) | (b << 08) | (a << 00);

        return e;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ReverseEndianness(in uint value)
    {
        var a = (byte)((value >> 24) & 0xFF);
        var b = (byte)((value >> 16) & 0xFF);
        var c = (byte)((value >> 08) & 0xFF);
        var d = (byte)((value >> 00) & 0xFF);
        var e = (uint)((d << 24) | (c << 16) | (b << 08) | (a << 00));

        return e;
    }
}