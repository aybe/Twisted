using System.IO;
using Twisted;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    internal sealed class DMDViewerModel : ScriptableObject
    {
        [SerializeField]
        public string CurrentFile;

        [SerializeField]
        public bool UseDistinctFiltering;

        [SerializeField]
        public bool UseSelectionFraming;

        [SerializeField]
        public bool UseModelSplitting;

        [SerializeField]
        public bool UseTexturing;

        [SerializeField]
        public bool UseVertexColors;

        [SerializeField]
        public bool UsePolygonColors;

        public SerializedProperty CurrentFileProperty = null!;

        public DMDFactory? DMDFactory;

        public SerializedProperty UseDistinctFilteringProperty = null!;

        public SerializedProperty UseModelSplittingProperty = null!;

        public SerializedProperty UsePolygonColorsProperty = null!;

        public SerializedProperty UseSelectionFramingProperty = null!;

        public SerializedProperty UseTexturingProperty = null!;

        public SerializedProperty UseVertexColorsProperty = null!;

        public SerializedObject SerializedObject { get; private set; } = null!;

        private void OnEnable()
        {
            SerializedObject = new SerializedObject(this);

            CurrentFileProperty          = SerializedObject.FindProperty(nameof(CurrentFile));
            UseDistinctFilteringProperty = SerializedObject.FindProperty(nameof(UseDistinctFiltering));
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

            InitializeFactory();
        }

        public void Initialize()
        {
            InitializeFactory();
        }

        private void InitializeFactory()
        {
            if (File.Exists(CurrentFile))
            {
                DMDFactory = DMDFactory.Create(CurrentFile);
            }
        }
    }
}