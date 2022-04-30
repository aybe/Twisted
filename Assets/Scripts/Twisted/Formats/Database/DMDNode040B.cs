using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode040B : DMDNode
    {
        public readonly float3 Vector1;

        public DMDNode040B(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(16);

            Assert.AreEqual(default, bytes[13]);

            var ints = MemoryMarshal.Cast<byte, int>(bytes);

            Vector1 = new float3(ints[0], ints[1], ints[2]);

            var count = bytes[14];

            var addresses = ReadAddresses(reader, count);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}