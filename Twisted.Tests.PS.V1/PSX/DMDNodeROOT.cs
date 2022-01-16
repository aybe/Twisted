using JetBrains.Annotations;

namespace Twisted.Tests.PS.V1.PSX;

public sealed class DMDNodeROOT : DMDNode
{
    public DMDNodeROOT(DMD dmd, [CanBeNull] DMDNode parent) : base(dmd, parent)
    {
    }
}