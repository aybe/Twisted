using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;
using Twisted.IO;

namespace Twisted.PS.V2;

public static class DMDTester
{
    public static void Test(TestContext context, string path)
    {
        using var stream = new BinaryStream(new MemoryStream(File.ReadAllBytes(path)));
        using var reader = new BinaryReader(stream);

        context.WriteLine($"{nameof(stream.Length)}: {stream.Length}");

        var dmd = new DMD(reader);

        var visited        = stream.GetRegions(BinaryStreamRegionKind.Reading, BinaryStreamRegionType.Visited).ToArray();
        var skipped        = stream.GetRegions(BinaryStreamRegionKind.Reading, BinaryStreamRegionType.Ignored).ToArray();
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
}