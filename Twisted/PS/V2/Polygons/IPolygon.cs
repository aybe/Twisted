namespace Twisted.PS.V2.Polygons;

public interface IPolygon
{
    long Position { get; }

    IReadOnlyList<int> Indices { get; }
}