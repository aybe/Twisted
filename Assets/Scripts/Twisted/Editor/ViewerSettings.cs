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
        private bool UseFilterDistinct;

        [SerializeField]
        private bool UseModelTexture;

        [SerializeField]
        private bool UseTextureAlpha = true;

        [SerializeField]
        private bool UseSceneFrame;

        [SerializeField]
        private bool UseSplitModel;

        [SerializeField]
        private bool UseVertexColors;

        [SerializeField]
        private bool UsePolygonColors;

        public SerializedObject SerializedObject { get; private set; } = null!;

        public SerializedProperty LastDatabaseProperty { get; private set; } = null!;

        public SerializedProperty LastFilterProperty { get; private set; } = null!;

        public SerializedProperty UseFilterDistinctProperty { get; private set; } = null!;

        public SerializedProperty UseModelTextureProperty { get; private set; } = null!;

        public SerializedProperty UseTextureAlphaProperty { get; private set; } = null!;

        public SerializedProperty UsePolygonColorsProperty { get; private set; } = null!;

        public SerializedProperty UseSceneFrameProperty { get; private set; } = null!;

        public SerializedProperty UseSplitModelProperty { get; private set; } = null!;

        public SerializedProperty UseVertexColorsProperty { get; private set; } = null!;

        private void OnEnable()
        {
            // evidently, these guys haven't tested enough their stupid class again
            // why does their singleton shows 'ScriptableSingleton already exists'?
            // and why does it suddenly become read-only once Unity gets restarted?
            // and for how long has this been reported but never addressed by them?

            hideFlags &= ~HideFlags.NotEditable; // fix their stupid, untested mess

            SerializedObject = new SerializedObject(this);

            LastDatabaseProperty      = SerializedObject.FindProperty(nameof(LastDatabase));
            LastFilterProperty        = SerializedObject.FindProperty(nameof(LastFilter));
            UseFilterDistinctProperty = SerializedObject.FindProperty(nameof(UseFilterDistinct));
            UseModelTextureProperty   = SerializedObject.FindProperty(nameof(UseModelTexture));
            UseTextureAlphaProperty   = SerializedObject.FindProperty(nameof(UseTextureAlpha));
            UseSceneFrameProperty     = SerializedObject.FindProperty(nameof(UseSceneFrame));
            UseSplitModelProperty     = SerializedObject.FindProperty(nameof(UseSplitModel));
            UseVertexColorsProperty   = SerializedObject.FindProperty(nameof(UseVertexColors));
            UsePolygonColorsProperty  = SerializedObject.FindProperty(nameof(UsePolygonColors));
        }

        public void Save()
        {
            Save(true);
        }
    }
}