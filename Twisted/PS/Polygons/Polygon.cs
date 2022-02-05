using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Twisted.Extensions;

namespace Twisted.PS.Polygons
{
    public abstract class Polygon : IBinaryObject
    {
        protected Polygon(
            BinaryReader reader,
            int polygonSize = -1,
            int polygonFaces = -1,
            int positionVertices = -1,
            int positionNormals = -1,
            int offsetNormals = -1)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (polygonSize is -1 or < 12)
                throw new ArgumentOutOfRangeException(nameof(polygonSize), polygonSize,
                    "Polygon size must be at least 12 bytes.");

            if (polygonFaces is not (3 or 4))
                throw new ArgumentOutOfRangeException(nameof(polygonFaces), polygonFaces,
                    "Polygon face count must be either 3 or 4.");

            if (positionVertices < 0 || positionVertices >= reader.BaseStream.Length)
                throw new ArgumentOutOfRangeException(nameof(positionVertices), positionVertices,
                    "Polygon position to vertices points outside stream bounds.");

            if (positionNormals != -1 && (offsetNormals < 12 || offsetNormals - polygonSize > -8))
                throw new ArgumentOutOfRangeException(nameof(offsetNormals), offsetNormals,
                    "Polygon offset to normals must be between 12 and polygon size minus 8.");

            Position = reader.BaseStream.Position;

            // read polygon data, this will be accessible to derived types for further reading

            Data = reader.ReadBytes(polygonSize);

            // set polygon length, this will allow us to extract chunks of polygons for further analysis

            Length = reader.BaseStream.Position - Position;

            // read polygon type

            Type = Data.ReadUInt32(0, Endianness.BE);

            /* NOTE we read all indices to max out visit rate but we then only take what we need */

            // read polygon vertices

            var verticesIndices = new int[4];

            for (var i = 0; i < 4; i++)
            {
                verticesIndices[i] = Data.ReadUInt16(4 + i * sizeof(ushort), Endianness.LE);
            }

            if (polygonFaces == 3) // trivial check to assert we're good
            {
                Assert.AreEqual(0, verticesIndices[3], "Polygon is a triangle but fourth vertex index is not zero.");
            }

            var vertices = new Vector4[4];

            for (var i = 0; i < 4; i++)
            {
                vertices[i] = reader.ReadObject(s => ReadVector4Int16(s, Endianness.LE), positionVertices + verticesIndices[i] * 8);
            }

            Vertices = vertices.Take(polygonFaces).AsReadOnly();

            // read polygon normals if any

            Normals = Array.Empty<Vector4>();

            if (positionNormals == -1)
                return;

            var normalsIndices = new int[4];

            for (var i = 0; i < 4; i++)
            {
                normalsIndices[i] = Data.ReadUInt16(offsetNormals + i * sizeof(ushort), Endianness.LE);
            }

            var normals = new Vector4[4];

            for (var i = 0; i < 4; i++)
            {
                normals[i] = reader.ReadObject(s => ReadVector4Int16(s, Endianness.LE), positionNormals + normalsIndices[i] * 8);
            }

            Normals = normals.Take(polygonFaces).AsReadOnly();

            static Vector4 ReadVector4Int16(BinaryReader reader, Endianness endianness)
            {
                if (reader == null)
                    throw new ArgumentNullException(nameof(reader));

                var x = reader.ReadInt16(endianness);
                var y = reader.ReadInt16(endianness);
                var z = reader.ReadInt16(endianness);
                var w = reader.ReadInt16(endianness);
                var v = new Vector4(x, y, z, w);

                return v;
            }
        }

        private byte[] Data { get; }

        public uint Type { get; }

        public IReadOnlyList<Vector4> Vertices { get; }

        public IReadOnlyList<Vector4> Normals { get; }

        public long Position { get; }

        public long Length { get; }

        public byte[] GetObjectData()
        {
            return Data.ToArray();
        }

        public override string ToString()
        {
            return
                $"{nameof(Type)}: 0x{Type:X8}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(Vertices)}: {Vertices.Count}, {nameof(Normals)}: {Normals.Count}";
        }
    }
}