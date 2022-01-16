﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;
using Twisted.IO;

namespace Twisted.PC;

public static class DPCTester
{
    public static void Test(TestContext context, string path)
    {
        if (!Path.GetExtension(path).Equals(".DPC", StringComparison.OrdinalIgnoreCase))
            throw new NotImplementedException();

        using var stream = new BinaryStream(new MemoryStream(File.ReadAllBytes(path)));
        using var reader = new BinaryReader(stream);

        if (reader.ReadInt32(Endianness.BE) is not 0x4443504D)
            throw new InvalidDataException("Unknown magic sequence.");

        if (reader.ReadInt32(Endianness.LE) is not 0x00000043)
            throw new InvalidDataException("Unknown version.");

        DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt32(Endianness.LE));

        reader.ReadInt32(Endianness.LE); // base address

        var nodeReader = new DPCNodeReader(reader);

        // read the nodes iteratively

        var queue = new Queue<(DPCNode? Parent, int Address)>();

        var root = default(DPCNode);

        queue.Enqueue((default, 16));

        while (queue.Any())
        {
            var (parent, address) = queue.Dequeue();

            reader.BaseStream.Position = address;

            DPCNode node;
            int[]   nodeChildren;
            var     nodeType = reader.Peek(s => s.ReadUInt32(Endianness.BE));

            switch (nodeType)
            {
                case 0xCC88_0180:
                    node = new DPCNodeRoot(nodeReader, out nodeChildren);
                    break;
                case 0x_0009_3D00:
                case 0x_0010_2700:
                case 0x_0019_0000:
                case 0x_0040_7A10:
                case 0x_0040_5A00:
                case 0x_0064_0000:
                case 0x_0071_0200:
                case 0x_00C4_0900:
                case 0x_00F9_1500:
                case 0x_0417_0300:
                case 0x_100E_0000:
                case 0x_1055_2200:
                case 0x_105C_0C00:
                case 0x_105E_5F00:
                case 0x_1027_0000:
                case 0x_107A_0700:
                case 0x_108C_5A00:
                case 0x_24F4_0000:
                case 0x_4006_0000:
                case 0x_4038_0000:
                case 0x_4042_0F00:
                case 0x_4054_8900:
                case 0x_4070_3100:
                case 0x_407E_0500:
                case 0x_409C_0000:
                case 0x_40F4_0200:
                case 0x_40E8_1D00:
                case 0x_4406_0B00:
                case 0x_4495_0800:
                case 0x_44D2_0200:
                case 0x_5125_0200:
                case 0x_5195_5900:
                case 0x_643F_4D00:
                case 0x_6927_0100:
                case 0x_8110_0000:
                case 0x_84DE_0100:
                case 0x_90C9_1900:
                case 0x_90D0_0300:
                case 0x_904A_4300:
                case 0x_905F_0100:
                case 0x_9076_1200:
                case 0x_907E_0000:
                case 0x_A177_0000:
                case 0x_A41F_0000:
                case 0x_C1C5_0000:
                case 0x_C904_0000:
                case 0x_E457_0000:
                case 0x_F915_0000:
                    Assert.IsTrue(parent is DPCNode020XXXXX);
                    node = new DPCNodeXXXXXXXX(nodeReader, out nodeChildren);
                    break;
                case 0x_0010_0000:
                    Assert.IsTrue(parent is DPCNode0B060000);
                    node = new DPCNode00100000(nodeReader, out nodeChildren);
                    break;
                case 0x_00F0_FFFF:
                    node = new DPCNode00F0XXXX(nodeReader, out nodeChildren);
                    break;
                case 0x_00FF_0000:
                case 0x_00FF_0014:
                case 0x_00FF_0A05:
                case 0x_00FF_0A14:
                case 0x_00FF_0B05:
                case 0x_00FF_0C05:
                case 0x_00FF_0D05:
                case 0x_00FF_1405:
                case 0x_00FF_1815:
                case 0x_00FF_2A03:
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
                case 0x_00FF_9903:
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
                    node = new DPCNode00FFXXXX(nodeReader, out nodeChildren);
                    break;
                case 0x_0107_0000:
                case 0x_0107_0100:
                case 0x_0107_0200:
                case 0x_0107_0300:
                case 0x_0107_0503:
                case 0x_0107_0A00:
                case 0x_0107_0A03:
                case 0x_0107_0D00:
                case 0x_0107_0D03:
                case 0x_0107_1400:
                case 0x_0107_1700:
                case 0x_0107_1E00:
                case 0x_0107_2100:
                case 0x_0107_2800:
                case 0x_0107_2A00:
                case 0x_0107_2B00:
                case 0x_0107_3200:
                case 0x_0107_3500:
                case 0x_0107_3801:
                case 0x_0107_3F00:
                case 0x_0107_3C00:
                case 0x_0107_4600:
                case 0x_0107_4900:
                case 0x_0107_5000:
                case 0x_0107_5300:
                case 0x_0107_5A00:
                case 0x_0107_5D00:
                case 0x_0107_6400:
                case 0x_0107_6700:
                case 0x_0107_6E00:
                case 0x_0107_7100:
                case 0x_0107_7800:
                case 0x_0107_7B00:
                case 0x_0107_8200:
                case 0x_0107_8F00:
                case 0x_0107_C900:
                case 0x_0107_CC02:
                case 0x_0107_D102:
                case 0x_0107_DA02:
                case 0x_0107_EC03:
                case 0x_0107_EC04:
                case 0x_0107_ED03:
                case 0x_0107_EE02:
                case 0x_0107_F002:
                case 0x_0107_F003:
                case 0x_0107_F401:
                case 0x_0107_F402:
                case 0x_0107_F703:
                case 0x_0107_F802:
                case 0x_0107_FD03:
                    node = new DPCNode0107XXXX(nodeReader, out nodeChildren);
                    break;
                case 0x_0309_0000:
                    node = new DPCNode03090000(nodeReader, out nodeChildren);
                    break;
                case 0x_0207_0000:
                case 0x_0206_0000:
                case 0x_0208_0000:
                case 0x_0209_0000:
                case 0x_020A_0000:
                    node = new DPCNode020XXXXX(nodeReader, out nodeChildren);
                    break;
                case 0x_040B_0000:
                case 0x_040B_5B02:
                case 0x_040B_9101:
                case 0x_040B_9201:
                case 0x_040B_9301:
                case 0x_040B_9401:
                case 0x_040B_9501:
                case 0x_040B_9A01:
                case 0x_040B_9B01:
                case 0x_040B_9C01:
                case 0x_040B_9D01:
                case 0x_040B_9E01:
                case 0x_040B_AE01:
                case 0x_040B_EA03:
                case 0x_040B_EE03:
                case 0x_040B_EF03:
                case 0x_040B_F403:
                    node = new DPCNode040BXXXX(nodeReader, out nodeChildren);
                    break;
                case 0x_050B_0000:
                case 0x_050B_2A00:
                case 0x_050B_8B00:
                case 0x_050B_EF03:
                case 0x_050B_F403:
                    node = new DPCNode050BXXXX(nodeReader, out nodeChildren);
                    break;
                case 0x_07FF_0000:
                    node = new DPCNode07FF0000(nodeReader, out nodeChildren);
                    break;
                case 0x_08FF_0000:
                case 0x_08FF_7805:
                case 0x_08FF_F401:
                    node = new DPCNode08FF0000(nodeReader, out nodeChildren);
                    break;
                case 0x_0905_0000:
                case 0x_0905_0203:
                case 0x_0905_0B00:
                case 0x_0905_0C00:
                case 0x_0905_1500:
                case 0x_0905_1600:
                case 0x_0905_1F00:
                case 0x_0905_2000:
                case 0x_0905_2900:
                case 0x_0905_3300:
                case 0x_0905_3400:
                case 0x_0905_3E00:
                case 0x_0905_4700:
                case 0x_0905_4800:
                case 0x_0905_5100:
                case 0x_0905_5802:
                case 0x_0905_5902:
                case 0x_0905_5B00:
                case 0x_0905_5C00:
                case 0x_0905_6202:
                case 0x_0905_6402:
                case 0x_0905_6500:
                case 0x_0905_6600:
                case 0x_0905_6F00:
                case 0x_0905_7000:
                case 0x_0905_7900:
                case 0x_0905_7A00:
                case 0x_0905_8300:
                case 0x_0905_8400:
                case 0x_0905_8603:
                case 0x_0905_9803:
                case 0x_0905_BC02:
                case 0x_0905_E803:
                case 0x_0905_E903:
                case 0x_0905_EA03:
                case 0x_0905_F102:
                case 0x_0905_F202:
                case 0x_0905_F103:
                case 0x_0905_F203:
                case 0x_0905_F302:
                case 0x_0905_F803:
                case 0x_0905_F903:
                case 0x_0905_FA03:
                case 0x_0905_FB03:
                    node = new DPCNode0905XXXX(nodeReader, out nodeChildren);
                    break;
                case 0x_0B06_0000:
                    node = new DPCNode0B060000(nodeReader, out nodeChildren);
                    break;
                default:
                    throw new NotImplementedException(
                        $"Unknown node 0x{nodeType:X8} @ {stream.Position}, Parent: {parent}.");
            }

            Assert.AreNotEqual(0, node.Length, node.GetType().Name);

            root ??= node;

            node.Parent = parent;

            parent?.Children.Add(node);

            foreach (var children in nodeChildren)
            {
                queue.Enqueue((node, children));
            }
        }

        if (root is null)
        {
            throw new InvalidDataException("DPC root node is null.");
        }

        int count = 0, depth = 0;

        root.Traverse(node =>
        {
            count += 1;
            depth =  Math.Max(depth, node.Depth);
        });

        context.WriteLine();
        context.WriteLine($"Tree depth: {depth}");
        context.WriteLine($"Tree nodes: {count}");
        context.WriteLine();

        {
            var rv = stream.GetRegions(BinaryStreamRegionKind.Reading, BinaryStreamRegionType.Visited).ToArray();
            var ri = stream.GetRegions(BinaryStreamRegionKind.Reading, BinaryStreamRegionType.Ignored).ToArray();
            var sv = rv.Sum(s => s.Length);
            var si = ri.Sum(s => s.Length);
            var pv = sv / (double)stream.Length;
            var pi = si / (double)stream.Length;

            context.WriteLine($"Regions ignored: {pi:P3}");
            context.WriteLine($"Regions visited: {pv:P3}");
            context.WriteLine();

            if (false)
            {
                // save reading statistics

                using (var sw = File.AppendText(@"C:\TEMP\test.txt"))
                {
                    sw.WriteLine($"{path};{pi};{pv}");
                }

                // load reading statistics

                using (var sr = File.OpenText(@"C:\TEMP\test.txt"))
                {
                    var dictionary = new Dictionary<string, List<(double Ignored, double Visited)>>();

                    for (var line = sr.ReadLine(); line != null; line = sr.ReadLine())
                    {
                        var split = line.Split(';');

                        var key = split[0];

                        if (dictionary.ContainsKey(key) is false)
                        {
                            dictionary.Add(key, new List<(double Ignored, double Visited)>());
                        }

                        var val = (double.Parse(split[1]), double.Parse(split[2]));

                        dictionary[key].Add(val);
                    }

                    // show visited improvement

                    var array = dictionary[path].OrderBy(s => s.Visited).Distinct().TakeLast(2).ToArray();

                    double vv;

                    if (array.Length > 1)
                    {
                        vv = array[1].Visited - array[0].Visited;
                    }
                    else
                    {
                        vv = default;
                    }

                    context.WriteLine($"Visited increase: {vv:P3}");
                }
            }

            context.WriteLine($"Ignored: {ri.Length}");

            foreach (var region in ri)
            {
                context.WriteLine(region.ToString());
            }

            context.WriteLine();

            context.WriteLine($"Visited: {rv.Length}");

            foreach (var region in rv)
            {
                context.WriteLine(region.ToString());
            }

            context.WriteLine();

            DPCNode.ToStringVerbose = true;

            context.WriteLine(root.Print());

            Assert.IsTrue(pi <= 1.0d, "More than 100% data ignored.");
            Assert.IsTrue(pv <= 1.0d, "More than 100% data visited.");

            // Assert.AreEqual(1.0d, pv, $"Visited: {pv:P}"); 
        }

        // perform some extra tests

        root.Traverse(s =>
        {
            if (s is DPCNode0107XXXX { B3: not 0 } node0107XXXX)
            {
                Assert.IsTrue(
                    node0107XXXX.Children.OfType<DPCNode00FFXXXX>().Any() ||
                    node0107XXXX.Children.OfType<DPCNode0107XXXX>().Any(),
                    node0107XXXX.ToString()
                );
            }
        });
    }
}