using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Twisted.PS.Polygons;
using Unity.Extensions.Binary;

namespace Twisted.PS
{
    public sealed class DMDNode00FF : DMDNode
    {
        public DMDNode00FF(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            PositionVertices = ReadAddress(reader);
            PositionNormals  = ReadAddress(reader, false); // TODO could it be that out of bounds signifies no normals?
            PositionPolygons = ReadAddress(reader);

            var bytes = reader.ReadBytes(28);

            var u1 = bytes.ReadUInt32(0,  Endianness.BE);
            var u2 = bytes.ReadUInt32(4,  Endianness.BE);
            var u3 = bytes.ReadUInt32(8,  Endianness.BE);
            var u4 = bytes.ReadUInt32(12, Endianness.BE);
            var u5 = bytes.ReadUInt32(16, Endianness.BE);
            var u6 = bytes.ReadUInt32(20, Endianness.BE);
            var u7 = bytes.ReadUInt32(24, Endianness.BE);

            Flags = bytes[11];

            if ((Flags & 0x80) != 0)
            {
                var extraBytes = reader.ReadBytes(32 + 16); // TODO 00 10 00 00 .. .. .. .., 00 01 05 05 .. .. .. .. 
                // TODO int32 x, y, z, unknown
            }

            SetupBinaryObject(reader);

            var count = bytes.ReadUInt16(0, Endianness.LE);

            reader.BaseStream.Position = PositionPolygons;

            Polygons = PolygonReader.TryRead(reader, count, PositionVertices, PositionNormals);
        }

        public uint PositionVertices { get; }

        public uint PositionNormals { get; }

        public uint PositionPolygons { get; }

        public IReadOnlyList<Polygon> Polygons { get; }

        public byte Flags { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, " +
                   $"{nameof(PositionVertices)}: {PositionVertices}, " +
                   $"{nameof(PositionNormals)}: {PositionNormals}, " +
                   $"{nameof(PositionPolygons)}: {PositionPolygons}, " +
                   $"{nameof(Polygons)}: {Polygons.Count}, {string.Join(", ", Polygons.Select(s => s.GetType().Name.Replace("Polygon", string.Empty)).Distinct().OrderBy(s => s))}";
        }
    }
}