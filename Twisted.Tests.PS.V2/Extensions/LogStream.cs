using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Twisted.Tests.PS.V2.Extensions;

public sealed class LogStream : Stream
{
    private readonly bool LeaveOpen;

    private readonly List<LogStreamRange> RangesReading = new();

    private readonly List<LogStreamRange> RangesWriting = new();

    private readonly Stream Stream;

    public LogStream(Stream stream, bool leaveOpen = false)
    {
        Stream    = stream ?? throw new ArgumentNullException(nameof(stream));
        LeaveOpen = leaveOpen;
    }

    public override bool CanRead => Stream.CanRead;

    public override bool CanSeek => Stream.CanSeek;

    public override bool CanWrite => Stream.CanWrite;

    public override long Length => Stream.Length;

    public override long Position
    {
        get => Stream.Position;
        set => Stream.Position = value;
    }

    public override void Flush()
    {
        Stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!CanRead)
            throw new NotSupportedException("Underlying stream is not readable.");

        var position = Position;

        var read = Stream.Read(buffer, offset, count);

        var range = new LogStreamRange(position, Position);

        RangesReading.Add(range);

        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return Stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        Stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (!CanWrite)
            throw new NotSupportedException("Underlying stream is not writable.");

        var position = Position;

        Stream.Write(buffer, offset, count);

        var range = new LogStreamRange(position, count);

        RangesWriting.Add(range);
    }

    protected override void Dispose(bool disposing)
    {
        if (!LeaveOpen)
        {
            Stream.Dispose();
        }

        base.Dispose(disposing);
    }

    public IReadOnlyList<LogStreamRange> GetRanges(LogStreamInfo info)
    {
        var r = info.HasFlag(LogStreamInfo.Reading);
        var w = info.HasFlag(LogStreamInfo.Writing);
        var s = info.HasFlag(LogStreamInfo.Skipped);
        var v = info.HasFlag(LogStreamInfo.Visited);

        if (r && w || s && v)
            throw new ArgumentOutOfRangeException(nameof(info));

        switch (r)
        {
            case true when s:
                return GetRangesSkipped(RangesReading);
            case true when v:
                return GetRangesVisited(RangesReading);
        }

        switch (w)
        {
            case true when s:
                return GetRangesSkipped(RangesWriting);
            case true when v:
                return GetRangesVisited(RangesReading);
        }

        throw new InvalidOperationException();
    }

    private static LinkedList<LogStreamRange> GetRangesOrdered(List<LogStreamRange> ranges)
    {
        if (ranges == null)
            throw new ArgumentNullException(nameof(ranges));

        var ordered = new LinkedList<LogStreamRange>(ranges.OrderBy(s => s.Start).ThenBy(s => s.End));

        return ordered;
    }

    private IReadOnlyList<LogStreamRange> GetRangesSkipped(List<LogStreamRange> ranges)
    {
        if (ranges == null)
            throw new ArgumentNullException(nameof(ranges));

        var source = GetRangesOrdered(ranges);
        var target = new LinkedList<LogStreamRange>();

        var node = source.First;

        while (node != null)
        {
            var next = node.Next;

            if (next == null) // last node, exit loop
                break;

            if (next.Value.Start <= node.Value.End) // if consecutive or overlapping, continue
            {
                node = next;
            }
            else // there is a gap, record it
            {
                var hunk = new LogStreamRange(node.Value.End, next.Value.Start);
                target.AddLast(hunk);
                node = next;
            }
        }

        if (source.First != null && source.First.Value.Start > 0) // record start gap, if any
            target.AddFirst(new LogStreamRange(0, source.First.Value.Start));

        if (source.Last != null && source.Last.Value.End < Length) // record end gap, if any
            target.AddLast(new LogStreamRange(source.Last.Value.End, Length));

        return target.ToList().AsReadOnly();
    }

    private static IReadOnlyList<LogStreamRange> GetRangesVisited(List<LogStreamRange> ranges)
    {
        if (ranges == null)
            throw new ArgumentNullException(nameof(ranges));

        var source = GetRangesOrdered(ranges);
        var target = new LinkedList<LogStreamRange>(source);

        var node = target.First;

        while (node != null)
        {
            var next = node.Next;

            if (next == null) // last node, exit loop
                break;

            if (next.Value.Start <= node.Value.End) // if consecutive or overlapping, consolidate
            {
                var hunk = new LogStreamRange(node.Value.Start, next.Value.End);
                var item = target.AddBefore(node, hunk);
                target.Remove(node);
                target.Remove(next);
                node = item;
            }
            else // there is a gap, continue
            {
                node = next;
            }
        }

        return target.ToList().AsReadOnly();
    }
}