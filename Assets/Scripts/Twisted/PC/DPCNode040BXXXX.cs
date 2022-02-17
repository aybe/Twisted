using Unity.Extensions.Binary;
using Unity.Extensions.General;

namespace Twisted.PC
{
    public sealed class DPCNode040BXXXX : DPCNode
    {
        internal DPCNode040BXXXX(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
        {
            var bytes = reader.ReadBytes(16);

            I1 = bytes.ReadInt32(4,  Endianness.LE);
            I2 = bytes.ReadInt32(8,  Endianness.LE);
            I3 = bytes.ReadInt32(12, Endianness.LE);

            // Assert.IsTrue(I1 > 0);
            // Assert.IsTrue(I2 > 0);
            // Assert.IsTrue(I3 > 0);

            B1 = reader.ReadByte();

            B2 = reader.ReadByte();

            ChildrenCount = reader.ReadByte();

            B4 = reader.ReadByte();

            // Assert.AreEqual(0, b1, Position.ToString());

            Assert.AreEqual(0, B2, Position.ToString());

            // Assert.AreEqual(1, b3, Position.ToString());

            // Assert.AreEqual(0, b4, Position.ToString());

            var addresses = reader.ReadAddresses(ChildrenCount);

            SetLength(reader);

            children = addresses;
        }

        public int I1 { get; }

        public int I2 { get; }

        public int I3 { get; }

        public byte B1 { get; }

        public byte B2 { get; }

        public byte ChildrenCount { get; }

        public byte B4 { get; }

        public override string ToString()
        {
            return
                $"{base.ToString()}, {nameof(I1)}: {I1}, {nameof(I2)}: {I2}, {nameof(I3)}: {I3}, {nameof(B1)}: {B1}, {nameof(B2)}: {B2}, {nameof(ChildrenCount)}: {ChildrenCount}, {nameof(B4)}: {B4}";
        }
    }
}