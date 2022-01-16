using System;
using System.IO;
using JetBrains.Annotations;

namespace Twisted.Tests.PS.V1;

public static class Storage
{
    public static string GetPath(string path, [CanBeNull] string directory = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

        directory ??= Environment.CurrentDirectory;

        var parent = new DirectoryInfo(directory);

        while (parent != null)
        {
            var combine = Path.Combine(parent.FullName, path);

            if (Directory.Exists(combine) || File.Exists(combine))
                return combine;

            parent = parent.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}