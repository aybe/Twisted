using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public sealed class DMDViewer : EditorWindow
        // BUG when tree view loses focus, one of its expander may turn blue at any time
        // BUG tree view keyboard expand/collapse may stop working for no reason at all
        // BUG horizontal scroll bar flickers and partially hides selected item at bottom
    {
        [SerializeField]
        private VisualTreeAsset VisualTreeAsset = null!;

        [SerializeField]
        private DMDViewerModel Model = null!;

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var container = VisualTreeAsset.Instantiate();

            container.style.flexGrow = 1;

            root.Add(container);

            var toolbarButtonOpenFile          = root.Q<ToolbarButton>("toolbarButtonOpenFile");
            var toolbarToggleDistinctFiltering = root.Q<ToolbarToggle>("toolbarToggleDistinctFiltering");
            var toolbarToggleRegexSearch       = root.Q<ToolbarToggle>("toolbarToggleRegexSearch");
            var toolbarToggleSelectionFraming  = root.Q<ToolbarToggle>("toolbarToggleSelectionFraming");
            var toolbarToggleModelSplitting    = root.Q<ToolbarToggle>("toolbarToggleModelSplitting");
            var toolbarToggleTexturing         = root.Q<ToolbarToggle>("toolbarToggleTexturing");
            var toolbarToggleVertexColors      = root.Q<ToolbarToggle>("toolbarToggleVertexColors");
            var toolbarTogglePolygonColoring   = root.Q<ToolbarToggle>("toolbarTogglePolygonColoring");
            var toolbarLabelSearchResults      = root.Q<Label>("toolbarLabelSearchResults");
            var toolbarSliderItemHeight        = root.Q<SliderInt>("toolbarSliderItemHeight");
            var toolbarSearchField             = root.Q<ToolbarSearchField>("toolbarSearchField");

            InitializeModel();
            InitializeWindowTitle();

            toolbarButtonOpenFile.clicked += () =>
            {
                Debug.Log(true);
                Model.OpenFile();
                InitializeWindowTitle();
            };

            toolbarToggleDistinctFiltering.BindProperty(Model.UseDistinctFilteringProperty);

            toolbarToggleDistinctFiltering.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleRegexSearch.BindProperty(Model.UseRegexSearchProperty);

            toolbarToggleRegexSearch.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleSelectionFraming.BindProperty(Model.UseSelectionFramingProperty);

            toolbarToggleSelectionFraming.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleModelSplitting.BindProperty(Model.UseModelSplittingProperty);

            toolbarToggleModelSplitting.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleTexturing.BindProperty(Model.UseTexturingProperty);

            toolbarToggleTexturing.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarToggleVertexColors.BindProperty(Model.UseVertexColorsProperty);

            toolbarToggleVertexColors.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            toolbarTogglePolygonColoring.BindProperty(Model.UsePolygonColorsProperty);

            toolbarTogglePolygonColoring.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
            });

            // TODO other properties

            root.Bind(Model.SerializedObject);

            var sliderItemHeight = root.Q<SliderInt>();
            var searchField      = root.Q<ToolbarSearchField>();
            var treeView         = root.Q<MultiColumnTreeView>();

            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode is KeyCode.F && evt.ctrlKey)
                {
                    searchField.Focus();
                }
            });

            sliderItemHeight.RegisterValueChangedCallback(evt =>
            {
                // how nice is that, we must rebuild the tree for it to pick up fixed item height change...

                if (treeView.virtualizationMethod is not CollectionVirtualizationMethod.FixedHeight)
                    treeView.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;

                treeView.fixedItemHeight = evt.newValue;

                treeView.Rebuild();
            });

            searchField.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue); // TODO update tree view
            });

            searchField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode is not KeyCode.DownArrow)
                    return;

                treeView.Q<ScrollView>().contentContainer.Focus(); // shamelessly stolen from source
            });

            treeView.fixedItemHeight = sliderItemHeight.value; // sync our view with slider

            treeView.columns.Add(new Column
            {
                name          = "Column 1 Name",
                title         = "Column 1 Title",
                icon          = default,
                visible       = true,
                width         = 200,
                minWidth      = 50,
                maxWidth      = 300,
                sortable      = true,
                stretchable   = true,
                optional      = true,
                resizable     = true,
                makeHeader    = () => new Label(),
                bindHeader    = element => ((Label)element).text = "Header 1",
                unbindHeader  = null,
                destroyHeader = null,
                makeCell = () =>
                {
                    var label = new Label();
                    label.AddToClassList("tree-view-cell");
                    return label;
                },
                bindCell    = (element, i) => ((Label)element).text = $"Column 1: {i}",
                unbindCell  = (element, i) => ((Label)element).text = default,
                destroyCell = null
            });

            // TODO bindHeader & title should be column name

            treeView.columns.Add(new Column
            {
                name          = "Column 2 Name",
                title         = "Column 2 Title",
                icon          = default,
                visible       = true,
                width         = 200,
                minWidth      = 50,
                maxWidth      = 300,
                sortable      = true,
                stretchable   = true,
                optional      = true,
                resizable     = true,
                makeHeader    = () => new Label(),
                bindHeader    = element => ((Label)element).text = "Header 2",
                unbindHeader  = null,
                destroyHeader = null,
                makeCell = () =>
                {
                    var label = new Label();
                    label.AddToClassList("tree-view-cell");
                    return label;
                },
                bindCell    = (element, i) => ((Label)element).text = $"Column 2: {i}",
                unbindCell  = (element, i) => ((Label)element).text = default,
                destroyCell = null
            });

            treeView.columns.Add(new Column
            {
                name          = "Column 3 Name",
                title         = "Column 3 Title",
                icon          = default,
                visible       = true,
                width         = 200,
                minWidth      = 50,
                maxWidth      = 300,
                sortable      = true,
                stretchable   = true,
                optional      = true,
                resizable     = true,
                makeHeader    = () => new Label(),
                bindHeader    = element => ((Label)element).text = "Header 3",
                unbindHeader  = null,
                destroyHeader = null,
                makeCell = () =>
                {
                    var label = new Label();
                    label.AddToClassList("tree-view-cell");
                    return label;
                },
                bindCell    = (element, i) => ((Label)element).text = $"Column 3: {i}",
                unbindCell  = (element, i) => ((Label)element).text = default,
                destroyCell = null
            });

            var index = 0; // NOTE: anything negative won't be shown
            var items = new List<TreeViewItemData<string>>
            {
                new(index++, "Root", TreeViewSampleData.Generate(5, 5, (id, depth) => $"{nameof(id)}: {id}, {nameof(depth)}: {depth}", ref index))
            };

            treeView.SetRootItems(items);
        }

        private void InitializeModel()
        {
            if (Model == null)
            {
                Model = CreateInstance<DMDViewerModel>();
            }
        }

        private void InitializeWindowTitle()
        {
            titleContent = new GUIContent(DMDViewerStyles.WindowTitle)
            {
                text = File.Exists(Model.CurrentFile) ? Path.GetFileName(Model.CurrentFile) : "DMD Viewer"
            };
        }


        [MenuItem("Twisted/DMD Viewer (UI Elements)")]
        public static void InitializeWindow()
        {
            GetWindow<DMDViewer>();
        }
    }
}