using System;
using System.Collections.Generic;
using System.Linq;
using Twisted.Extensions;
using Twisted.PS;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Twisted
{
    [ExecuteAlways]
    [Singleton(Automatic = true)]
    public sealed class DMDPreview : MonoBehaviour, ISingleton
    {
        public void SetNode(DMDNode00FF? node, bool split = true)
        {
            while (transform.childCount > 0)
            {
                var tr = transform.GetChild(0);
                var mf = tr.GetComponent<MeshFilter>();
                var mr = tr.GetComponent<MeshRenderer>();
                var mc = tr.GetComponent<MeshCollider>();

                DestroyImmediate(mf.sharedMesh);
                DestroyImmediate(mc.sharedMesh);
                DestroyImmediate(mr.sharedMaterial);
                DestroyImmediate(tr.gameObject);
            }

            if (node == null)
                return;

            var lists = split ? node.Polygons.Select(s => new[] { s }).ToArray() : new[] { node.Polygons };

            var index = 0;

            using (new RandomStateScope(1234))
            {
                var dictionary = new Dictionary<Type, Color>();
                var winding    = new[] { 2, 1, 0, 2, 3, 1 };
                var matrix     = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90.0f, 0.0f, 0.0f), Vector3.one * 0.01f);

                foreach (var list in lists)
                {
                    var vertices = new List<Vector3>();
                    var colors   = new List<Color>();
                    var indices  = new List<int>();

                    foreach (var polygon in list)
                    {
                        Color color;

                        var type = polygon.GetType();

                        if (dictionary.ContainsKey(type))
                        {
                            color = dictionary[type];
                        }
                        else
                        {
                            color = Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
                            dictionary.Add(type, color);
                        }

                        for (var i = 0; i < polygon.Vertices.Count / 2; i++)
                        {
                            for (var j = 0; j < 3; j++)
                            {
                                var k = i * 3 + j;
                                var l = winding[k];
                                var m = polygon.Vertices[l];
                                var n = m.ToVector3();
                                var o = matrix.MultiplyPoint(n);
                                vertices.Add(o);
                                colors.Add(color);
                                indices.Add(indices.Count);
                            }
                        }
                    }

                    var meshName = split ? $"Polygon {index++} @ {list.Single().Position} ({list.Single().GetType().Name})" : "Polygons";

                    var mesh = new Mesh
                    {
                        name      = meshName,
                        hideFlags = HideFlags.HideAndDontSave,
                        vertices  = vertices.ToArray(),
                        colors    = colors.ToArray(),
                        triangles = indices.ToArray()
                    };

                    var go = gameObject.CreateChild(meshName);
                    var mf = go.AddComponent<MeshFilter>();
                    var mc = go.AddComponent<MeshCollider>();
                    var mr = go.AddComponent<MeshRenderer>();
                    var pb = go.AddComponent<DMDPreviewBatch>();

                    mf.sharedMesh     = mesh;
                    mc.sharedMesh     = mesh;
                    mr.sharedMaterial = new Material(Shader.Find("Twisted/DMDViewer"));
                    pb.Polygons       = list.Select(s => string.Concat(s.GetObjectData().Select(t => t.ToString("X2")))).ToList();
                }
            }

            // frame the object but unlike Unity, make it pleasant

            var view = SceneView.lastActiveSceneView;

            if (view == null)
                return;

            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            var renderer1 = renderers.FirstOrDefault();
            var bounds    = renderer1 != null ? renderer1.bounds : new Bounds();

            foreach (var current in renderers)
            {
                bounds.Encapsulate(current.bounds);
            }

            view.Frame(bounds, false);
        }
    }
}