﻿using System;
using System.Collections.Generic;
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

    private void Test(string path)
    {
        using var stream = new BinaryStream(File.OpenRead(path));
        using var reader = new BinaryReader(stream);

        if (reader.ReadInt32(Endianness.BE) is not 0x4443504D)
            throw new InvalidDataException("Unknown magic sequence.");

        if (reader.ReadInt32(Endianness.LE) is not 0x00000043)
            throw new InvalidDataException("Unknown version.");

        DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt32(Endianness.LE));

        var baseAddress = reader.ReadInt32(Endianness.LE);

        int ReadAddress()
        {
            var readInt32 = reader.ReadInt32(Endianness.LE);
            return readInt32 - baseAddress;
        }

        bool TryReadAddress(out int result)
        {
            result = ReadAddress();
            return result >= 0 && result < stream.Length;
        }

        // read table of contents

        stream.Position = ReadAddress();

        var addresses = new int[reader.ReadInt32(Endianness.LE)];

        for (var i = 0; i < addresses.Length; i++)
        {
            addresses[i] = ReadAddress();
        }

        TestContext.WriteLine("Table of contents:");

        foreach (var address in addresses)
        {
            TestContext.WriteLine(address.ToString());
        }

        // read the nodes iteratively

        var queue = new Queue<(DPCNode Parent, int Address)>();

        var root = new DPCNodeRoot();

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
                case 0x_00FF_0000:
                case 0x_00FF_0014:
                case 0x_00FF_0A05:
                case 0x_00FF_0A14:
                case 0x_00FF_0B05:
                case 0x_00FF_0C05:
                case 0x_00FF_0D05:
                case 0x_00FF_1405:
                case 0x_00FF_1815:
                case 0x_00FF_2D01:
                case 0x_00FF_2E01:
                case 0x_00FF_3401:
                case 0x_00FF_3501:
                case 0x_00FF_5014:
                case 0x_00FF_5114:
                case 0x_00FF_8813:
                case 0x_00FF_8A02:
                case 0x_00FF_8B02:
                case 0x_00FF_8C02:
                case 0x_00FF_8D02:
                case 0x_00FF_8E02:
                case 0x_00FF_8F02:
                case 0x_00FF_9002:
                case 0x_00FF_9102:
                case 0x_00FF_9202:
                case 0x_00FF_9E02:
                case 0x_00FF_9F02:
                case 0x_00FF_A002:
                case 0x_00FF_A102:
                case 0x_00FF_A802:
                case 0x_00FF_A902:
                case 0x_00FF_AA02:
                case 0x_00FF_AB02:
                case 0x_00FF_B414:
                case 0x_00FF_D200:
                case 0x_00FF_D400:
                case 0x_00FF_D500:
                case 0x_00FF_D600:
                case 0x_00FF_EC13:
                case 0x_00FF_F613:
                {
                    stream.Position += 4;

                    for (var i = 0; i < 3; i++)
                    {
                        // these aren't nodes, last is graphics

                        var tryReadAddress = TryReadAddress(out var result);

                        if (tryReadAddress is false)
                        {
                            TestContext.WriteLine($"Invalid address: {result}");
                        }
                    }

                    continue;
                }

                case 0x_0107_0000:
                case 0x_0107_0200:
                case 0x_0107_0300:
                case 0x_0107_0503:
                case 0x_0107_0A03:
                case 0x_0107_0D03:
                case 0x_0107_3200:
                case 0x_0107_3801:
                case 0x_0107_7800:
                case 0x_0107_0A00:
                case 0x_0107_1400:
                case 0x_0107_1E00:
                case 0x_0107_2800:
                case 0x_0107_2A00:
                case 0x_0107_3C00:
                case 0x_0107_4600:
                case 0x_0107_5000:
                case 0x_0107_5A00:
                case 0x_0107_6400:
                case 0x_0107_6E00:
                case 0x_0107_8200:
                case 0x_0107_C900:
                case 0x_0107_EC04:
                case 0x_0107_EE02:
                case 0x_0107_F002:
                case 0x_0107_F402:
                case 0x_0107_F802:
                {
                    stream.Position += 20;

                    var b1 = reader.ReadByte();
                    var b2 = reader.ReadByte();
                    var b3 = reader.ReadByte();
                    var b4 = reader.ReadByte();

                    for (var i = 0; i < b1; i++)
                        queue.Enqueue((parent, ReadAddress()));

                    continue;
                }
                case 0x_0107_0100:
                {
                    stream.Position += 20;

                    var b1 = reader.ReadByte();
                    var b2 = reader.ReadByte();
                    var b3 = reader.ReadByte();
                    var b4 = reader.ReadByte();

                    for (var i = 0; i < b1; i++)
                        ReadAddress();

                    continue;
                }

                case 0x_0207_0000:
                case 0x_0206_0000:
                case 0x_0208_0000:
                case 0x_0209_0000:
                case 0x_020A_0000:
                {
                    stream.Position += 16;

                    var b1 = reader.ReadByte();
                    var b2 = reader.ReadByte();
                    var b3 = reader.ReadByte();
                    var b4 = reader.ReadByte();

                    for (var i = 0; i < b1; i++)
                        ReadAddress(); // not nodes

                    continue;
                }
                case 0x_040B_0000:
                {
                    stream.Position += 16;

                    var b1 = reader.ReadByte();
                    var b2 = reader.ReadByte();
                    var b3 = reader.ReadByte();
                    var b4 = reader.ReadByte();

                    for (var i = 0; i < b3; i++)
                        queue.Enqueue((parent, ReadAddress()));

                    continue;
                }
                case 0x_050B_2A00:
                case 0x_050B_8B00:
                {
                    stream.Position += 52;

                    var b1 = reader.ReadByte();
                    var b2 = reader.ReadByte();
                    var b3 = reader.ReadByte();
                    var b4 = reader.ReadByte();

                    for (var i = 0; i < b3; i++)
                        queue.Enqueue((parent, ReadAddress()));

                    continue;
                }
                case 0x_07FF_0000:
                    reader.ReadBytes(72); // TODO
                    continue;
                case 0x_08FF_0000:
                    reader.ReadBytes(120); // TODO
                    continue;
                case 0x_0905_0000:
                case 0x_0905_0B00:
                case 0x_0905_1500:
                case 0x_0905_1F00:
                case 0x_0905_2900:
                case 0x_0905_3300:
                case 0x_0905_4700:
                case 0x_0905_5100:
                case 0x_0905_5B00:
                case 0x_0905_6500:
                case 0x_0905_6F00:
                case 0x_0905_7900:
                case 0x_0905_8300:
                {
                    stream.Position += 4;

                    var b1 = reader.ReadByte();
                    var b2 = reader.ReadByte();
                    var b3 = reader.ReadByte();
                    var b4 = reader.ReadByte();

                    for (var i = 0; i < b3; i++)
                        queue.Enqueue((parent, ReadAddress()));

                    continue;
                }
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
            TestContext.WriteLine($"Ignored: {pi:P5}");
            TestContext.WriteLine($"Visited: {pv:P5}");
            // Assert.AreEqual(1.0d, pv, $"Stream position: {stream.Position}");
        }
    }
}

public abstract class DPCNode
{
    public DPCNode? Parent { get; set; }

    public List<DPCNode> Children { get; set; } = new();
}

public sealed class DPCNodeRoot : DPCNode
{
}