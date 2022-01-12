using System;
using System.Buffers.Binary;
using System.Globalization;
using System.IO;

namespace Twisted.Tests;

public static class EndiannessExtensions
{
    public static Endianness Endianness { get; } =
        BitConverter.IsLittleEndian ? Endianness.LE : Endianness.BE;

    private static bool IsDifferent(this Endianness endianness)
    {
        return endianness != Endianness;
    }

    private static bool IsIdentical(this Endianness endianness)
    {
        return endianness == Endianness;
    }

    public static T Peek<T>(this BinaryReader reader, Func<BinaryReader, T> func)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        var position = reader.BaseStream.Position;

        var value = func(reader);

        reader.BaseStream.Position = position;

        return value;
    }

    public static T ReadEnum<T>(this BinaryReader reader, Endianness endianness) where T : Enum, IConvertible
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        var type = Enum.GetUnderlyingType(typeof(T));
        var code = Type.GetTypeCode(type);

        object value = code switch
        {
            TypeCode.SByte  => reader.ReadSByte(),
            TypeCode.Int16  => reader.ReadInt16(endianness),
            TypeCode.Int32  => reader.ReadInt32(endianness),
            TypeCode.Int64  => reader.ReadInt64(endianness),
            TypeCode.Byte   => reader.ReadByte(),
            TypeCode.UInt16 => reader.ReadUInt16(endianness),
            TypeCode.UInt32 => reader.ReadUInt32(endianness),
            TypeCode.UInt64 => reader.ReadUInt64(endianness),
            _               => throw new InvalidOperationException()
        };

        value = Convert.ChangeType(value, type);

        return (T)value;
    }

    public static short ReadInt16(this BinaryReader reader, Endianness endianness)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadInt16();

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        return value;
    }

    public static int ReadInt32(this BinaryReader reader, Endianness endianness)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadInt32();

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        return value;
    }

    public static long ReadInt64(this BinaryReader reader, Endianness endianness)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadInt64();

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        return value;
    }

    public static ushort ReadUInt16(this BinaryReader reader, Endianness endianness)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadUInt16();

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        return value;
    }

    public static uint ReadUInt32(this BinaryReader reader, Endianness endianness)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadUInt32();

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        return value;
    }

    public static ulong ReadUInt64(this BinaryReader reader, Endianness endianness)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        var value = reader.ReadUInt64();

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        return value;
    }

    public static void WriteEnum<T>(this BinaryWriter writer, T value, Endianness endianness)
        where T : Enum, IConvertible
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

        var type = Enum.GetUnderlyingType(typeof(T));
        var code = Type.GetTypeCode(type);
        var info = NumberFormatInfo.InvariantInfo;

        switch (code)
        {
            case TypeCode.SByte:
                writer.Write(value.ToSByte(info));
                break;
            case TypeCode.Int16:
                writer.Write(value.ToInt16(info), endianness);
                break;
            case TypeCode.Int32:
                writer.Write(value.ToInt32(info), endianness);
                break;
            case TypeCode.Int64:
                writer.Write(value.ToInt64(info), endianness);
                break;
            case TypeCode.Byte:
                writer.Write(value.ToByte(info));
                break;
            case TypeCode.UInt16:
                writer.Write(value.ToUInt16(info), endianness);
                break;
            case TypeCode.UInt32:
                writer.Write(value.ToUInt32(info), endianness);
                break;
            case TypeCode.UInt64:
                writer.Write(value.ToUInt64(info), endianness);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public static void Write(this BinaryWriter writer, short value, Endianness endianness)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, int value, Endianness endianness)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, long value, Endianness endianness)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, ushort value, Endianness endianness)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, uint value, Endianness endianness)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, ulong value, Endianness endianness)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));

        if (endianness.IsDifferent())
        {
            value = value.ReverseEndianness();
        }

        writer.Write(value);
    }

    public static short ReverseEndianness(this short value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static int ReverseEndianness(this int value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static long ReverseEndianness(this long value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static ushort ReverseEndianness(this ushort value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static uint ReverseEndianness(this uint value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static ulong ReverseEndianness(this ulong value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static short ToEndianness(this short value, Endianness endianness)
    {
        return endianness.IsIdentical() ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static int ToEndianness(this int value, Endianness endianness)
    {
        return endianness.IsIdentical() ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static long ToEndianness(this long value, Endianness endianness)
    {
        return endianness.IsIdentical() ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static ushort ToEndianness(this ushort value, Endianness endianness)
    {
        return endianness.IsIdentical() ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static uint ToEndianness(this uint value, Endianness endianness)
    {
        return endianness.IsIdentical() ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static ulong ToEndianness(this ulong value, Endianness endianness)
    {
        return endianness.IsIdentical() ? value : BinaryPrimitives.ReverseEndianness(value);
    }
}