using System.Collections.Generic;
using System.Drawing;

namespace Twisted.PS.Texturing
{
    public interface IFrameBufferObject
    {
        Point Position { get; }

        Size Size { get; }

        IReadOnlyList<byte> Data { get; }
    }
}