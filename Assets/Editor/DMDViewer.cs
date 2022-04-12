using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public sealed class DMDViewer : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        public void CreateGUI()
        {
            var root = rootVisualElement;

            root.Add(VisualTreeAsset.Instantiate());
        }

        [MenuItem("Twisted/DMD Viewer (UI Elements)")]
        public static void ShowExample()
        {
            var window = GetWindow<DMDViewer>();

            window.titleContent = new GUIContent("DMDViewer");
        }
    }
}