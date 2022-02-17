using System;
using System.IO;
using UnityEngine.Assertions;

namespace Twisted.PS
{
    public sealed class DMDNode0B06 : DMDNode
    {
        public DMDNode0B06(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Assert.IsTrue(parent is DMDNode0107);

            var bytes = reader.ReadBytes(8);

            Assert.AreEqual((byte)0, bytes[0]);
            Assert.AreEqual((byte)0, bytes[1]);
            Assert.AreEqual((byte)0, bytes[2]);
            Assert.AreEqual((byte)0, bytes[3]);

            Assert.AreEqual((byte)0, bytes[4]);
            Assert.AreEqual((byte)0, bytes[5]);

            var count = bytes[6];

            Assert.AreEqual((byte)0, bytes[7]);

            switch (count)
            {
                case 1:
                    count = 3;
                    break;
                case 2:
                    count = 4;
                    break;
                default:
                    throw new InvalidDataException(Position.ToString());
            }

            var addresses = ReadAddresses(reader, count);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}