using UnityEditor;
using UnityEngine;

namespace Twisted.Editor
{
    [FilePath("UserSettings/MatrixCalculatorWorkspace.yaml", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class MatrixCalculatorWorkspace : ScriptableSingleton<MatrixCalculatorWorkspace>
    {
        [SerializeField]
        private Vector3 Rotation0 = new(1, 2, 3);

        [SerializeField]
        private Vector3 Rotation1 = new(4, 5, 6);

        [SerializeField]
        private Vector3 Rotation2 = new(7, 8, 9);

        [SerializeField]
        private Vector3 Scale = new(1, 1, 1);

        [SerializeField]
        private Vector3 Translation = new(0, 0, 0);

        [SerializeField]
        private Vector3 Input = new(1000, 2000, 3000);

        public SerializedProperty InputProperty = null!;

        public SerializedProperty Rotation0Property = null!;

        public SerializedProperty Rotation1Property = null!;

        public SerializedProperty Rotation2Property = null!;

        public SerializedProperty ScaleProperty = null!;

        public SerializedProperty TranslationProperty = null!;

        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;

            var serializedObject = new SerializedObject(this);

            Rotation0Property   = serializedObject.FindProperty(nameof(Rotation0));
            Rotation1Property   = serializedObject.FindProperty(nameof(Rotation1));
            Rotation2Property   = serializedObject.FindProperty(nameof(Rotation2));
            ScaleProperty       = serializedObject.FindProperty(nameof(Scale));
            TranslationProperty = serializedObject.FindProperty(nameof(Translation));
            InputProperty       = serializedObject.FindProperty(nameof(Input));
        }
        
        public void Save()
        {
            Save(true);
        }
    }
}