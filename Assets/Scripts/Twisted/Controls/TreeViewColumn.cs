using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Twisted.Controls
{
    [PublicAPI]
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

                var view = element.GetParent<TreeView<T>>() ?? throw new InvalidOperationException();
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
                label.text     = text;
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

        public string Name
        {
            get => Column.name;
            set => Column.name = value;
        }

        public string Title
        {
            get => Column.title;
            set => Column.title = value;
        }

        public Background Icon
        {
            get => Column.icon;
            set => Column.icon = value;
        }

        public bool Visible
        {
            get => Column.visible;
            set => Column.visible = value;
        }

        public Length Width
        {
            get => Column.width;
            set => Column.width = value;
        }

        public Length MinWidth
        {
            get => Column.minWidth;
            set => Column.minWidth = value;
        }

        public Length MaxWidth
        {
            get => Column.maxWidth;
            set => Column.maxWidth = value;
        }

        public bool Sortable
        {
            get => Column.sortable;
            set => Column.sortable = value;
        }

        public bool Stretchable
        {
            get => Column.stretchable;
            set => Column.stretchable = value;
        }

        public bool Optional
        {
            get => Column.optional;
            set => Column.optional = value;
        }

        public bool Resizable
        {
            get => Column.resizable;
            set => Column.resizable = value;
        }

        public Func<VisualElement> HeaderMake
        {
            get => Column.makeHeader;
            set => Column.makeHeader = value;
        }

        public Action<VisualElement> HeaderBind
        {
            get => Column.bindHeader;
            set => Column.bindHeader = value;
        }

        public Action<VisualElement> HeaderUnbind
        {
            get => Column.unbindHeader;
            set => Column.unbindHeader = value;
        }

        public Action<VisualElement> HeaderDestroy
        {
            get => Column.destroyHeader;
            set => Column.destroyHeader = value;
        }

        public Func<VisualElement> CellMake
        {
            get => Column.makeCell;
            set => Column.makeCell = value;
        }

        public Action<VisualElement, int> CellBind
        {
            get => Column.bindCell;
            set => Column.bindCell = value;
        }

        public Action<VisualElement, int> CellUnbind
        {
            get => Column.unbindCell;
            set => Column.unbindCell = value;
        }

        public Action<VisualElement> CellDestroy
        {
            get => Column.destroyCell;
            set => Column.destroyCell = value;
        }

        public Func<T, object?>? ValueGetter { get; set; }

        public Func<object?, string?>? ValueFormatter { get; set; }

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