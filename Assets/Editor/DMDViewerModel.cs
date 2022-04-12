using UnityEditor;
using UnityEngine;

namespace Editor
{
    internal sealed class DMDViewerModel : ScriptableObject
    {
        [SerializeField]
        private string CurrentFile;

        [SerializeField]
        private bool UseDistinctFiltering;

        [SerializeField]
        private bool UseRegexSearch;

        [SerializeField]
        private bool UseSelectionFraming;

        [SerializeField]
        private bool UseModelSplitting;

        [SerializeField]
        private bool UseTexturing;

        [SerializeField]
        private bool UseVertexColors;

        [SerializeField]
        private bool UsePolygonColors;

        public SerializedProperty CurrentFileProperty = null!;

        public SerializedProperty UseDistinctFilteringProperty = null!;

        public SerializedProperty UseModelSplittingProperty = null!;

        public SerializedProperty UsePolygonColorsProperty = null!;

        public SerializedProperty UseRegexSearchProperty = null!;

        public SerializedProperty UseSelectionFramingProperty = null!;

        public SerializedProperty UseTexturingProperty = null!;

        public SerializedProperty UseVertexColorsProperty = null!;

        public SerializedObject SerializedObject { get; private set; } = null!;

        private void OnEnable()
        {
            SerializedObject = new SerializedObject(this);

            CurrentFileProperty          = SerializedObject.FindProperty(nameof(CurrentFile));
            UseDistinctFilteringProperty = SerializedObject.FindProperty(nameof(UseDistinctFiltering));
            UseRegexSearchProperty       = SerializedObject.FindProperty(nameof(UseRegexSearch));
            UseSelectionFramingProperty  = SerializedObject.FindProperty(nameof(UseSelectionFraming));
            UseModelSplittingProperty    = SerializedObject.FindProperty(nameof(UseModelSplitting));
            UseTexturingProperty         = SerializedObject.FindProperty(nameof(UseTexturing));
            UseVertexColorsProperty      = SerializedObject.FindProperty(nameof(UseVertexColors));
            UsePolygonColorsProperty     = SerializedObject.FindProperty(nameof(UsePolygonColors));
        }

        public void OpenFile()
        {
            var path = EditorUtility.OpenFilePanel(null, null, "DMD");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            CurrentFile = path;

            LoadFile();
        }

        private void LoadFile()
        {
        }
    }
}