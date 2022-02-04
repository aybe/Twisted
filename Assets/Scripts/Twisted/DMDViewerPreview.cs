using System;
using System.Collections.Generic;
using Twisted.Extensions;
using Twisted.PS;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Twisted
{
    [ExecuteAlways]
    [Singleton(Automatic = true)]
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
    public sealed class DMDViewerPreview : MonoBehaviour, ISingleton
    {
        public void SetNode(DMDNode00FF node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

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

            var meshFilter = GetComponent<MeshFilter>();

            if (meshFilter.sharedMesh != null)
            {
                DestroyImmediate(meshFilter.sharedMesh); // BUG this is editor-only
            }

            meshFilter.sharedMesh = mesh;

            var meshCollider = GetComponent<MeshCollider>();

            meshCollider.sharedMesh = mesh;

            var meshRenderer = GetComponent<MeshRenderer>();

            if (meshRenderer.sharedMaterial == null)
            {
                meshRenderer.sharedMaterial = new Material(Shader.Find("Twisted/DMDViewer"));
            }
        }
    }
}