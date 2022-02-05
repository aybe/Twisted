using System;
using System.Collections.Generic;
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
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
    public sealed class DMDViewerPreview : MonoBehaviour, ISingleton
    {
        public void SetNode(DMDNode00FF? node)
        {
            var meshFilter   = GetComponent<MeshFilter>();
            var meshCollider = GetComponent<MeshCollider>();
            var meshRenderer = GetComponent<MeshRenderer>();

            // welcome to Unity... when a property to a reference doesn't behave as it normally should,
            // destroying a reference simply isn't enough, you have to nullify that property as well...
            // or you'll scratch your head on why it works only once and that cryptic 'Missing' message

            DestroyImmediate(meshFilter.sharedMesh);
            DestroyImmediate(meshCollider.sharedMesh);
            DestroyImmediate(meshRenderer.sharedMaterial);

            meshFilter.sharedMesh       = null;
            meshCollider.sharedMesh     = null;
            meshRenderer.sharedMaterial = null;

            if (node == null)
                return;

            var polygons = node.Polygons;

            Debug.Log($"Building {node}...");

            Debug.Log($"Model has {polygons.Count} polygons.");

            var vertices   = new List<Vector3>();
            var colors     = new List<Color>();
            var indices    = new List<int>();
            var dictionary = new Dictionary<Type, Color>();

            using var scope = new RandomStateScope(1234);

            var winding = new[] { 2, 1, 0, 2, 3, 1 };
            var matrix  = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90.0f, 0.0f, 0.0f), Vector3.one * 0.01f);

            foreach (var polygon in polygons)
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

            var mesh = new Mesh
            {
                hideFlags = HideFlags.HideAndDontSave, name = node.ToString(),
                vertices  = vertices.ToArray(),
                colors    = colors.ToArray(),
                triangles = indices.ToArray()
            };

            meshFilter.sharedMesh = mesh;

            meshCollider.sharedMesh = mesh;

            meshRenderer.sharedMaterial = new Material(Shader.Find("Twisted/DMDViewer"));




            {
            }
        }
    }
}