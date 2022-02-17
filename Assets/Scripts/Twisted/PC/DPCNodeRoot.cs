using Unity.Extensions.General;

namespace Twisted.PC
{
    public sealed class DPCNodeRoot : DPCNode
    {
        internal DPCNodeRoot(DPCNodeReader reader, out int[] children, DPCNode? parent = null) : base(reader, parent)
        {
            var address = reader.ReadAddress();

            reader.Position = address;

            var count = reader.ReadInt32(Endianness.LE);

            var addresses = reader.ReadAddresses(count);

            SetLength(reader);

            children = addresses;
        }
    }
}