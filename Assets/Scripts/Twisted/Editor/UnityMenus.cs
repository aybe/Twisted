using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Twisted.Controls;
using Twisted.Formats.Database;
using UnityEditor;
using UnityEngine;

namespace Twisted.Editor
{
    internal static partial class UnityMenus
    {
        [MenuItem("Twisted/DMD Viewer", priority = 0)]
        private static void DMDViewer()
        {
            EditorWindow.GetWindow<Viewer>();
        }

        private static void BatchExtract(Action<TreeNode, List<byte[]>> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var open = EditorUtility.OpenFolderPanel(null, null, null);

            if (string.IsNullOrEmpty(open))
                return;

            var save = EditorUtility.SaveFilePanel(null, null, null, null);

            if (string.IsNullOrEmpty(save))
                return;

            var files = Directory.GetFiles(open, "*.DMD");

            var bytes = new List<byte[]>();

            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];

                using var fs = File.OpenRead(file);
                using var br = new BinaryReader(fs);

                var dmd = new DMD(br);

                foreach (var node in dmd.TraverseDfs())
                {
                    action(node, bytes);
                }

                if (!EditorUtility.DisplayCancelableProgressBar($"Extracting file {i + 1} of {files.Length}", file, 1.0f / files.Length * (i + 1)))
                    continue;

                EditorUtility.ClearProgressBar();
                return;
            }

            bytes.Sort(ArrayComparer<byte>.Instance);

            var max = bytes.Max(s => s.Length);

            using var ms = new MemoryStream();

            foreach (var polygon in bytes)
            {
                ms.Write(polygon);

                for (var i = 0; i < max - polygon.Length; i++)
                {
                    ms.WriteByte(0xCD);
                }
            }

            File.WriteAllBytes(save, ms.ToArray());

            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Twisted/Batch Extract Nodes", priority = 1)]
        private static void BatchExtractNodes()
        {
            if (EditorUtility.DisplayDialog("Batch Extract Nodes", "This will batch extract nodes from DMD files.", "Continue", "Cancel"))
            {
                BatchExtract(Action);
            }

            static void Action(TreeNode node, List<byte[]> list)
            {
                if (node is DMDNode dn)
                {
                    list.Add(dn.GetObjectData());
                }
            }
        }

        [MenuItem("Twisted/Batch Extract Polygons", priority = 2)]
        private static void BatchExtractPolygons()
        {
            if (EditorUtility.DisplayDialog("Batch Extract Polygons", "This will batch extract polygons from DMD files.", "Continue", "Cancel"))
            {
                BatchExtract(Action);
            }

            static void Action(TreeNode node, List<byte[]> list)
            {
                if (node is DMDNode00FF ff)
                {
                    list.AddRange(ff.Polygons.Select(s => s.GetObjectData()));
                }
            }
        }

        private sealed class ArrayComparer<T> : Comparer<T[]> where T : IComparable<T>
        {
            public static ArrayComparer<T> Instance { get; } = new();

            public override int Compare(T[]? x, T[]? y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return +1;
                }

                for (var i = 0; i < x.Length; i++)
                {
                    var a = x[i];
                    var b = y[i];
                    var c = a.CompareTo(b);

                    if (c != 0)
                    {
                        return c;
                    }
                }

                return 0;
            }
        }
    }

    internal static partial class UnityMenus
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