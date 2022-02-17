using System;
using Unity.Extensions.Binary;
using Unity.Extensions.General;

namespace Twisted.PC
{
    public sealed class DPCNode08FF0000 : DPCNode
    {
        internal DPCNode08FF0000(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
        {
            var bytes = reader.ReadBytes(120);

            I1 = bytes.ReadInt32(4,  Endianness.LE);
            I2 = bytes.ReadInt32(8,  Endianness.LE);
            I3 = bytes.ReadInt32(12, Endianness.LE);

            Assert.AreNotEqual(0, I1, ToString());

            // Assert.AreNotEqual(0, i2, ToString());

            // Assert.AreNotEqual(0, i3, ToString());

            SetLength(reader);

            children = Array.Empty<int>();
        }

        public int I1 { get; }

        public int I2 { get; }

        public int I3 { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(I1)}: {I1}, {nameof(I2)}: {I2}, {nameof(I3)}: {I3}";
        }
    }
}