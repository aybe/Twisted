using System.IO;

namespace Twisted.Tests.PC;

internal sealed class DPCNodeReader
{
    public DPCNodeReader(BinaryReader reader, int baseAddress)
    {
        Reader      = reader;
        BaseAddress = baseAddress;
    }

    public BinaryReader Reader { get; }

    public int BaseAddress { get; }

    public long Position
    {
        get => Reader.BaseStream.Position;
        set => Reader.BaseStream.Position = value;
    }

    public int ReadAddress()
    {
        return Reader.ReadInt32(Endianness.LE) - BaseAddress;
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
}