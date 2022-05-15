using System.Collections.Generic;
using Twisted.Formats.Database;

namespace Twisted.Editor
{
    internal sealed class ViewerComparer : EqualityComparer<DMDNode>
    {
        public static EqualityComparer<DMDNode> Instance { get; } = new ViewerComparer();

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