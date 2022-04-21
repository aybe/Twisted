using Unity.Extensions;
using UnityEngine.UIElements;

namespace Editor
{
    public delegate void TreeViewContextMenuHandler<in T>(T node, DropdownMenu menu) where T : TreeNode;
}