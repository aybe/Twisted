using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode050B : DMDNode
    {
        private readonly byte Unknown1;

        private readonly byte Unknown2;

        private readonly Vector3 Vector1;

        public readonly float3x3 Rotation;

        public DMDNode050B(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(36);

            var i16 = MemoryMarshal.Cast<byte, short>(bytes);
            var i32 = MemoryMarshal.Cast<byte, int>(bytes);

            Rotation = new float3x3(i16[0], i16[1], i16[2], i16[3], i16[4], i16[5], i16[6], i16[7], i16[8]);

            var pos = new float3(i32[5], i32[6], i32[7]);

            Vector1 = pos;

            Unknown1 = bytes[32];

            Assert.AreEqual((byte)0, bytes[33]);

            var count = bytes[34];

            Unknown2 = bytes[35];

            var addresses = ReadAddresses(reader, count);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}