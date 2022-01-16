using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Tests.PS.V2.Extensions;
using Twisted.Tests.PS.V2.Formats.Graphics;

namespace Twisted.Tests.PS.V2;

internal static class Tester
{
    internal static void Test(TestContext context, string path)
    {
        context.WriteLine(path);

        var extension = Path.GetExtension(path).ToUpperInvariant();

        switch (extension)
        {
            case ".DMD":
                TestDMD(context, path);
                break;
            case ".TIM":
                TestTIM(context, path);
                break;
            case ".TMS":
                TestTMS(context, path);
                break;
            case ".TPC":
                TestTPC(context, path);
                break;
            default:
                throw new NotImplementedException(extension);
        }
    }

    private static void TestDMD(TestContext context, string path)
    {
        using var stream = new LogStream(new MemoryStream(File.ReadAllBytes(path)));
        using var reader = new BinaryReader(stream);

        context.WriteLine($"{nameof(stream.Length)}: {stream.Length}");

        var dmd = new DMD(reader);

        var visited        = stream.GetRanges(LogStreamInfo.Reading | LogStreamInfo.Visited);
        var skipped        = stream.GetRanges(LogStreamInfo.Reading | LogStreamInfo.Skipped);
        var visitedLength  = visited.Sum(s => s.Length);
        var skippedLength  = skipped.Sum(s => s.Length);
        var visitedPercent = (float)visitedLength / stream.Length;
        var skippedPercent = (float)skippedLength / stream.Length;
        var visitedMessage = $"Visited: {visitedPercent:P}";
        var skippedMessage = $"Skipped: {skippedPercent:P}";

        var builder = new StringBuilder();

        builder.AppendLine(path);
        builder.AppendLine(dmd.Time.ToString());

        var tree = dmd.PrintTree();

        builder.AppendLine(tree);

        builder.AppendLine(visitedMessage);

        foreach (var range in visited)
            builder.AppendLine($"\t{range}");

        builder.AppendLine(skippedMessage);

        foreach (var range in skipped)
            builder.AppendLine($"\t{range}");

        var text = builder.ToString();

        File.WriteAllText(Path.ChangeExtension(path, "TXT"), text);

        var nodes = new List<DMDNode>();
        ((DMDNode)dmd).TraverseDFS(nodes.Add);
        var ffs    = nodes.OfType<DMDNode00FF>().ToArray();
        var byVert = ffs.OrderBy(s => s.VerticesOffset).ToArray();
        var byNorm = ffs.OrderBy(s => s.NormalOffset).ToArray();
        var byPoly = ffs.OrderBy(s => s.PolygonsOffset).ToArray();

        var polygons = ffs.SelectMany(s => s.Polygons).ToArray();
        Assert.IsTrue(polygons.All(s => s.Indices.All(t => t >= 0)));
        Assert.IsTrue(visitedLength == stream.Length, $"{visitedMessage}, {skippedMessage}");
    }

    private static void TestTIM(TestContext context, string path)
    {
        new Tim(File.ReadAllBytes(path));
    }

    private static void TestTMS(TestContext context, string path)
    {
        using var stream = new MemoryStream(File.ReadAllBytes(path));
        using var reader = new BinaryReader(stream);

        context.WriteLine($"{nameof(stream.Length)}: {stream.Length}");

        var magic = reader.ReadInt32(Endianness.LittleEndian);

        Assert.AreEqual(0x50535854, magic, "Invalid identifier"); // TSXP -> PSX Texture ?

        var version = reader.ReadInt32(Endianness.LittleEndian);

        Assert.AreEqual(0x00000043, version, "Invalid version");

        var time = reader.ReadUnixTime(Endianness.LittleEndian).ToLocalTime();

        context.WriteLine($"{nameof(time)}: {time:F}");

        var count = reader.ReadInt32(Endianness.LittleEndian);

        context.WriteLine($"{nameof(count)}: {count}");

        var late = new DateTime(1996, 1, 1);

        if (time > late)
        {
            context.WriteLine("Twisted Metal 2 detected");

            var unknowns = new List<int>();

            for (var i = 0; i < count; i++)
            {
                var unknown = reader.ReadInt32();

                context.WriteLine($"\t{nameof(unknown)}: {unknown}");

                unknowns.Add(unknown);
            }

            context.WriteLine($"\t{nameof(unknowns)}: {unknowns.Sum()}");
        }

        context.WriteLine(null);

        var lengths = new List<int>();

        for (var i = 0; i < count; i++)
        {
            var length = reader.ReadInt32(Endianness.LittleEndian);

            context.WriteLine($"\t{nameof(length)}: {length}");

            lengths.Add(length);

            var bytes = reader.ReadBytes(length);
            var tim   = new Tim(bytes);
        }

        context.WriteLine($"\t{nameof(lengths)}: {lengths.Sum()}");

        Assert.IsTrue(lengths.Sum() < stream.Length, "lengths.Sum() < stream.Length");

        EnsureFullyRead(stream);
    }

    private static void TestTPC(TestContext context, string path)
    {
        using var stream = new MemoryStream(File.ReadAllBytes(path));
        using var reader = new BinaryReader(stream);

        context.WriteLine($"{nameof(stream.Length)}: {stream.Length}");

        var magic = reader.ReadInt32();

        Assert.AreEqual(0x4D504354, magic, "Invalid identifier"); // TCPM -> MPC (Multimedia PC) Texture ?

        var version = reader.ReadInt32();

        Assert.AreEqual(0x00000043, version, "Invalid version");

        var time = reader.ReadUnixTime(Endianness.LittleEndian).ToLocalTime();

        context.WriteLine($"{nameof(time)}: {time:F}");

        var count = reader.ReadInt32(Endianness.LittleEndian);

        context.WriteLine($"{nameof(count)}: {count}");

        var lengths = new List<int>();

        for (var i = 0; i < count; i++)
        {
            var length = reader.ReadInt32(Endianness.LittleEndian);

            context.WriteLine($"\t{nameof(length)}: {length}");

            lengths.Add(length);

            var bytes = reader.ReadBytes(length);
            var tim   = new Tim(bytes);
        }

        context.WriteLine($"\t{nameof(lengths)}: {lengths.Sum()}");

        Assert.IsTrue(lengths.Sum() < stream.Length, "lengths.Sum() < stream.Length");

        EnsureFullyRead(stream);
    }

    private static void EnsureFullyRead(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        Assert.AreEqual(stream.Length, stream.Position, "File was not fully read.");
    }
}