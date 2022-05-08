using System;
using System.IO;
using Twisted.Formats.Graphics2D;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Twisted.Editor.Tools
{
    internal sealed class TextureWindowCalculatorWindow : EditorWindow
    {
        #region Methods

        private static Texture2D BuildTexture(Texture2D source, TextureWindow window)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var src = source.GetPixels32();
            var dst = new Color32[src.Length];

            var pw = source.width;
            var ph = source.height;

            for (var y = 0; y < ph; y++)
            {
                for (var x = 0; x < pw; x++)
                {
                    var u = TextureWindow.Transform(x, window.MaskX, window.OffsetX);
                    var v = TextureWindow.Transform(y, window.MaskY, window.OffsetY);
                    dst[(ph - y - 1) * pw + x] = src[(ph - v - 1) * pw + u];
                }
            }

            var texture = new Texture2D(source.width, source.height)
            {
                filterMode = FilterMode.Point
            };

            texture.SetPixels32(dst);

            texture.Apply();

            return texture;
        }

        private void UpdateTexture(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));

            var texture = new Texture2D(0, 0);

            if (texture.LoadImage(File.ReadAllBytes(path)))
            {
                texture.filterMode = FilterMode.Point;

                DestroyImmediate(Texture);

                Texture = texture;
            }
            else
            {
                DestroyImmediate(texture);
                ShowNotification(L10n.TextContent($"Unable to load '{path}'."));
            }

            UpdateTextureControls();
            UpdateTexturePreviews();
        }

        private void UpdateTextureControls()
        {
            var elements = new[] { SliderWindowMaskX, SliderWindowMaskY, SliderWindowOffsetX, SliderWindowOffsetY };

            foreach (var element in elements)
            {
                element.SetEnabled(Texture != null);
            }
        }

        private void UpdateTexturePreviews()
        {
            ElementImageSource.style.backgroundImage = new StyleBackground(Texture);

            if (Texture == null)
                return;

            var window = new TextureWindow(
                SliderWindowMaskX.value,
                SliderWindowMaskY.value,
                SliderWindowOffsetX.value,
                SliderWindowOffsetY.value
            );

            var texture = BuildTexture(Texture, window);

            DestroyImmediate(ElementImageTarget.style.backgroundImage.value.texture);

            ElementImageTarget.style.backgroundImage = new StyleBackground(texture);
        }

        #endregion

        #region Event handlers

        private void OnButtonBrowseClicked()
        {
            var path = EditorUtility.OpenFilePanel(null, null, "png");

            if (string.IsNullOrEmpty(path))
                return;

            UpdateTexture(path);

            TexturePath = path;
        }

        private void OnSliderTextureWindowValueChanged(ChangeEvent<int> evt)
        {
            UpdateTexturePreviews();
        }

        #endregion

        #region Initialization

        private void CreateGUI()
        {
            InitializeControls();

            if (!string.IsNullOrEmpty(TexturePath))
            {
                UpdateTexture(TexturePath!);
            }
        }

        private void InitializeControls()
        {
            var root = rootVisualElement;

            root.Add(VisualTreeAsset.Instantiate());

            ButtonBrowse         =  root.Q<ToolbarButton>("buttonBrowse");
            ButtonBrowse.clicked += OnButtonBrowseClicked;

            SliderWindowMaskX = root.Q<SliderInt>("sliderWindowMaskX");
            SliderWindowMaskX.RegisterValueChangedCallback(OnSliderTextureWindowValueChanged);

            SliderWindowMaskY = root.Q<SliderInt>("sliderWindowMaskY");
            SliderWindowMaskY.RegisterValueChangedCallback(OnSliderTextureWindowValueChanged);

            SliderWindowOffsetX = root.Q<SliderInt>("sliderWindowOffsetX");
            SliderWindowOffsetX.RegisterValueChangedCallback(OnSliderTextureWindowValueChanged);

            SliderWindowOffsetY = root.Q<SliderInt>("sliderWindowOffsetY");
            SliderWindowOffsetY.RegisterValueChangedCallback(OnSliderTextureWindowValueChanged);

            ElementImageSource = root.Q<VisualElement>("elementImageSource");
            ElementImageTarget = root.Q<VisualElement>("elementImageTarget");
        }

        [MenuItem("Twisted/Texture Window Calculator", priority = int.MaxValue)]
        private static void InitializeWindow()
        {
            var window = GetWindow<TextureWindowCalculatorWindow>();
            window.titleContent = new GUIContent("Texture Window Calculator");
        }

        #endregion

        #region Fields

        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        [SerializeField]
        private Texture2D? Texture;

        [SerializeField]
        private string? TexturePath;

        #endregion

        #region Controls

        private ToolbarButton ButtonBrowse        = null!;
        private VisualElement ElementImageSource  = null!;
        private VisualElement ElementImageTarget  = null!;
        private SliderInt     SliderWindowMaskX   = null!;
        private SliderInt     SliderWindowMaskY   = null!;
        private SliderInt     SliderWindowOffsetX = null!;
        private SliderInt     SliderWindowOffsetY = null!;

        #endregion
    }
}