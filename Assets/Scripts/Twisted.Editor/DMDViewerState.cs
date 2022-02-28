using System;
using UnityEditor.IMGUI.Controls;

namespace Twisted.Editor
{
    [Serializable]
    internal sealed class DMDViewerState
    {
        public DMDFactory? Factory;

        public string? FactoryPath;

        public bool FactorySplit = true;

        public float ViewHeight = 24;

        public TreeViewState? ViewState;

        public MultiColumnHeaderState? ViewStateHeader;
    }
}