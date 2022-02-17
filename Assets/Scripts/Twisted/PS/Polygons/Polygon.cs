using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Twisted.PS.Texturing;
using Unity.Extensions.Binary;
using Unity.PlayStation.Graphics;
using UnityEngine;
using UnityEngine.Assertions;
using Vector2 = UnityEngine.Vector2;
using Vector4 = System.Numerics.Vector4;

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

            Vertices = new ReadOnlyCollection<Vector4>(vertices.Take(polygonFaces).ToArray());

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

            Normals = new ReadOnlyCollection<Vector4>(normals.Take(polygonFaces).ToArray());

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

        public TextureInfo? TextureInfo { get; protected init; } = null;

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

        protected static TextureInfo ReadTexture(byte[] data, int indices, int index)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(data));

            if (indices is not (3 or 4))
                throw new ArgumentOutOfRangeException(nameof(indices));

            if (index < 0 || index > data.Length - indices * 4)
                throw new ArgumentOutOfRangeException(nameof(index));

            var uvs = new Vector2Int[indices];

            for (var i = 0; i < indices; i++)
            {
                uvs[i] = ReadTextureUV(data, index + i * 4);
            }

            var page = ReadTexturePage(data, index + 6);

            var palette = ReadTexturePalette(data, index + 2);

            var texture = new TextureInfo(page, palette, new ReadOnlyCollection<Vector2Int>(uvs));

            return texture;
        }

        private static TexturePage ReadTexturePage(byte[] data, int index)
        {
            var raw = data.ReadInt32(index, Endianness.LE);

            var x = (raw & 0b_00000000_00001111) * 64;
            var y = (raw & 0b_00000000_00010000) / 16 * 256;
            var a = (raw & 0b_00000000_01100000) / 32;
            var b = (raw & 0b_00000001_10000000) / 128;
            var d = (raw & 0b_00001000_00000000) / 1024;

            var page = new TexturePage(x, y, (TexturePageAlpha)a, (TexturePageColors)b, (TexturePageDisable)d);

            return page;
        }

        private static Vector2Int ReadTexturePalette(byte[] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (index < 0 || index > data.Length - sizeof(short))
                throw new ArgumentOutOfRangeException(nameof(index));

            var raw = data.ReadInt16(index, Endianness.LE);

            var x = (raw & 0b00000000_00111111) * 16;
            var y = (raw & 0b01111111_11000000) / 64;

            var palette = new Vector2Int(x, y);

            return palette;
        }

        private static Vector2Int ReadTextureUV(byte[] data, int index)
        {
            var u = data.ReadByte(index + 0);
            var v = data.ReadByte(index + 1);

            return new Vector2Int(u, v);
        }
    }
}