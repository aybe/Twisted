using UnityEditor.IMGUI.Controls;

namespace Unity.Extensions.Editor
{
    public class TreeViewItem<T> : TreeViewItem
    {
        public TreeViewItem(int id, int depth, string? displayName = default, T? data = default)
            : base(id, depth, displayName)
        {
            Data = data;
        }

        public T? Data { get; set; }
    }
}