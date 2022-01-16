// ReSharper disable RedundantUsingDirective

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twisted.PS.V1;

public static class DMDNodeReader
{
    [SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
    [SuppressMessage("Style",     "IDE0066:Convert switch statement to expression", Justification = "<Pending>")]
    // ReSharper disable once UnusedMethodReturnValue.Global // TODO this is for clarity
    public static DMDNode Read(DMD dmd, DMDNode parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        var position = dmd.Position;

        var code = dmd.Peek(dmd.ReadUInt32BE);

        switch (code)
        {
            case 0x00093D00: return ReadGeometry(dmd, parent);
            case 0x00102700: return ReadGeometry(dmd, parent);
            case 0x00190000: return ReadGeometry(dmd, parent);
            case 0x00405A00: return ReadGeometry(dmd, parent);
            case 0x00407A10: return ReadGeometry(dmd, parent);
            case 0x00640000: return ReadGeometry(dmd, parent);
            case 0x00710200: return ReadGeometry(dmd, parent);
            case 0x00C40900: return ReadGeometry(dmd, parent);
            case 0x00E10000: return ReadGeometry(dmd, parent);
            case 0x00E45700: return ReadGeometry(dmd, parent);
            case 0x00F91500: return ReadGeometry(dmd, parent);
            case 0x04170300: return ReadGeometry(dmd, parent);
            case 0x093D0000: return ReadGeometry(dmd, parent);
            case 0x10270000: return ReadGeometry(dmd, parent);
            case 0x10552200: return ReadGeometry(dmd, parent);
            case 0x105C0C00: return ReadGeometry(dmd, parent);
            case 0x105E5F00: return ReadGeometry(dmd, parent);
            case 0x107A0700: return ReadGeometry(dmd, parent);
            case 0x108C5A00: return ReadGeometry(dmd, parent);
            case 0x24F40000: return ReadGeometry(dmd, parent);
            case 0x40380000: return ReadGeometry(dmd, parent);
            case 0x40420F00: return ReadGeometry(dmd, parent);
            case 0x40703100: return ReadGeometry(dmd, parent);
            case 0x407E0500: return ReadGeometry(dmd, parent);
            case 0x409C0000: return ReadGeometry(dmd, parent);
            case 0x40E81D00: return ReadGeometry(dmd, parent);
            case 0x40F40200: return ReadGeometry(dmd, parent);
            case 0x44060B00: return ReadGeometry(dmd, parent);
            case 0x44950800: return ReadGeometry(dmd, parent);
            case 0x44D20200: return ReadGeometry(dmd, parent);
            case 0x51250200: return ReadGeometry(dmd, parent);
            case 0x51955900: return ReadGeometry(dmd, parent);
            case 0x643F4D00: return ReadGeometry(dmd, parent);
            case 0x64720600: return ReadGeometry(dmd, parent);
            case 0x69270100: return ReadGeometry(dmd, parent);
            case 0x84DE0100: return ReadGeometry(dmd, parent);
            case 0x904A4300: return ReadGeometry(dmd, parent);
            case 0x905F0100: return ReadGeometry(dmd, parent);
            case 0x90761200: return ReadGeometry(dmd, parent);
            case 0x907E0000: return ReadGeometry(dmd, parent);
            case 0x90C91900: return ReadGeometry(dmd, parent);
            case 0x90D00300: return ReadGeometry(dmd, parent);
            case 0x90EBBA00: return ReadGeometry(dmd, parent);
            case 0xA1770000: return ReadGeometry(dmd, parent);
            case 0xC1C50000: return ReadGeometry(dmd, parent);
            case 0xE4570000: return ReadGeometry(dmd, parent);
        }

        /*
         * according to dbScanForInteractiveStuff
         * 0 ok
         * 1 ok
         * 2 ok
         * 3 ok
         * 4 ok
         * 5 ok
         * 6 never encountered, 7 ok, 8 ok
         * 9 ok
         * B ok
         * anything else is bad op code
         */

        var mask = code & 0xFFFF0000;

        switch (mask)
        {
            case 0x01070000: return new DMDNode0107XXXX(dmd, parent);

            case 0x02060000: return new DMDNode02XXXXXX(dmd, parent);
            case 0x02070000: return new DMDNode02XXXXXX(dmd, parent);
            case 0x02080000: return new DMDNode02XXXXXX(dmd, parent);
            case 0x02090000: return new DMDNode02XXXXXX(dmd, parent);
            case 0x020A0000: return new DMDNode02XXXXXX(dmd, parent);

            case 0x03050000: return new DMDNode0305XXXX(dmd, parent);

            case 0x040B0000: return new DMDNode040BXXXX(dmd, parent);

            case 0x050B0000: return new DMDNode050BXXXX(dmd, parent);

            case 0x07FF0000: return new DMDNode07FFXXXX(dmd, parent);

            case 0x08FF0000: return new DMDNode08FFXXXX(dmd, parent);

            case 0x09030000: return new DMDNode0903XXXX(dmd, parent);

            case 0x0B060000: return new DMDNode0B06XXXX(dmd, parent);

            case 0x00FF0000: return new DMDNode00FFXXXX(dmd, parent);
            case 0x00100000: return new DMDNode0010XXXX(dmd, parent);
            case 0x00F00000: return new DMDNode0010XXXX(dmd, parent);

            default:
                throw new ArgumentOutOfRangeException(nameof(code), $"0x{code:X8}", $"Unknown code at {position}.");
        }
    }

    private static DMDNode ReadGeometry(DMD dmd, DMDNode parent)
    {
        Assert.IsTrue(parent is DMDNode02XXXXXX);

        Trace.WriteLine($"{nameof(ReadGeometry)}: {nameof(dmd.Position)}: {dmd.Position}");
        dmd.ReadInt32BE();
        dmd.ReadInt32BE();
        Read(dmd, parent, 1);
        return null;
    }

    public static void Read(DMD dmd, DMDNode parent, int count)
    {
        var addresses = dmd.ReadAddresses(count);

        foreach (var address in addresses)
        {
            dmd.Position = address;

            Read(dmd, parent);
        }
    }
}