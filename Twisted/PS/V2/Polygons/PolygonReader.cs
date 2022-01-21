using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twisted.Extensions;

namespace Twisted.PS.V2.Polygons;

internal static class PolygonReader
{
    public static IReadOnlyList<IPolygon> TryRead(BinaryReader reader, int count, uint vertices, uint normals)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var polygons = new IPolygon[count];

        for (var i = 0; i < count; i++)
        {
            var peek    = reader.Peek(s => s.ReadUInt32(Endianness.BigEndian));
            var polygon = ReadPolygon(reader, peek, vertices, normals);
            polygons[i] = polygon;
        }

        return polygons;
    }

    [SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression", Justification = "Code coverage")]
    private static IPolygon ReadPolygon(BinaryReader reader, uint type, uint vertices, uint normals)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        if (vertices >= reader.BaseStream.Length) // TODO delete once useless
        {
            Assert.Fail();
        }

        if (normals >= reader.BaseStream.Length) // TODO delete once useless
        {
            // all the data read by these is -12 in size -> type + 4 indices TODO are all quads?

            switch (type)
            {
                case 0x00010505: break;
                case 0x00030706: break;
                case 0x00040808: break;
                case 0x03010807: break;
                case 0x03030A09: break;
                case 0x04010909: break;
                case 0x04040C0C: break;
                case 0x83010907: break;
                case 0x83030B09: break;
                case 0x84010A09: break;
                case 0x84040D0C: break;
                default:
                    Assert.Fail($"Missing normals for 0x{type:X8} @ {reader.BaseStream.Position}");
                    break;
            }
        }

        switch (type)
        {
            case 0x00010504: return new Polygon00010504(reader, vertices.ToInt32());
            case 0x00010505: return new Polygon00010505(reader, vertices.ToInt32());
            case 0x00010604: return new Polygon00010604(reader, vertices.ToInt32());
            case 0x00010605: return new Polygon00010605(reader, vertices.ToInt32());
            case 0x00010706: return new Polygon00010706(reader, vertices.ToInt32(), normals.ToInt32());
            case 0x00010708: return new Polygon00010708(reader, vertices.ToInt32(), normals.ToInt32());
            case 0x00030706: return new Polygon00030706(reader, vertices.ToInt32());
            case 0x00030806: return new Polygon00030806(reader, vertices.ToInt32());
            case 0x00030906: return new Polygon00030906(reader, vertices.ToInt32());
            case 0x00040808: return new Polygon00040808(reader, vertices.ToInt32());
            case 0x00040908: return new Polygon00040908(reader, vertices.ToInt32());
            case 0x00040A08: return new Polygon00040A08(reader, vertices.ToInt32(), normals.ToInt32());
            case 0x03010807: return new Polygon03010807(reader, vertices.ToInt32()); 
            case 0x03010907: return new Polygon03010907(reader, vertices.ToInt32());
            case 0x03010A09: return new Polygon03010A09(reader, vertices.ToInt32(), normals.ToInt32());
            case 0x03030A09: return new Polygon03030A09(reader, vertices.ToInt32());
            case 0x03030B09: return new Polygon03030B09(reader, vertices.ToInt32());
            case 0x03030C09: return new Polygon03030C09(reader, vertices.ToInt32());
            case 0x04010909: return new Polygon04010909(reader, vertices.ToInt32());
            case 0x04010A09: return new Polygon04010A09(reader, vertices.ToInt32());
            case 0x04010B0C: return new Polygon04010B0C(reader, vertices.ToInt32(), normals.ToInt32());
            case 0x04040C0C: return new Polygon04040C0C(reader, vertices.ToInt32());
            case 0x04040D0C: return new Polygon04040D0C(reader, vertices.ToInt32());
            case 0x04040E0C: return new Polygon04040E0C(reader, vertices.ToInt32());
            case 0x83010A07: return new Polygon83010A07(reader, vertices.ToInt32());
            case 0x83010B09: return new Polygon83010B09(reader, vertices.ToInt32(), normals.ToInt32());
            case 0x83010907: return new Polygon83010907(reader, vertices.ToInt32());
            case 0x83030B09: return new Polygon83030B09(reader, vertices.ToInt32());
            case 0x83030C09: return new Polygon83030C09(reader, vertices.ToInt32());
            case 0x83030D09: return new Polygon83030D09(reader, vertices.ToInt32());
            case 0x84010A09: return new Polygon84010A09(reader, vertices.ToInt32());
            case 0x84010B09: return new Polygon84010B09(reader, vertices.ToInt32());
            case 0x84010C0C: return new Polygon84010C0C(reader, vertices.ToInt32());
            case 0x84040D0C: return new Polygon84040D0C(reader, vertices.ToInt32());
            case 0x84040E0C: return new Polygon84040E0C(reader, vertices.ToInt32());
            case 0x84040F0C: return new Polygon84040F0C(reader, vertices.ToInt32());
            default: throw new NotSupportedException($"Unknown polygon: 0x{type:X8} @ {reader.BaseStream.Position}");
        }
    }
}