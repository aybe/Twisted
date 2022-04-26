using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Twisted.Formats.Graphics2D;
using Unity.Extensions.Binary;
using UnityEngine;
using UnityEngine.Assertions;
using Vector4 = System.Numerics.Vector4;

namespace Twisted.Graphics
{
    public abstract class Polygon : IBinaryObject
    {
        protected Polygon(
            BinaryReader reader,
            int          polygonSize      = -1,
            int          polygonFaces     = -1,
            int          positionVertices = -1,
            int          positionNormals  = -1,
            int          offsetNormals    = -1)
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

        public long Position { get; }

        public long Length { get; }

        public byte[] GetObjectData()
        {
            return Data.ToArray();
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: 0x{Type:X8}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(Vertices)}: {Vertices.Count}, {nameof(Normals)}: {Normals.Count}";
        }

        #region Color

        protected virtual int? ColorElements => null;

        protected virtual int? ColorPosition => null;

        protected virtual int? ColorType { get; } = null;

        public IReadOnlyList<Color32>? Colors => TryReadColors();

        private IReadOnlyList<Color32>? TryReadColors()
        {
            var elements = ColorElements;
            var position = ColorPosition;

            if (elements is null && position is null)
                return null;

            if (elements is null != position is null)
                throw new InvalidOperationException($"Both {nameof(ColorElements)} and {nameof(ColorPosition)} must be overridden.");

            var colors = new Color32[elements.Value];

            var data = GetObjectData();

            for (var i = 0; i < colors.Length; i++)
            {
                var span = data.AsSpan(position.Value + i * 4);

                var r = span[0];
                var g = span[1];
                var b = span[2];

                if (ColorType != null)
                {
                    var a = span[3];

                    if (a != ColorType.Value)
                    {
                        Debug.LogWarning($"{GetType().Name}: {nameof(ColorType)} is 0x{ColorType.Value:X2} but color type data is 0x{a:X2}.");
                    }
                }

                colors[i] = new Color32(r, g, b, byte.MaxValue);
            }

            return colors;
        }

        #endregion

        #region Texture

        protected virtual int? TextureElements { get; } = null;

        protected virtual int? TexturePosition { get; } = null;

        public TextureInfo? TextureInfo => TryReadTextureInfo();

        public IReadOnlyList<Vector2Int>? TextureUVs => TryReadTextureUVs();

        private TextureInfo? TryReadTextureInfo()
        {
            var elements = TextureElements;
            var position = TexturePosition;

            if (elements is null && position is null)
                return null;

            if (elements is null != position is null)
                throw new InvalidOperationException($"Both {nameof(TextureElements)} and {nameof(TexturePosition)} must be overridden.");

            if (elements.Value is not (3 or 4))
                throw new InvalidDataException($"{nameof(TextureElements)} must be 3 or 4.");

            var data    = GetObjectData();
            var dataMax = data.Length - 8;

            if (position.Value > dataMax)
                throw new ArgumentOutOfRangeException($"{nameof(TexturePosition)} must be less than or equal to {dataMax}.");

            var paletteRaw = data.ReadInt16(position.Value + 2, Endianness.LE);
            var paletteX   = (paletteRaw & 0b00000000_00111111) * 16;
            var paletteY   = (paletteRaw & 0b01111111_11000000) / 64;
            var palette    = new Vector2Int(paletteX, paletteY);

            var pageRaw     = data.ReadInt32(position.Value + 6, Endianness.LE);
            var pageX       = (pageRaw & 0b_00000000_00001111) * 64;
            var pageY       = (pageRaw & 0b_00000000_00010000) / 16 * 256;
            var pageAlpha   = (pageRaw & 0b_00000000_01100000) / 32;
            var pageColors  = (pageRaw & 0b_00000001_10000000) / 128;
            var pageDisable = (pageRaw & 0b_00001000_00000000) / 1024;
            var page        = new TexturePage(new Vector2Int(pageX, pageY), (TexturePageAlpha)pageAlpha, (TexturePageColors)pageColors, (TexturePageDisable)pageDisable);

            return new TextureInfo(page, palette);
        }

        private IReadOnlyList<Vector2Int>? TryReadTextureUVs()
        {
            var elements = TextureElements;
            var position = TexturePosition;

            if (elements is null && position is null)
                return null;

            if (elements is null != position is null)
                throw new InvalidOperationException($"Both {nameof(TextureElements)} and {nameof(TexturePosition)} must be overridden.");

            if (elements.Value is not (3 or 4))
                throw new InvalidDataException($"{nameof(TextureElements)} must be 3 or 4.");

            var data    = GetObjectData();
            var dataMax = data.Length - elements.Value * 4;

            if (position.Value > dataMax)
                throw new ArgumentOutOfRangeException($"{nameof(TexturePosition)} must be less than or equal to {dataMax}.");

            var uvs = new Vector2Int[elements.Value];

            for (var i = 0; i < uvs.Length; i++)
            {
                var j = position.Value + i * 4;
                var u = data.ReadByte(j + 0);
                var v = data.ReadByte(j + 1);
                uvs[i] = new Vector2Int(u, v);
            }

            return uvs;
        }

        #endregion
    }
}