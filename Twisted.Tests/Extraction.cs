using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;
using Twisted.PS;

// ReSharper disable StringLiteralTypo

namespace Twisted.Tests;

[TestClass]
public sealed class Extraction
// massive extraction of all nodes and polygons sorted for further analysis
{
    [TestMethod]
    public void ExtractNodes()
    {
        var path = GetPath(@".twisted\TM1PSJAP");

        var root = GetNodes(path);

        var dictionary = TreeNodeUtility.GetDictionaryOfNodes(root);

        ExtractBinaryObjects(dictionary, Path.Combine(path, "nodes.bin"));
    }

    [TestMethod]
    public void ExtractPolygons()
    {
        var path = GetPath(@".twisted\TM1PSJAP");

        var root = GetNodes(path);

        var dictionary = TreeNodeUtility.GetDictionaryOfPolygons(root);

        ExtractBinaryObjects(dictionary, Path.Combine(path, "polygons.bin"));
    }

    private static void ExtractBinaryObjects(Dictionary<Type, List<IBinaryObject>> dictionary, string path)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

        var length = dictionary.Values.SelectMany(s => s).Max(s => s.Length) + 4;

        using var stream = File.Create(path);

        foreach (var binaryObject in dictionary.Values.SelectMany(s => s))
        {
            var data = binaryObject.GetObjectData();

            stream.Write(data);

            for (var i = 0; i < length - data.Length; i++)
            {
                stream.WriteByte(0xCD);
            }
        }
    }

    private static TempNode GetNodes(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(directory));

        var root = new TempNode();

        var files = Directory.GetFiles(directory, "*.DMD", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            using var reader = new BinaryReader(File.OpenRead(file));

            var dmd = new DMD(reader);

            root.Add(dmd);
        }

        return root;
    }

    private static string GetPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

        var info = new DirectoryInfo(Environment.CurrentDirectory);

        while (info.Parent != null)
        {
            var combine = Path.Combine(info.FullName, path);

            if (Directory.Exists(combine))
            {
                return combine;
            }

            info = info.Parent;
        }

        throw new DirectoryNotFoundException();
    }

    private class TempNode : TreeNode
    {
    }
}