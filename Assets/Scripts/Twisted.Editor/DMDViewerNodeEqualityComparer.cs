using System.Collections.Generic;
using Twisted.Formats.Database;

namespace Editor
{
    internal sealed class DMDViewerNodeEqualityComparer : EqualityComparer<DMDNode>
    {
        public static EqualityComparer<DMDNode> Instance { get; } = new DMDViewerNodeEqualityComparer();

        public override bool Equals(DMDNode x, DMDNode y)
        {
            return x.Position.Equals(y.Position);
        }

        public override int GetHashCode(DMDNode obj)
        {
            return obj.Position.GetHashCode();
        }
    }
}