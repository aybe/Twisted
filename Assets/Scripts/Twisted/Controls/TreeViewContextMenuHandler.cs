using UnityEngine.UIElements;

namespace Twisted.Controls
{
    public delegate void TreeViewContextMenuHandler<in T>(T node, DropdownMenu menu) where T : TreeNode;
}