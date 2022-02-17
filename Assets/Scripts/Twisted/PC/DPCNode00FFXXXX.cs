using System;
using Unity.Extensions.Binary;
using UnityEngine.Assertions;

namespace Twisted.PC
{
    public sealed class DPCNode00FFXXXX : DPCNode
    {
        internal DPCNode00FFXXXX(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
        {
            // todo maybe size is 108

            reader.Position += 4;

            var addresses = reader.ReadAddresses(3, false);

            children = Array.Empty<int>();

            // TODO vertices, normals, polygons ???

            Vertices = addresses[0];
            Normals  = addresses[1];
            Polygons = addresses[2];

            var b1 = reader.ReadByte();
            var b2 = reader.ReadByte();
            var b3 = reader.ReadByte();
            var b4 = reader.ReadByte();

            Assert.AreNotEqual((byte)0, b1);

            if (b1 != 0)
            {
                // Assert.Fail($"Maybe polygons @ {ToString()}");
            }

            Assert.AreEqual((byte)0, b2);
            Assert.AreEqual((byte)0, b3);
            Assert.AreEqual((byte)0, b4);

            var data = reader.ReadBytes(88); // BUG this does cross over some data

            SetLength(reader);

            // try read polygons

            var address = addresses[2];

            reader.Position = address;

            for (var i = 0; i < b1; i++)
            {
                var peek = reader.Peek(s => s.ReadUInt32(Endianness.BE));
                var open = reader.Position;

                switch (peek)
                {
                    case 0x00010604:
                    case 0x00010605:
                    {
                        var bytes = reader.ReadBytes(24); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x00010704:
                    case 0x00010705:
                    {
                        var bytes = reader.ReadBytes(28); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x00010806:
                    case 0x00010808:
                    {
                        var bytes = reader.ReadBytes(32); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        var n1 = bytes.ReadInt16(24, Endianness.LE);
                        var n2 = bytes.ReadInt16(26, Endianness.LE);
                        var n3 = bytes.ReadInt16(28, Endianness.LE);
                        var n4 = bytes.ReadInt16(30, Endianness.LE);

                        foreach (var index in new[] { n1, n2, n3, n4 })
                        {
                            reader.Position = Normals + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x03010A07: // TODO are these the double-faced polygons? 
                    {
                        var bytes = reader.ReadBytes(40); // TODO maybe

                        var v1 = bytes.ReadInt16(4, Endianness.LE);
                        var v2 = bytes.ReadInt16(6, Endianness.LE);
                        var v3 = bytes.ReadInt16(8, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        if (false) // BUG reads out of bounds
                        {
                            var n1 = bytes.ReadInt16(24, Endianness.LE);
                            var n2 = bytes.ReadInt16(26, Endianness.LE);
                            var n3 = bytes.ReadInt16(28, Endianness.LE);
                            var n4 = bytes.ReadInt16(30, Endianness.LE);
                            var n5 = bytes.ReadInt16(32, Endianness.LE);
                            var n6 = bytes.ReadInt16(34, Endianness.LE);

                            foreach (var index in new[] { n1, n2, n3, n4, n5, n6 })
                            {
                                reader.Position = Normals + index * 8;
                                reader.ReadBytes(8);
                            }
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x03010B07:
                    {
                        var bytes = reader.ReadBytes(44); // TODO maybe

                        var v1 = bytes.ReadInt16(4, Endianness.LE);
                        var v2 = bytes.ReadInt16(6, Endianness.LE);
                        var v3 = bytes.ReadInt16(8, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x04010B09:
                    {
                        var bytes = reader.ReadBytes(44); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x03010C09:
                    case 0x04010C09:
                    {
                        var bytes = reader.ReadBytes(48); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x04010D0C:
                    {
                        var bytes = reader.ReadBytes(52); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x83010B07: // TODO 0x000000E2 at end
                    {
                        var bytes = reader.ReadBytes(44); // TODO maybe

                        var v1 = bytes.ReadInt16(4, Endianness.LE);
                        var v2 = bytes.ReadInt16(6, Endianness.LE);
                        var v3 = bytes.ReadInt16(8, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x83010C07:
                    {
                        var bytes = reader.ReadBytes(48); // TODO maybe

                        var v1 = bytes.ReadInt16(4, Endianness.LE);
                        var v2 = bytes.ReadInt16(6, Endianness.LE);
                        var v3 = bytes.ReadInt16(8, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x84010C09:
                    {
                        var bytes = reader.ReadBytes(48); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x83010D09:
                    {
                        var bytes = reader.ReadBytes(52); // TODO maybe

                        var v1 = bytes.ReadInt16(4, Endianness.LE);
                        var v2 = bytes.ReadInt16(6, Endianness.LE);
                        var v3 = bytes.ReadInt16(8, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x84010D09:
                    {
                        var bytes = reader.ReadBytes(52); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    case 0x84010E0C:
                    {
                        var bytes = reader.ReadBytes(56); // TODO maybe

                        var v1 = bytes.ReadInt16(4,  Endianness.LE);
                        var v2 = bytes.ReadInt16(6,  Endianness.LE);
                        var v3 = bytes.ReadInt16(8,  Endianness.LE);
                        var v4 = bytes.ReadInt16(10, Endianness.LE);

                        var position = reader.Position;

                        foreach (var index in new[] { v1, v2, v3, v4 })
                        {
                            reader.Position = Vertices + index * 8;
                            reader.ReadBytes(8);
                        }

                        reader.Position = position;
                        continue;
                    }
                    default:
                        throw new NotSupportedException($"Unknown polygon 0x{peek:X8} @ {reader.Position}.");
                }

                Console.WriteLine($"Unprocessed polygon 0x{peek:X8} @ {open}.");
            }
        }

        public int Vertices { get; }

        public int Normals { get; }

        public int Polygons { get; }

        public override string ToString()
        {
            return ToStringVerbose
                ? $"{base.ToString()}, {nameof(Vertices)}: {Vertices}, {nameof(Normals)}: {Normals}, {nameof(Polygons)}: {Polygons}"
                : base.ToString();
        }
    }
}