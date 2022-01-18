using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V1;

[Obsolete(null, true)]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class DMD
{
    public DMD(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        Reader = new BinaryReader(stream);

        var magic = Reader.ReadStringAscii(4);
        if (magic != "DXSP")
            throw new InvalidDataException($"Invalid identifier: {magic}.");

        var version = Reader.ReadInt32();
        if (version != 0x00000043)
            throw new InvalidDataException($"Invalid version: 0x{version:X8}.");

        var time = DateTime.UnixEpoch.AddSeconds(Reader.ReadInt32());

        BaseAddress = Reader.ReadUInt32();

        var table = ReadAddress();

        Reader.BaseStream.Position = table;

        var count = ReadInt32LE();

        var addresses = Read(ReadAddress, count);

        Trace.WriteLine("Addresses:");

        foreach (var address in addresses)
        {
            Trace.WriteLine(address.ToString());
        }

        var root = new DMDNodeROOT(this, null);

        foreach (var address in addresses)
        {
            stream.Position = address;
            DMDNodeReader.Read(this, root);
        }
    }

    private BinaryReader Reader { get; }

    [PublicAPI]
    public DMDPointer BaseAddress { get; }

    public long Position
    {
        get => Reader.BaseStream.Position;
        set => Reader.BaseStream.Position = value;
    }

    public uint ReadAddress()
    {
        var position = Position;

        var address1 = Reader.ReadUInt32();

        Assert.IsTrue(IsAddress(address1), $"Not an address at {position} : 0x{address1:X8}");

        var address2 = address1 - BaseAddress;

        // NOTE these can be outside file !?

        // Assert.IsTrue(address2 < Reader.BaseStream.Length, $"Invalid address at {position}");

        return address2;
    }

    public uint[] ReadAddresses(int count)
    {
        return Reader.Read(s => ReadAddress(), count);
    }

    public short ReadInt16BE()
    {
        return BitConverter.ToInt16(BitConverter.GetBytes(Reader.ReadInt16()).Reverse().ToArray());
    }

    public short ReadInt16LE()
    {
        return Reader.ReadInt16();
    }

    public int ReadInt32BE()
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(Reader.ReadInt32()).Reverse().ToArray());
    }

    public int ReadInt32LE()
    {
        return Reader.ReadInt32();
    }

    public uint ReadUInt32BE()
    {
        return BitConverter.ToUInt32(BitConverter.GetBytes(Reader.ReadUInt32()).Reverse().ToArray());
    }

    public T Peek<T>(Func<T> func)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));

        var position = Reader.BaseStream.Position;

        var value = func();

        Reader.BaseStream.Position = position;

        return value;
    }

    public bool IsAddress(uint value)
    {
        return value >= BaseAddress;
    }

    public T[] Read<T>(Func<T> func, int count)
    {
        var items = Reader.Read(s => func(), count);

        return items;
    }
}