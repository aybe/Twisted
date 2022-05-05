using UnityEditor;
using UnityEngine;

namespace Twisted.Editor
{
    internal static class TransformExtensions
    {
        [MenuItem("Edit/Clear Undo History", priority = 10)]
        private static void ClearUndoHistory()
        {
            if (EditorUtility.DisplayDialog("Clear undo history?", "This action cannot be undone.", "Don't clear", "Clear"))
                return;

            Undo.ClearAll();
        }

        [MenuItem("CONTEXT/Transform/Select, Ping", priority = 999998)]
        private static void TransformSelectPing(MenuCommand command)
        {
            var gameObject = ((Transform)command.context).gameObject;

            Selection.activeGameObject = gameObject;

            EditorGUIUtility.PingObject(gameObject);
        }

        [MenuItem("CONTEXT/Transform/Select, Ping, Frame", priority = 999999)]
        private static void TransformSelectFramePing(MenuCommand command)
        {
            var gameObject = ((Transform)command.context).gameObject;

            Selection.activeGameObject = gameObject;

            EditorGUIUtility.PingObject(gameObject);

            var view = SceneView.currentDrawingSceneView;

            if (view == null)
            {
                view = SceneView.lastActiveSceneView;
            }

            if (view != null)
            {
                EditorApplication.delayCall += () => view.FrameSelected();
            }
        }
    }
}