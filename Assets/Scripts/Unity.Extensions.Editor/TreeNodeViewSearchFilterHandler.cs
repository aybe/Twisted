using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Unity.Extensions.Editor
{
    public delegate IList<TreeViewItem<T>> TreeNodeViewSearchFilterHandler<T>(string searchString, IEnumerable<TreeViewItem<T>> items) where T : TreeNode;
}