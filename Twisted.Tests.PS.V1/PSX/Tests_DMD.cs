using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Twisted.Tests.PS.V1.PSX;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Tests_DMD
{
    protected static void TestDMD(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

        path = Storage.GetPath(path);

        using var stream0 = File.OpenRead(path);
        using var stream1 = File.Create($"{path}.log");
        //using var stream2 = File.Create($"{path} {DateTime.Now.ToString("s").Replace(':', '-')}.log");

        Trace.Listeners.Clear();
        Trace.Listeners.Add(new TextWriterTraceListener(stream1));
        //Trace.Listeners.Add(new TextWriterTraceListener(stream2));
        Trace.AutoFlush   = true;
        Trace.IndentLevel = 0;
        Trace.WriteLine(path);
        Trace.WriteLine(DateTime.Now);

        new DMD(stream0);
    }
}