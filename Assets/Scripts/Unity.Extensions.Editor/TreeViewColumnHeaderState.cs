using System;
using UnityEditor.IMGUI.Controls;

namespace Unity.Extensions.Editor
{
    [Serializable]
    public class TreeViewColumnHeaderState : MultiColumnHeaderState
        // this isn't really needed just for architectural symmetry
    {
        public TreeViewColumnHeaderState(Column[] columns) : base(columns)
        {
        }
    }
}