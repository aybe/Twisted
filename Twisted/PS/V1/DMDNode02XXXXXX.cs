using System.Diagnostics;
using JetBrains.Annotations;
using Twisted.Extensions;

namespace Twisted.PS.V1;

[Obsolete(null, true)]
[NoReorder]
public sealed class DMDNode02XXXXXX : DMDNode
{
    public readonly short Code;
    public readonly short Unknown1;
    public readonly int   Unknown2;
    public readonly int   Unknown3;
    public readonly int   Unknown4;
    public readonly byte  Addresses;
    public readonly byte  Byte2;
    public readonly byte  Byte3;
    public readonly byte  Byte4;

    public DMDNode02XXXXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        Code     = dmd.ReadInt16BE();
        Unknown1 = dmd.ReadInt16LE();
        Unknown2 = dmd.ReadInt32LE();
        Unknown3 = dmd.ReadInt32LE();
        Unknown4 = dmd.ReadInt32LE();

        var (byte1, byte2, byte3, byte4) = dmd.ReadInt32LE();

        Addresses = byte1;
        Byte2     = byte2;
        Byte3     = byte3;
        Byte4     = byte4;

        Debug.WriteLine(
            $"{nameof(Code)}: 0x{Code:X4}, " +
            $"{nameof(Unknown1)}: {Unknown1}, " +
            $"{nameof(Unknown2)}: {Unknown2}, " +
            $"{nameof(Unknown3)}: {Unknown3}, " +
            $"{nameof(Unknown4)}: {Unknown4}, " +
            $"{nameof(Addresses)}: {Addresses}, " +
            $"{nameof(Byte2)}: {Byte2}, " +
            $"{nameof(Byte3)}: {Byte3}, " +
            $"{nameof(Byte4)}: {Byte4}");

        DMDNodeReader.Read(dmd, this, Addresses);
    }
}