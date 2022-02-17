using Unity.Extensions.Binary;
using Unity.Extensions.General;

namespace Twisted.PC
{
    public sealed class DPCNodeXXXXXXXX : DPCNode
    {
        internal DPCNodeXXXXXXXX(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
        {
            Type = reader.ReadInt32(Endianness.LE);

            Assert.IsTrue(Type > 1000, Type.ToString());

            B1 = reader.ReadByte();
            B2 = reader.ReadByte();
            B3 = reader.ReadByte();
            B4 = reader.ReadByte();

            // Assert.AreNotEqual((byte)0, B1, ToString());

            // Assert.AreNotEqual((byte)0, B2, ToString());

            // Assert.AreNotEqual((byte)0, B3, ToString());

            Assert.AreEqual((byte)0, B4, ToString());

            var addresses = reader.ReadAddresses(1);

            SetLength(reader);

            children = addresses;
        }

        public int Type { get; }

        public byte B1 { get; }

        public byte B2 { get; }

        public byte B3 { get; }

        public byte B4 { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Type)}: {Type}, {nameof(B1)}: {B1}, {nameof(B2)}: {B2}, {nameof(B3)}: {B3}, {nameof(B4)}: {B4}";
        }
    }
}