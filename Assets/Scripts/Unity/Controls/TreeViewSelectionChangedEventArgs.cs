using System;
using Unity.Extensions;

namespace Unity.Controls
{
    public sealed class TreeViewSelectionChangedEventArgs<T> : EventArgs where T : TreeNode
    {
        public TreeViewSelectionChangedEventArgs(T[] items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public T[] Items { get; }
    }
}