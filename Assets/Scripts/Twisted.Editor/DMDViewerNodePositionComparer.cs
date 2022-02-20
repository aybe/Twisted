using System.Collections.Generic;
using Twisted.Graphics;
using Unity.Extensions;

namespace Twisted.Editor
{
    internal sealed class DMDViewerNodePositionComparer : EqualityComparer<TreeNode>
    {
        public static DMDViewerNodePositionComparer Instance { get; } = new();

        public override bool Equals(TreeNode x, TreeNode y)
        {
            return x is DMDNode a && y is DMDNode b && a.Position == b.Position;
        }

        public override int GetHashCode(TreeNode obj)
        {
            return obj is DMDNode node ? node.Position.GetHashCode() : obj.GetHashCode();
        }
    }
}