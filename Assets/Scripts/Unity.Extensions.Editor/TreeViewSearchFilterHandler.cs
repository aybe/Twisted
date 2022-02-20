using System.Collections.Generic;

namespace Unity.Extensions.Editor
{
    public delegate IList<TreeViewItem<T>> TreeViewSearchFilterHandler<T>(string searchString, IEnumerable<TreeViewItem<T>> items) where T : TreeNode;
}