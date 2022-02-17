using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Twisted.PS.Texturing;
using Unity.PlayStation.Graphics;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable StringLiteralTypo

namespace Twisted.Tests
{
    public sealed class NewTestScript
    {
        [Test]
        public void TestIndexed4()
        {
            const string fileName = @"C:\TEMP\TM1PSJAP car select.SNA";

            GetTexture(
                fileName,
                FrameBufferFormat.Indexed4,
                new RectInt(64, 256, 64, 256),
                new RectInt(16, 281, 16, 1),
                TransparentColorMode.None
            );
        }

        [Test]
        public void TestIndexed8()
        {
            const string fileName = @"C:\TEMP\TM1PSJAP car select.SNA";

            GetTexture(
                fileName,
                FrameBufferFormat.Indexed8,
                new RectInt(0, 256, 128, 256),
                new RectInt(0, 280, 256, 1),
                TransparentColorMode.None
            );
        }

        [Test]
        public void TestDirect15()
        {
            const string fileName = @"C:\TEMP\TM1PSJAP car select.SNA";

            GetTexture(
                fileName,
                FrameBufferFormat.Direct15,
                new RectInt(320, 0, 320, 240),
                null,
                TransparentColorMode.None
            );
        }

        [Test]
        public void TestDirect24()
        {
            const string fileName = @"C:\TEMP\TM1PSJAP video.SNA";

            GetTexture(
                fileName,
                FrameBufferFormat.Direct24,
                new RectInt(480, 0, 480, 240),
                null,
                TransparentColorMode.None
            );
        }

        private static void GetTexture(string path, FrameBufferFormat fmt, RectInt picRect, RectInt? palRect, TransparentColorMode mode)
        {
            var buffer = GetFrameBuffer(path);

            var texture = FrameBuffer.GetTexture(fmt, buffer, picRect, palRect, buffer, mode);

            path = Path.Combine(
                Path.GetDirectoryName(path)!,
                Path.ChangeExtension(
                    $"{Path.GetFileNameWithoutExtension(path)} x = {picRect.x}, y = {picRect.y}, w = {picRect.width}, h = {picRect.height}, f = {fmt}",
                    "PNG"
                )
            );

            File.WriteAllBytes(path, texture.EncodeToPNG());

            Object.DestroyImmediate(texture);
        }

        private static FrameBuffer GetFrameBuffer(string path)
        {
            var bytes = File.ReadAllBytes(path);

            bytes = bytes.Skip(0x289070).Take(1024 * 512 * 2).ToArray();

            var span = MemoryMarshal.Cast<byte, short>(bytes.AsSpan());

            var pixels = span.ToArray();

            var obj = new FrameBuffer(FrameBufferFormat.Direct15, new RectInt(Vector2Int.zero, new Vector2Int(1024, 512)), pixels);

            return obj;
        }
    }
}