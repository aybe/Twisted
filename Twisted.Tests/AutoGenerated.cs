﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;
using Twisted.IO;
using Twisted.PC;
using Twisted.PS.V2;

namespace Twisted.Tests;

public static class AutoGenerated
{
    public static void Test(TestContext context, string path)
    {
        using var stream = new BinaryStream(new MemoryStream(File.ReadAllBytes(path)));

        // print file information

        var info = new FileInfo(path);
        context.WriteLine($"File name: {info.Name}");
        context.WriteLine($"File size: {info.Length}");

        // read file

        TreeNode treeNode;

        var extension = Path.GetExtension(path).ToUpperInvariant();

        switch (extension)
        {
            case ".DMD":
                treeNode = new DMD(new BinaryReader(stream));
                break;
            case ".DPC":
                treeNode = DPCTester.Test(context, stream);
                break;
            default:
                throw new NotSupportedException(extension);
        }

        // check that node lengths are valid

        treeNode.TraverseDFS(s =>
        {
            switch (s)
            {
                case DMDNode dmdNode:
                    Assert.IsTrue(dmdNode.Length != default, $"Zero length for {dmdNode.GetType().Name} @ {dmdNode.Position}");
                    break;
                case DPCNode dpcNode:
                    Assert.IsTrue(dpcNode.Length != default, $"Zero length for {dpcNode.GetType().Name} @ {dpcNode.Position}");
                    break;
                default:
                    throw new NotSupportedException();
            }
        });

        // print visited, ignored, depth, count

        int count = 0, depth = 0;

        treeNode.TraverseDFS(node =>
        {
            count += 1;
            depth =  Math.Max(depth, node.Depth);
        });

        context.WriteLine();

        var visited = stream.GetRegions(BinaryStreamRegionKind.Reading, BinaryStreamRegionType.Visited).ToArray();
        var ignored = stream.GetRegions(BinaryStreamRegionKind.Reading, BinaryStreamRegionType.Ignored).ToArray();

        var visitedSum = visited.Sum(s => s.Length);
        var ignoredSum = ignored.Sum(s => s.Length);
        var visitedPct = (double)visitedSum / stream.Length;
        var ignoredPct = (double)ignoredSum / stream.Length;

        context.WriteLine($"Regions visited: {visited.Length} ({visitedPct:P3})");
        context.WriteLine($"Regions ignored: {ignored.Length} ({ignoredPct:P3})");
        context.WriteLine();

        context.WriteLine($"Tree depth: {depth}");
        context.WriteLine($"Tree nodes: {count}");
        context.WriteLine();

        // print tree

        context.WriteLine(treeNode.Print());
        context.WriteLine();

        // print nodes by position

        var pairs1 = new List<KeyValuePair<long, TreeNode>>();
        var pairs2 = pairs1;

        treeNode.TraverseDFS(s =>
        {
            switch (s)
            {
                case DMDNode node:
                    pairs2.Add(new KeyValuePair<long, TreeNode>(node.Position, node));
                    break;
                case DPCNode node:
                    pairs2.Add(new KeyValuePair<long, TreeNode>(node.Position, node));
                    break;
            }
        });

        pairs1 = pairs1.OrderBy(s => s.Key).ToList();

        context.WriteLine($"Nodes by position: {pairs1.Count}");

        foreach (var (key, value) in pairs1)
        {
            context.WriteLine($"{key,-6} {value.GetType().Name}");
        }

        context.WriteLine();

        // print regions in detail

        context.WriteLine($"Regions visited: {visited.Length}");

        foreach (var region in visited)
        {
            context.WriteLine(region);
        }

        context.WriteLine();

        context.WriteLine($"Regions ignored: {ignored.Length}");

        foreach (var region in ignored)
        {
            context.WriteLine($"{region}, {pairs1.Last(s => s.Key < region.Position).Value}");
        }

        context.WriteLine();

        // final extra checks

        Assert.IsTrue(visitedSum <= stream.Length, "More than 100% data visited.");
        Assert.IsTrue(ignoredSum <= stream.Length, "More than 100% data ignored.");

        Assert.AreEqual(stream.Length, visitedSum + ignoredSum, "Sum of visited and ignored mismatch stream length.");

        // Assert.IsTrue(visitedSum == stream.Length, $"Visited: {visitedPct:P3}.");
    }
}