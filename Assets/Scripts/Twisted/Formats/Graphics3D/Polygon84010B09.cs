using System.IO;
using Twisted.Formats.Binary;

namespace Twisted.Formats.Graphics3D
{
    internal sealed class Polygon84010B09 : PolygonFT3
        // TODO unknown int32 @ 12
        // TODO unknown int32 @ 40
    {
        public Polygon84010B09(BinaryReader reader, int positionVertices)
            : base(reader, 44, 4, positionVertices)
        {
            var data = GetObjectData();

            var unknown1 = data.ReadInt16(12, Endianness.LE);
            var unknown2 = data.ReadInt16(14, Endianness.LE);
            var unknown3 = data.ReadInt16(40, Endianness.LE);
            var unknown4 = data.ReadInt16(42, Endianness.LE);
        }

        protected override int? ColorElements { get; } = 1;

        protected override int? ColorPosition { get; } = 16;

        protected override int? ColorType { get; } = 0x2C;

        protected override int? TextureElements { get; } = 4;

        protected override int? TextureElementsPosition { get; } = 20;

        protected override int? TextureWindowPosition { get; } = 36;
    }
}