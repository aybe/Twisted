using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Mathematics.math;
using float3x3 = Unity.Mathematics.float3x3;

namespace Twisted.Editor
{
    internal sealed class MatrixCalculatorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        private static MatrixCalculatorWorkspace Workspace => MatrixCalculatorWorkspace.instance;

        private void OnDisable()
        {
            Workspace.Save();
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            root.Add(VisualTreeAsset.Instantiate());

            var r0 = root.Q<Vector3Field>("r0");
            var r1 = root.Q<Vector3Field>("r1");
            var r2 = root.Q<Vector3Field>("r2");
            var vs = root.Q<Vector3Field>("vs");
            var vt = root.Q<Vector3Field>("vt");
            var vi = root.Q<Vector3Field>("vi");
            var vo = root.Q<Vector3Field>("vo");

            r0.SetLabels("m00", "m01", "m02");
            r1.SetLabels("m10", "m11", "m12");
            r2.SetLabels("m20", "m21", "m22");

            SetFields(r0, r1, r2, vs, vt, vi, vo);

            r0.BindProperty(Workspace.Rotation0Property);
            r1.BindProperty(Workspace.Rotation1Property);
            r2.BindProperty(Workspace.Rotation2Property);
            vs.BindProperty(Workspace.ScaleProperty);
            vt.BindProperty(Workspace.TranslationProperty);
            vi.BindProperty(Workspace.InputProperty);

            void SetFields(params Vector3Field[] fields)
            {
                foreach (var field in fields)
                {
                    field.RegisterValueChangedCallback(OnValueChanged);
                }
            }

            void OnValueChanged(ChangeEvent<Vector3> evt)
            {
                Update();
            }

            void Update()
            {
                var rotation = new float3x3(
                    r0.value.x, r0.value.y, r0.value.z,
                    r1.value.x, r1.value.y, r1.value.z,
                    r2.value.x, r2.value.y, r2.value.z
                );

                const float scale = 4096.0f;

                var translation = new float3(vt.value);

                var trs = float4x4(
                    float4(rotation.c0 * (vs.value.x / scale), 0.0f),
                    float4(rotation.c1 * (vs.value.y / scale), 0.0f),
                    float4(rotation.c2 * (vs.value.z / scale), 0.0f),
                    float4(translation,                        1.0f)
                );

                vo.value = transform(trs, vi.value);
            }
        }

        [MenuItem("Twisted/Matrix Calculator")]
        public static void InitializeWindow()
        {
            var window = GetWindow<MatrixCalculatorWindow>();

            window.titleContent = new GUIContent("Matrix Calculator");

            window.minSize = new Vector2(320, 320);
        }
    }
}