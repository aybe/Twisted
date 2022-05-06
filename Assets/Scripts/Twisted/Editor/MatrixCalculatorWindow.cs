using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Mathematics.math;

namespace Twisted.Editor
{
    internal sealed class MatrixCalculatorWindow : EditorWindow, IHasCustomMenu
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

                var vec1 = MathMulVec(rotation, new int3(vi.value), false);
                var vec2 = MathMulVec(rotation, new int3(vi.value), true);
                var vec3 = MathMulTransVec(rotation, new int3(vi.value), false);
                var vec4 = MathMulTransVec(rotation, new int3(vi.value), true);

                var label = root.Q<Label>("temp");

                if (label == null)
                {
                    root.Add(label = new Label { name = "temp" });
                }

                label.text = string.Join(Environment.NewLine, vec1, vec2, vec3, vec4);
            }
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            const string text = "Reset";

            menu.AddItem(L10n.TextContent(text), false, ResetWorkspace);

            static void ResetWorkspace()
            {
                Undo.RecordObject(Workspace, text);
                Workspace.Reset();
            }
        }

        private static float3 MathMulVec(float3x3 a1, int3 a2, bool transpose)
        {
            // 0 1 2
            // 3 4 5
            // 6 7 8

            // 0 3 6
            // 1 4 7
            // 2 5 8

            if (transpose)
            {
                a1 = math.transpose(a1);
            }

            var x = (a2.x * a1.c0.x + a2.y * a1.c0.y + a2.z * a1.c0.z) / 4096.0f;
            var y = (a2.x * a1.c1.x + a2.y * a1.c1.y + a2.z * a1.c1.z) / 4096.0f;
            var z = (a2.x * a1.c2.x + a2.y * a1.c2.y + a2.z * a1.c2.z) / 4096.0f;

            return new float3(x, y, z);
        }

        private static float3 MathMulTransVec(float3x3 a1, int3 a2, bool transpose)
        {
            // 0 1 2
            // 3 4 5
            // 6 7 8

            // 0 3 6
            // 1 4 7
            // 2 5 8

            if (transpose)
            {
                a1 = math.transpose(a1);
            }

            var x = (a2.x * a1.c0.x + a2.y * a1.c1.x + a2.z * a1.c2.x) / 4096.0f;
            var y = (a2.x * a1.c0.y + a2.y * a1.c1.y + a2.z * a1.c2.y) / 4096.0f;
            var z = (a2.x * a1.c0.z + a2.y * a1.c1.z + a2.z * a1.c2.z) / 4096.0f;

            return new float3(x, y, z);
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