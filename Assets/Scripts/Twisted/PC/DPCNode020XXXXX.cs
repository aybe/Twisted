using Unity.Extensions.Binary;
using UnityEngine.Assertions;

namespace Twisted.PC
{
    public sealed class DPCNode020XXXXX : DPCNode
    {
        internal DPCNode020XXXXX(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
        {
            var bytes = reader.ReadBytes(16);

            A = bytes.ReadInt32(4,  Endianness.LE);
            B = bytes.ReadInt32(8,  Endianness.LE);
            C = bytes.ReadInt32(12, Endianness.LE);

            var b1 = reader.ReadByte();
            var b2 = reader.ReadByte();
            var b3 = reader.ReadByte();
            var b4 = reader.ReadByte();

            Assert.IsTrue(b1 is 1 or 2 or 3 or 4 or 5, Position.ToString());

            Assert.IsTrue(b2 is 0, Position.ToString());

            Assert.IsTrue(b3 is 0, Position.ToString());

            Assert.IsTrue(b4 is 0, Position.ToString());

            var addresses = reader.ReadAddresses(b1);

            SetLength(reader);

            children = addresses;
        }

        public int A { get; }

        public int B { get; }

        public int C { get; }

        public override string ToString()
        {
            return ToStringVerbose
                ? $"{base.ToString()}, {nameof(A)}: {A}, {nameof(B)}: {B}, {nameof(C)}: {C}"
                : base.ToString();
        }
    }
}