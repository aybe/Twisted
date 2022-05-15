using System;
using UnityEditor;
using UnityEngine;

namespace Twisted.Editor
{
    [FilePath("UserSettings/ViewerSettings.yaml", FilePathAttribute.Location.ProjectFolder)]
    [Serializable]
    internal sealed class ViewerSettings : ScriptableSingleton<ViewerSettings>
    {
        [SerializeField]
        private string? LastDatabase;

        [SerializeField]
        private string? LastFilter;

        [SerializeField]
        private bool EnableFraming = true;

        [SerializeField]
        private bool EnablePolygonGeneration;

        [SerializeField]
        private bool EnableTexture = true;

        [SerializeField]
        private bool EnableTextureAlpha = true;

        [SerializeField]
        private bool EnableVertexColors = true;

        [SerializeField]
        private bool EnablePolygonColors;

        [SerializeField]
        private bool EnableFilteredNodes;

        [SerializeField]
        private bool EnableFilteredSearch;

        public SerializedObject SerializedObject { get; private set; } = null!;

        public SerializedProperty LastDatabaseProperty { get; private set; } = null!;

        public SerializedProperty LastFilterProperty { get; private set; } = null!;

        public SerializedProperty EnableFilteredSearchProperty { get; private set; } = null!;

        public SerializedProperty EnableTextureProperty { get; private set; } = null!;

        public SerializedProperty EnableTextureAlphaProperty { get; private set; } = null!;

        public SerializedProperty EnablePolygonColorsProperty { get; private set; } = null!;

        public SerializedProperty EnableFramingProperty { get; private set; } = null!;

        public SerializedProperty EnablePolygonGenerationProperty { get; private set; } = null!;

        public SerializedProperty EnableVertexColorsProperty { get; private set; } = null!;

        public SerializedProperty EnableFilteredNodesProperty { get; private set; } = null!;

        private void OnEnable()
        {
            // evidently, these guys haven't tested enough their stupid class again
            // why does their singleton shows 'ScriptableSingleton already exists'?
            // and why does it suddenly become read-only once Unity gets restarted?
            // and for how long has this been reported but never addressed by them?

            hideFlags &= ~HideFlags.NotEditable; // fix their stupid, untested mess

            SerializedObject = new SerializedObject(this);

            LastDatabaseProperty            = SerializedObject.FindProperty(nameof(LastDatabase));
            LastFilterProperty              = SerializedObject.FindProperty(nameof(LastFilter));
            EnableFramingProperty           = SerializedObject.FindProperty(nameof(EnableFraming));
            EnablePolygonGenerationProperty = SerializedObject.FindProperty(nameof(EnablePolygonGeneration));
            EnableTextureProperty           = SerializedObject.FindProperty(nameof(EnableTexture));
            EnableTextureAlphaProperty      = SerializedObject.FindProperty(nameof(EnableTextureAlpha));
            EnableVertexColorsProperty      = SerializedObject.FindProperty(nameof(EnableVertexColors));
            EnablePolygonColorsProperty     = SerializedObject.FindProperty(nameof(EnablePolygonColors));
            EnableFilteredNodesProperty     = SerializedObject.FindProperty(nameof(EnableFilteredNodes));
            EnableFilteredSearchProperty    = SerializedObject.FindProperty(nameof(EnableFilteredSearch));
        }

        public void Save()
        {
            Save(true);
        }
    }
}