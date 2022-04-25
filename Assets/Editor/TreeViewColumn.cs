using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class TreeViewColumn<T> where T : TreeNode
    {
        public const string ControlUssClassName = "generic-tree-view-cell-control";

        public TreeViewColumn()
        {
            Optional    = true;
            Resizable   = true;
            Sortable    = true;
            Visible     = true;
            Stretchable = false;

            static T GetNode(VisualElement element, int index)
            {
                if (element is null)
                    throw new ArgumentNullException(nameof(element));

                var view = element.GetParent<GenericTreeView<T>>() ?? throw new InvalidOperationException();
                var item = view.viewController.GetItemForIndex(index);
                var node = item as T ?? throw new InvalidOperationException();

                return node;
            }

            static VisualElement GetLabel(TextAnchor anchor)
            {
                return new Label
                {
                    style =
                    {
                        flexGrow       = 1,
                        unityTextAlign = anchor,
                        overflow       = Overflow.Hidden,
                        textOverflow   = TextOverflow.Ellipsis
                    }
                };
            }

            VisualElement MakeHeader()
            {
                return GetLabel(TextAnchor.MiddleLeft);
            }

            void BindHeader(VisualElement element)
            {
                ((Label)element).text = Title;
            }

            void UnbindHeader(VisualElement element)
            {
                ((Label)element).text = default;
            }

            void DestroyHeader(VisualElement element)
            {
            }

            VisualElement MakeCell()
            {
                return GetLabel(TextAnchor.MiddleLeft);
            }

            void BindCell(VisualElement element, int index)
            {
                var node  = GetNode(element, index);
                var data  = ValueGetter?.Invoke(node);
                var text  = ValueFormatter?.Invoke(data) ?? data?.ToString();
                var label = element as Label ?? throw new InvalidOperationException();
                label.text = text;
                label.userData = node;
                label.AddToClassList(ControlUssClassName);
            }

            void UnbindCell(VisualElement element, int index)
            {
                var label = element as Label ?? throw new InvalidOperationException();
                label.text     = default;
                label.userData = default;
            }

            void DestroyCell(VisualElement element)
            {
            }

            HeaderMake    = MakeHeader;
            HeaderBind    = BindHeader;
            HeaderUnbind  = UnbindHeader;
            HeaderDestroy = DestroyHeader;
            CellMake      = MakeCell;
            CellBind      = BindCell;
            CellUnbind    = UnbindCell;
            CellDestroy   = DestroyCell;
        }

        private Column Column { get; } = new();

        [PublicAPI]
        public string Name
        {
            get => Column.name;
            set => Column.name = value;
        }

        [PublicAPI]
        public string Title
        {
            get => Column.title;
            set => Column.title = value;
        }

        [PublicAPI]
        public Background Icon
        {
            get => Column.icon;
            set => Column.icon = value;
        }

        [PublicAPI]
        public bool Visible
        {
            get => Column.visible;
            set => Column.visible = value;
        }

        [PublicAPI]
        public Length Width
        {
            get => Column.width;
            set => Column.width = value;
        }

        [PublicAPI]
        public Length MinWidth
        {
            get => Column.minWidth;
            set => Column.minWidth = value;
        }

        [PublicAPI]
        public Length MaxWidth
        {
            get => Column.maxWidth;
            set => Column.maxWidth = value;
        }

        [PublicAPI]
        public bool Sortable
        {
            get => Column.sortable;
            set => Column.sortable = value;
        }

        [PublicAPI]
        public bool Stretchable
        {
            get => Column.stretchable;
            set => Column.stretchable = value;
        }

        [PublicAPI]
        public bool Optional
        {
            get => Column.optional;
            set => Column.optional = value;
        }

        [PublicAPI]
        public bool Resizable
        {
            get => Column.resizable;
            set => Column.resizable = value;
        }

        [PublicAPI]
        public Func<VisualElement> HeaderMake
        {
            get => Column.makeHeader;
            set => Column.makeHeader = value;
        }

        [PublicAPI]
        public Action<VisualElement> HeaderBind
        {
            get => Column.bindHeader;
            set => Column.bindHeader = value;
        }

        [PublicAPI]
        public Action<VisualElement> HeaderUnbind
        {
            get => Column.unbindHeader;
            set => Column.unbindHeader = value;
        }

        [PublicAPI]
        public Action<VisualElement> HeaderDestroy
        {
            get => Column.destroyHeader;
            set => Column.destroyHeader = value;
        }

        [PublicAPI]
        public Func<VisualElement> CellMake
        {
            get => Column.makeCell;
            set => Column.makeCell = value;
        }

        [PublicAPI]
        public Action<VisualElement, int> CellBind
        {
            get => Column.bindCell;
            set => Column.bindCell = value;
        }

        [PublicAPI]
        public Action<VisualElement, int> CellUnbind
        {
            get => Column.unbindCell;
            set => Column.unbindCell = value;
        }

        [PublicAPI]
        public Action<VisualElement> CellDestroy
        {
            get => Column.destroyCell;
            set => Column.destroyCell = value;
        }

        [PublicAPI]
        public Func<T, object?>? ValueGetter { get; set; }

        [PublicAPI]
        public Func<object?, string?>? ValueFormatter { get; set; }

        [PublicAPI]
        public Func<object?, IComparer>? ValueComparer { get; set; }

        internal Column GetColumn()
        {
            return Column;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}";
        }
    }
}