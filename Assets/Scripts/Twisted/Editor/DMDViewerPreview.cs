using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Twisted.Formats.Database;
using Twisted.Formats.Graphics2D;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Twisted.Editor
{
    [ExecuteAlways]
    [Singleton(Automatic = true)]
    public sealed class DMDViewerPreview : MonoBehaviour, ISingleton
    {
        public void FrameSelection()
        {
            // this is a consistent framing experience unlike Unity's which may or may not further zoom in/out

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

        public void SetNode(DMDViewerFactory factory, DMDNode00FF? node, bool split = true, bool frame = true)
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

            if (node is null)
                return;

            var parents = node.GetParents<DMDNode050B>();

            if (parents.Count > 1)
            {
                throw new NotSupportedException();
            }

            // TODO preliminary positioning with start arena ground @ 512116

            var mat = parents.FirstOrDefault()?.GetTransform() ?? float4x4.identity;

            var pos = parents.FirstOrDefault()?.GetVectors().FirstOrDefault() ?? default;

            pos = math.transform(mat, pos.yzx) * (1.0f / 200.0f);

            var infos = node.Polygons.Where(s => s.TextureInfo.HasValue).Select(s => s.TextureInfo!.Value).ToArray();

            factory.GetTextureAtlas(infos, out var atlas, out var atlasTexture, out var atlasIndices);

            Assert.AreEqual(atlasTexture != null, atlasTexture is not null);

            if (atlasTexture is not null)
            {
                atlasTexture.filterMode = FilterMode.Point;
                atlasTexture.hideFlags  = HideFlags.HideAndDontSave;
            }

            var sharedMaterial = new Material(Shader.Find("Twisted/DMDViewer")) { mainTexture = atlasTexture };

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
                    var uvs      = new List<Vector2>();
                    var colors1  = new List<Vector4>();
                    var colors2  = new List<Vector4>();
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

                        var polygonColors = polygon.Colors;

                        var textureInfo = polygon.TextureInfo;
                        var textureUVs  = polygon.TextureUVs;

                        for (var i = 0; i < polygon.Vertices.Count / 2; i++)
                        {
                            for (var j = 0; j < 3; j++)
                            {
                                var k = i * 3 + j;
                                var l = winding[k];
                                var m = polygon.Vertices[l];
                                var n = new Vector3(m.X, m.Y, m.Z);
                                var o = matrix.MultiplyPoint(n);
                                o = math.transform(mat, n);

                                vertices.Add(o);

                                if (polygonColors != null)
                                {
                                    switch (polygonColors.Count)
                                    {
                                        case 0:
                                            throw new InvalidDataException();
                                        case 1:
                                            colors1.Add((Color)polygonColors[0]);
                                            break;
                                        default:
                                            colors1.Add((Color)polygonColors[l]);
                                            break;
                                    }
                                }
                                else
                                {
                                    colors1.Add(Color.magenta); // BUG there should be distinct meshes
                                }

                                colors2.Add(color);
                                indices.Add(indices.Count);

                                if (textureInfo is not null && textureUVs is not null)
                                {
                                    var t = textureUVs[l];
                                    var z = atlasIndices[textureInfo.Value];
                                    var u = atlas.GetUV(z, t, false, TextureTransform.FlipY);
                                    uvs.Add(u);
                                }
                                else
                                {
                                    uvs.Add(Vector2.zero); // BUG this is wrong, there should be distinct sub-meshes
                                }
                            }
                        }
                    }

                    var meshName = split ? $"Polygon {index++} @ {list.Single().Position} ({list.Single().GetType().Name})" : "Polygons";

                    var mesh = new Mesh
                    {
                        name      = meshName,
                        hideFlags = HideFlags.HideAndDontSave
                    };

                    mesh.SetVertices(vertices);
                    mesh.SetUVs(0, uvs);
                    mesh.SetUVs(1, colors1);
                    mesh.SetUVs(2, colors2);
                    mesh.SetTriangles(indices, 0);

                    var go = new GameObject(meshName) { transform = { parent = gameObject.transform, position = pos } };
                    var mf = go.AddComponent<MeshFilter>();
                    var mc = go.AddComponent<MeshCollider>();
                    var mr = go.AddComponent<MeshRenderer>();

                    Debug.Log($"position = {go.transform.position}, bounds = {mr.bounds}");

                    mf.sharedMesh     = mesh;
                    mc.sharedMesh     = mesh;
                    mr.sharedMaterial = sharedMaterial;
                }
            }

            DestroyImmediate(atlas);

            if (frame)
            {
                FrameSelection();
            }
        }

        private readonly struct RandomStateScope : IDisposable
        {
            private readonly Random.State State;

            public RandomStateScope(int seed)
            {
                State = Random.state;
                Random.InitState(seed);
            }

            public void Dispose()
            {
                Random.state = State;
            }
        }
    }
}