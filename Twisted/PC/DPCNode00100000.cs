namespace Twisted.PC;

public sealed class DPCNode00100000 : DPCNode
{
    internal DPCNode00100000(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
    {
        var bytes = reader.ReadBytes(16);

        SetLength(reader);

        children = Array.Empty<int>();
    }
}