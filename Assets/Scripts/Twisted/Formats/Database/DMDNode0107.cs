using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode0107 : DMDNode
    {
        private readonly int Unknown1;

        public readonly float3 BoundsCenter;

        public DMDNode0107(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(20);

            Assert.AreEqual((byte)0, bytes[19]);

            var ints = MemoryMarshal.Cast<byte, int>(bytes);

            BoundsCenter = new float3(ints[0], ints[1], ints[2]);

            Unknown1 = ints[3];

            var count = bytes[16];

            var addresses = ReadAddresses(reader, count);

            // TODO there may be 76 more bytes in here

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}