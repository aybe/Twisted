namespace Twisted.PS.Polygons;

internal sealed class Polygon84010C0C : Polygon
{
    public Polygon84010C0C(BinaryReader reader, int vertices, int positionNormals)
        : base(reader, 48, 4, vertices, positionNormals, 40)
    {
        // TODO tex stuff ? 4 bytes
        // TODO ...
        // TODO tex stuff E2 ? 4 bytes
        // TODO normals ? 8 bytes
    }
}