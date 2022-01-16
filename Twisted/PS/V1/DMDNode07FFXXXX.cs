namespace Twisted.PS.V1;

public sealed class DMDNode07FFXXXX : DMDNode
{
    public DMDNode07FFXXXX(DMD dmd, DMDNode parent) : base(dmd, parent)
    {
        if (dmd == null)
            throw new ArgumentNullException(nameof(dmd));

        var code = dmd.ReadInt32BE();

        dmd.Read(dmd.ReadInt32LE, 13); // vectors and normals ?
    }
}