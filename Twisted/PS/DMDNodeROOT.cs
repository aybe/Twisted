using JetBrains.Annotations;

namespace Twisted.PS;

public sealed class DMDNodeROOT : DMDNode
{
    public DMDNodeROOT(DMD dmd, [CanBeNull] DMDNode parent) : base(dmd, parent)
    {
    }
}