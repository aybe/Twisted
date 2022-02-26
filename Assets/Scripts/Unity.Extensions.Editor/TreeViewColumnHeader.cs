using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Unity.Extensions.Editor
{
    public class TreeViewColumnHeader : MultiColumnHeader
        // this is needed because these idiots are using protected members but also managed to write a crash-prone context menu
    {
        public TreeViewColumnHeader(MultiColumnHeaderState state) : base(state)
        {
            allowDraggingColumnsToReorder = true;
        }

        protected override void AddColumnHeaderContextMenuItems(GenericMenu menu)
        {
            var resizeToFit = EditorGUIUtility.TrTextContent("Resize to fit");

            if (state.columns.Any(s => s.autoResize)) // while we're at it, fix that as well
            {
                menu.AddItem(resizeToFit, false, ResizeToFit);
            }
            else
            {
                menu.AddDisabledItem(resizeToFit);
            }

            menu.AddSeparator(string.Empty);

            for (var i = 0; i < state.columns.Length; ++i)
            {
                var column = state.columns[i];

                var content = EditorGUIUtility.TrTextContent(string.IsNullOrEmpty(column.contextMenuText) ? column.headerContent.text : column.contextMenuText);

                if (column.allowToggleVisibility)
                {
                    var contains = state.visibleColumns.Contains(i);

                    if (contains && state.visibleColumns.Length is 1)
                    {
                        menu.AddDisabledItem(content, contains); // this should have been an obvious thing to do, right?
                    }
                    else
                    {
                        menu.AddItem(content, contains, ToggleVisibilityPrivate, i);
                    }
                }
                else
                {
                    menu.AddDisabledItem(content);
                }
            }
        }

        private void ToggleVisibilityPrivate(object userData)
        {
            ToggleVisibility((int)userData);
        }
    }
}