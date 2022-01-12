﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests;

[TestClass]
public sealed partial class AutoGenerated
{
    [PublicAPI]
    public TestContext TestContext { get; set; } = null!;

    [SuppressMessage("ReSharper", "CommentTypo")]
    private void Test(string path)
    {
        using var stream = new BinaryStream(File.OpenRead(path));

        var reader = new BinaryReader(stream);

        var nodeReader = new DPCNodeReader(stream);

        // read table of contents

        nodeReader.Stream.Position = nodeReader.ReadAddress();

        var addresses = new int[nodeReader.Stream.ReadInt32(Endianness.LE)];

        for (var i = 0; i < addresses.Length; i++)
        {
            addresses[i] = nodeReader.ReadAddress();
        }

        foreach (var address in addresses)
        {
            //  Console.WriteLine(address);
        }
        // read the nodes iteratively

        var queue = new Queue<(DPCNode Parent, int Address)>();

        var root = new DPCNodeRoot(nodeReader);

        foreach (var address in addresses)
        {
            queue.Enqueue((root, address));
        }

        while (queue.Any())
        {
            var (parent, address) = queue.Dequeue();

            reader.BaseStream.Position = address;

            var nodeType = reader.Peek(s => s.ReadInt32(Endianness.BE));

            switch (nodeType)
            {
                case 0x_0107_0000:
                    //case 0x_0107_0100:
                    //case 0x_0107_0200:
                    //case 0x_0107_0300:
                    //case 0x_0107_0A00:
                    //case 0x_0107_1400:
                    //case 0x_0107_1E00:
                    //case 0x_0107_2800:
                    //case 0x_0107_2A00:
                    //case 0x_0107_3200:
                    //case 0x_0107_3C00:
                    //case 0x_0107_3801:
                    //case 0x_0107_4600:
                    //case 0x_0107_5000:
                    //case 0x_0107_5A00:
                    //case 0x_0107_6400:
                    //case 0x_0107_6E00:
                    //case 0x_0107_7800:
                    //case 0x_0107_8200:
                    //case 0x_0107_C900:
                    //case 0x_0107_EC04:
                    //case 0x_0107_EE02:
                    //case 0x_0107_F002:
                {
                    stream.Position += 20;

                    var b1 = reader.ReadByte();
                    var b2 = reader.ReadByte();
                    var b3 = reader.ReadByte();
                    var b4 = reader.ReadByte();

                    for (var i = 0; i < b1; i++)
                        queue.Enqueue((parent, nodeReader.ReadAddress()));

                    continue;
                }
                case 0x_00FF_0000:
                case 0x_00FF_0014:
                case 0x_00FF_0A14:
                case 0x_00FF_8813:
                case 0x_00FF_EC13:
                case 0x_00FF_F613:
                    //case 0x_00FF_5014:
                    //case 0x_00FF_5114:
                {
                    stream.Position += 4;

                    for (var i = 0; i < 3; i++)
                        nodeReader.ReadAddress(); // these aren't nodes, last is graphics

                    continue;
                }
                //case 0x_00FF_1405:
                //case 0x_00FF_1815:
                //case 0x_00FF_2D01:
                //case 0x_00FF_2E01:
                //case 0x_00FF_3401:
                //case 0x_00FF_3501:
                //case 0x_00FF_9002:
                //case 0x_00FF_A802:
                //case 0x_00FF_A902:
                //case 0x_00FF_AA02:
                //case 0x_00FF_AB02:
                //case 0x_00FF_B414:
                //case 0x_00FF_D200:
                //case 0x_00FF_D400:
                //case 0x_00FF_D500:
                //case 0x_00FF_D600:

                //case 0x_0107_0503:
                //case 0x_0107_0A03:
                //case 0x_0107_0D03:
                //case 0x_0107_F402:
                //case 0x_0107_F802:

                //case 0x_0206_0000:
                //case 0x_0207_0000:
                //case 0x_0208_0000:
                //case 0x_0209_0000:
                //case 0x_020A_0000:
                //{
                //    stream.Position += 16;

                //    var b1 = reader.ReadByte();
                //    var b2 = reader.ReadByte();
                //    var b3 = reader.ReadByte();
                //    var b4 = reader.ReadByte();

                //    for (var i = 0; i < b1; i++)
                //        queue.Enqueue((parent, nodeReader.ReadAddress()));

                //    continue;
                //}
                //case 0x_040B_0000:
                //{
                //    stream.Position += 16;

                //    var b1 = reader.ReadByte();
                //    var b2 = reader.ReadByte();
                //    var b3 = reader.ReadByte();
                //    var b4 = reader.ReadByte();

                //    for (var i = 0; i < b3; i++)
                //        queue.Enqueue((parent, nodeReader.ReadAddress()));

                //    continue;
                //}
                //case 0x_0B06_0000:
                //{
                //    stream.Position += 12;

                //    for (var i = 0; i < 3; i++)
                //        queue.Enqueue((parent, nodeReader.ReadAddress()));

                //    continue;
                //}
            }

            throw new NotImplementedException($"Not implemented node 0x{nodeType:X8} @ {stream.Position}.");
        }

        {
            var rv = stream.GetRegions(BinaryStreamRegionKind.Reading, BinaryStreamRegionType.Visited);
            var ri = stream.GetRegions(BinaryStreamRegionKind.Reading, BinaryStreamRegionType.Ignored);
            var sv = rv.Sum(s => s.Length);
            var si = ri.Sum(s => s.Length);
            var pv = sv / (double)stream.Length;
            var pi = si / (double)stream.Length;
            Console.WriteLine($"Ignored: {pi:P5}");
            Console.WriteLine($"Visited: {pv:P5}");
            // Assert.AreEqual(1.0d, pv, $"Stream position: {stream.Position}");
        }
    }
}

internal class DPCNodeRoot : DPCNode
{
    public DPCNodeRoot(DPCNodeReader reader) : base(reader)
    {
    }
}

public class DPCNodeReader
{
    public DPCNodeReader(BinaryStream stream)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        var magic = stream.ReadInt32(Endianness.BE);

        if (magic is not 0x4443504D)
        {
            throw new InvalidDataException("Unknown magic sequence.");
        }

        var version = stream.ReadInt32(Endianness.LE);

        if (version is not 0x00000043)
        {
            throw new InvalidDataException("Unknown version.");
        }

        DateTimeOffset.FromUnixTimeSeconds(stream.ReadInt32(Endianness.LE));

        BaseAddress = stream.ReadInt32(Endianness.LE);

        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }

    public BinaryStream Stream { get; }

    private int BaseAddress { get; }

    public int ReadAddress()
    {
        var address = Stream.ReadInt32(Endianness.LE);

        address -= BaseAddress;

        if (address < 0 || address >= Stream.Length)
        {
            throw new InvalidDataException($"Invalid address 0x{address:X8} @ {Stream.Position - sizeof(int)}.");
        }

        return address;
    }
}

public abstract class DPCNode
{
    protected DPCNode(DPCNodeReader reader)
    {
        Reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public DPCNodeReader Reader { get; }
}