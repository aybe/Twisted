namespace Twisted.PS;

public sealed class DMDNode00FFXXXX : DMDNode
{
    public DMDNode00FFXXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        dmd.ReadInt32LE();

        dmd.ReadAddresses(3); // TODO DATA ?
    }
}