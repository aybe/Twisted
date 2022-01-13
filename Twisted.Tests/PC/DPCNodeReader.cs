using System.IO;

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

    public int ReadAddress()
    {
        return Reader.ReadInt32(Endianness.LE) - unchecked((int)0x800188B8);
    }

    public int[] ReadAddresses(int count)
    {
        var addresses = new int[count];

        for (var i = 0; i < count; i++)
        {
            addresses[i] = ReadAddress();
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