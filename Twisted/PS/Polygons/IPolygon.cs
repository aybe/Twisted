namespace Twisted.PS.Polygons;

public interface IPolygon
{
    long Position { get; }

    IReadOnlyList<int> Indices { get; }
}