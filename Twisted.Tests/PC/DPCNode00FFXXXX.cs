using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.Tests.PC;

public sealed class DPCNode00FFXXXX : DPCNode
{
    internal DPCNode00FFXXXX(DPCNodeReader reader, out int[] children) : base(reader)
    {
        // todo maybe size is 108

        reader.Position += 4;

        var addresses = reader.ReadAddresses(3);

        children = Array.Empty<int>();

        // TODO vertices, normals, polygons ???

        var b1 = reader.ReadByte();
        var b2 = reader.ReadByte();
        var b3 = reader.ReadByte();
        var b4 = reader.ReadByte();

        Assert.AreNotEqual((byte)0, b1);
        Assert.AreEqual((byte)0, b2);
        Assert.AreEqual((byte)0, b3);
        Assert.AreEqual((byte)0, b4);

        // try read polygons

        var address = addresses[2];

        reader.Position = address;

        for (var i = 0; i < b1; i++)
        {
            var position = reader.Position;

            var peek = reader.Peek(s => s.ReadUInt32(Endianness.BE));

            switch (peek)
            {
                case 0x00010604:
                    reader.ReadBytes(24); // TODO maybe
                    break;
                case 0x00010605:
                    reader.ReadBytes(24); // TODO maybe
                    break;
                case 0x00010705:
                    reader.ReadBytes(28); // TODO maybe
                    break;
                case 0x00010806:
                    reader.ReadBytes(32); // TODO maybe
                    break;
                case 0x00010808:
                    reader.ReadBytes(32); // TODO maybe
                    break;
                case 0x03010A07:
                    reader.ReadBytes(40); // TODO maybe
                    break;
                case 0x04010B09:
                    reader.ReadBytes(44); // TODO maybe
                    break;
                case 0x04010C09:
                    reader.ReadBytes(48); // TODO maybe
                    break;
                case 0x83010B07:
                    reader.ReadBytes(44); // TODO maybe
                    break;
                case 0x84010C09:
                    reader.ReadBytes(48); // TODO maybe
                    break;
                case 0x84010D09:
                    reader.ReadBytes(52); // TODO maybe
                    break;
                default:
                    throw new NotSupportedException($"Unknown polygon 0x{peek:X8} @ {position}.");
            }
        }
    }
}