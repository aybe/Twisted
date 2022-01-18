namespace Twisted.Tests;

internal sealed class TreeNodeTest : TreeNode<TreeNodeTest>
{
    public TreeNodeTest(TreeNodeTest? parent = null) : base(parent)
    {
    }
}