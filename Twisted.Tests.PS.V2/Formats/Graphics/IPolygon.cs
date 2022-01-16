using System.Collections.Generic;

namespace Twisted.Tests.PS.V2.Formats.Graphics;

public interface IPolygon
{
    long Position { get; }

    IReadOnlyList<int> Indices { get; }
}