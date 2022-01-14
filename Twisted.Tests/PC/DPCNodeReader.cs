using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

internal sealed class DPCNodeReader
{
    public DPCNodeReader(BinaryReader reader)
    {
        Reader = reader;
    }

    public BinaryReader Reader { get; }

    public long Position
    {
        get => Reader.BaseStream.Position;
        set => Reader.BaseStream.Position = value;
    }

    public T Peek<T>(Func<BinaryReader, T> func)
    {
        if (func is null)
            throw new ArgumentNullException(nameof(func));

        var peek = Reader.Peek(func);

        return peek;
    }

    public int ReadAddress(bool validate = true)
    {
        const int baseAddress = unchecked((int)0x800188B8);

        var address1 = Reader.ReadInt32(Endianness.LE);
        var address2 = address1 - baseAddress;

        if (validate)
        {
            Assert.IsTrue(
                address2 >= 0 && address2 < Reader.BaseStream.Length,
                $"Invalid address @ {Position - sizeof(int)}: 0x{address2:X8}."
            );
        }

        return address2;
    }

    public int[] ReadAddresses(int count, bool validate = true)
    {
        var addresses = new int[count];

        for (var i = 0; i < count; i++)
        {
            addresses[i] = ReadAddress(validate);
        }

        return addresses;
    }

    public byte ReadByte()
    {
        return Reader.ReadByte();
    }

    public byte[] ReadBytes(int count)
    {
        return Reader.ReadBytes(count);
    }

    public int ReadInt32(Endianness endianness)
    {
        return Reader.ReadInt32(endianness);
    }
}