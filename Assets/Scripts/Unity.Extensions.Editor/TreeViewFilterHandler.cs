namespace Unity.Extensions.Editor
{
    public delegate bool? TreeViewFilterHandler<in T>(T node, string column, string content, string filter) where T : TreeNode;
}