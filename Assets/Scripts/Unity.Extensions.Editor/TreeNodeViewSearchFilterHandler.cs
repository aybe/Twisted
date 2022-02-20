using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Unity.Extensions.Editor
{
    public delegate IList<TreeViewItem> TreeNodeViewSearchFilterHandler(string searchString, IList<TreeViewItem> items);
}