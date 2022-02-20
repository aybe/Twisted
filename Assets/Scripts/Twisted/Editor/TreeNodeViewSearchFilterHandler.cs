using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Twisted.Editor
{
    public delegate IList<TreeViewItem> TreeNodeViewSearchFilterHandler(string searchString, IList<TreeViewItem> items);
}