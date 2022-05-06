using UnityEditor;
using UnityEngine;

namespace Twisted.Editor
{
    [FilePath("UserSettings/MatrixCalculatorWorkspace.yaml", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class MatrixCalculatorWorkspace : ScriptableSingleton<MatrixCalculatorWorkspace>
    {
        [SerializeField]
        private Vector3 Rotation0;

        [SerializeField]
        private Vector3 Rotation1;

        [SerializeField]
        private Vector3 Rotation2;

        [SerializeField]
        private Vector3 Scale;

        [SerializeField]
        private Vector3 Translation;

        [SerializeField]
        private Vector3 Input;

        public SerializedProperty InputProperty;

        public SerializedProperty Rotation0Property;

        public SerializedProperty Rotation1Property;

        public SerializedProperty Rotation2Property;

        public SerializedProperty ScaleProperty;

        public SerializedProperty TranslationProperty;

        public MatrixCalculatorWorkspace()
        {
            TranslationProperty = null!;
            ScaleProperty       = null!;
            Rotation2Property   = null!;
            Rotation1Property   = null!;
            Rotation0Property   = null!;
            InputProperty       = null!;
        }

        public void Reset()
        {
            Rotation0   = new Vector3(1,    2,    3);
            Rotation1   = new Vector3(4,    5,    6);
            Rotation2   = new Vector3(7,    8,    9);
            Scale       = new Vector3(1,    1,    1);
            Translation = new Vector3(0,    0,    0);
            Input       = new Vector3(1000, 2000, 3000);
        }

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