using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Unity.Extensions.Editor
{
    public class TreeViewColumn<T> : MultiColumnHeaderState.Column where T : TreeNode
    {
        public TreeViewColumn(Func<T, object> getter)
        {
            Getter = getter ?? throw new ArgumentNullException(nameof(getter));
        }

        public Func<T, object> Getter { get; }

        public bool IsPrimaryColumn { get; init; }

        public TextAnchor TextAnchor { get; init; }
    }
}