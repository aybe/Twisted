using System;

namespace Twisted.Tests.PS.V2.Extensions;

[Flags]
public enum LogStreamInfo
{
    Reading = 1 << 0,
    Writing = 1 << 1,
    Skipped = 1 << 2,
    Visited = 1 << 3
}