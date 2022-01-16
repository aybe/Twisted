using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Twisted.Extensions;
using Twisted.Tests.PS.V1.Extensions;

namespace Twisted.Tests.PS.V1.Graphics;

public sealed class TimClutBlock : IReadOnlyList<TimClut>
{
    public TimClutBlock(BinaryReader reader, TimPixelMode pixelMode)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var bnum = reader.ReadInt32();

        FrameBufferX = reader.ReadInt16();
        FrameBufferY = reader.ReadInt16();
        PixelWidth   = reader.ReadInt16();
        PixelHeight  = reader.ReadInt16();

        var clutBytes = reader.ReadBytes(bnum - 12);
        var clutWidth = pixelMode.GetClutWidth();
        var clutCount = PixelWidth * PixelHeight / clutWidth;

        using var clutReader = new BinaryReader(new MemoryStream(clutBytes));

        Cluts = clutReader.Read(s => new TimClut(s.Read(t => new TimClutEntry(t), clutWidth)), clutCount);
    }

    private IReadOnlyList<TimClut> Cluts { get; }

    public short FrameBufferX { get; }

    public short FrameBufferY { get; }

    public short PixelWidth { get; }

    public short PixelHeight { get; }

    public override string ToString()
    {
        return $"{nameof(FrameBufferX)}: {FrameBufferX}, " +
               $"{nameof(FrameBufferY)}: {FrameBufferY}, " +
               $"{nameof(PixelWidth)}: {PixelWidth}, " +
               $"{nameof(PixelHeight)}: {PixelHeight}, " +
               $"{nameof(Cluts)}: {Cluts.Count}";
    }

    #region IReadOnlyList<TimClut> Members

    public TimClut this[int index] => Cluts[index];

    public int Count => Cluts.Count;

    public IEnumerator<TimClut> GetEnumerator()
    {
        return Cluts.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Cluts).GetEnumerator();
    }

    #endregion
}