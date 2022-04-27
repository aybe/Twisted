using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode050B : DMDNode
    {
        private readonly byte Unknown1;

        private readonly byte Unknown2;

        private readonly Vector3 Vector1;

        public DMDNode050B(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var bytes = reader.ReadBytes(36);

            var i16 = MemoryMarshal.Cast<byte, short>(bytes);
            var i32 = MemoryMarshal.Cast<byte, int>(bytes);

            Vector1 = MathMulVec(i16, i32);

            Unknown1 = bytes[32];

            Assert.AreEqual((byte)0, bytes[33]);

            var count = bytes[34];

            Unknown2 = bytes[35];

            var addresses = ReadAddresses(reader, count);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }

        private static Vector3 MathMulVec(Span<short> i16, Span<int> i32) // 800FE9C4
        {
            var x = i32[5];
            var y = i32[6];
            var z = i32[7];

            var u = (x * i16[0] + y * i16[1] + z * i16[2]) / 4096.0f;
            var v = (x * i16[3] + y * i16[4] + z * i16[5]) / 4096.0f;
            var w = (x * i16[6] + y * i16[7] + z * i16[8]) / 4096.0f;

            return new Vector3(u, v, w);
        }

        public IEnumerable<Vector3> GetVectors()
        {
            yield return Vector1;
        }

        public IEnumerable<object> GetUnknowns()
        {
            yield return Unknown1;
            yield return Unknown2;
        }
    }
}