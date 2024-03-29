﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine.Assertions;

namespace Twisted.Formats.Database
{
    public sealed class DMDNode0903 : DMDNode
    {
        [SuppressMessage("ReSharper", "UnusedVariable")]
        [SuppressMessage("Style",     "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public DMDNode0903(DMDNode? parent, BinaryReader reader)
            : base(parent, reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var b1 = reader.ReadByte();
            var b2 = reader.ReadByte();
            var b3 = reader.ReadByte();
            var b4 = reader.ReadByte();

            // Assert.AreEqual(0, b1);

            Assert.AreEqual((byte)0, b2);

            Assert.AreNotEqual((byte)0, b3);

            Assert.AreEqual((byte)0, b4);

            var addresses = ReadAddresses(reader, b3);

            SetupBinaryObject(reader);

            ReadNodes(this, reader, addresses);
        }
    }
}