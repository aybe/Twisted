using System.IO;
using UnityEngine.Assertions;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon84010C0C : PolygonFT4
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 40
        // TODO unknown int32 @ 44
    {
        public Polygon84010C0C(BinaryReader reader, int vertices, int positionNormals)
            : base(reader, 48, 4, vertices, positionNormals, 40)
        {
            Assert.AreEqual(default, Data[30]);
            Assert.AreEqual(default, Data[31]);
            Assert.AreEqual(default, Data[34]);
            Assert.AreEqual(default, Data[35]);
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x3C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TexturePosition { get; } = 20;

        protected override int? TextureWindowPosition { get; } = 36;
    }
}