using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Twisted.Formats.Database;
using Twisted.Formats.Graphics2D;
using Twisted.Formats.Graphics3D;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace Twisted.Editor
{
    [ExecuteAlways]
    [Singleton(Automatic = true)]
    public sealed class DMDViewerPreview : MonoBehaviour, ISingleton
    {
        private static IReadOnlyDictionary<Type, Color> PolygonColors { get; set; } = null!;

        private static int[] PolygonWinding { get; } = { 2, 1, 0, 2, 3, 1 };

        private void OnEnable()
        {
            InitializePolygonColors();

            static void InitializePolygonColors()
            {
                var type = typeof(Polygon);

                var types = type.Assembly.GetTypes().Where(s => s.IsAbstract is false && type.IsAssignableFrom(s)).ToArray();

                var state = Random.state;

                Random.InitState(123456789);

                var dictionary = types.ToDictionary(s => s, _ => Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f));

                Random.state = state;

                PolygonColors = new ReadOnlyDictionary<Type, Color>(dictionary);
            }
        }

        [Obsolete]
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

        [Obsolete]
        public void SetNode(DMDViewerFactory factory, DMDNode00FF? node, bool split = true, bool frame = true)
        {
            while (transform.childCount > 0)
            {
                var tr = transform.GetChild(0);
                var mf = tr.GetComponent<MeshFilter>();
                var mr = tr.GetComponent<MeshRenderer>();
                var mc = tr.GetComponent<MeshCollider>();

                if (mf != null)
                {
                    // DestroyImmediate(mf.sharedMesh);
                }

                if (mc != null)
                {
                    // DestroyImmediate(mc.sharedMesh);
                }

                if (mr != null)
                {
                    // DestroyImmediate(mr.sharedMaterial);
                }

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

            var mat = parents.FirstOrDefault()?.GetTransformMatrix() ?? float4x4.identity;

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

        public void ConfigureNodes(DMDViewerFactory factory, DMDNode[] nodes, bool split, bool frame)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            if (nodes is null)
                throw new ArgumentNullException(nameof(nodes));

            // cleanup garbage from previous hierarchy if any

            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            // build hierarchy of selected nodes

            var stack = new Stack<KeyValuePair<DMDNode, GameObject>>();

            foreach (var node in nodes)
            {
                stack.Push(new KeyValuePair<DMDNode, GameObject>(node, gameObject));
            }

            while (stack.Count > 0)
            {
                var (node, parent) = stack.Pop();

                var child = parent.CreateChild($"0x{node.NodeType:X8} @ {node.Position}");

                ConfigureNode(child, node, factory);

                foreach (var item in node.Cast<DMDNode>().Reverse())
                {
                    stack.Push(new KeyValuePair<DMDNode, GameObject>(item, child));
                }
            }

            if (frame is false)
                return;

            Selection.activeGameObject = gameObject;

            FrameSelection();
        }

        private static void ConfigureNode(GameObject parent, DMDNode node, DMDViewerFactory factory)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            switch (node)
            {
                case DMD:
                    break;
                case DMDNode0010:
                    throw new NotImplementedException();
                case DMDNode00FF node00FF:
                    ConfigureModel(factory, node00FF, out var mesh, out var texture);

                    var mc = parent.AddComponent<MeshCollider>();
                    mc.cookingOptions = MeshColliderCookingOptions.None; // BUG [Physics.PhysX] cleaning the mesh failed
                    mc.sharedMesh     = mesh;

                    var mf = parent.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;

                    var mr = parent.AddComponent<MeshRenderer>();
                    mr.material = new Material(Shader.Find("Twisted/DMDViewer")) { mainTexture = texture };

                    break;
                case DMDNode0107 node0107:
                    parent.transform.position = math.transform(node.TransformHierarchy, node0107.Vector1) * 0.01f; // TODO
                    break;
                case DMDNode020X: // scenery
                    break;
                case DMDNode0305: // scenery
                    break;
                case DMDNode040B: // ground
                    break;
                case DMDNode050B: // ground, scenery
                    break;
                case DMDNode07FF:
                    throw new NotImplementedException();
                case DMDNode08FF: // scenery
                    break;
                case DMDNode0903: // scenery
                    break;
                case DMDNode0B06:
                    throw new NotImplementedException();
                case DMDNodeXXXX: // scenery
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(node));
            }
        }

        private static void ConfigureModel(DMDViewerFactory factory, DMDNode00FF node, out Mesh mesh, out Texture? texture)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var textureEnabled = false;

            texture = default;

            var polygons = node.Polygons;

            var meshes = new List<Mesh>();
            var groups = polygons.GroupBy(s => s.TextureInfo.HasValue).ToArray();
            var matrix = node.TransformHierarchy;

            foreach (var group in groups)
            {
                var atlas        = default(TextureAtlas?);
                var atlasTexture = default(Texture2D?);
                var atlasIndices = default(IReadOnlyDictionary<TextureInfo, int>?);

                if (textureEnabled)
                {
                    if (group.Key)
                    {
                        var infos = group.Select(s => s.TextureInfo!.Value).ToArray();
                        Debug.Log($"Texture infos: {infos.Length}");
                        Profiler.BeginSample($"{nameof(DMDViewerPreview)} get atlas");
                        factory.GetTextureAtlas(infos, out atlas, out atlasTexture, out atlasIndices);
                        Profiler.EndSample();
                    }
                }

                var vertices = new List<Vector3>();
                var uvs      = new List<Vector2>();
                var colors1  = new List<Vector4>();
                var colors2  = new List<Vector4>();
                var indices  = new List<int>();

                foreach (var polygon in group)
                {
                    var polygonColor = PolygonColors![polygon.GetType()];

                    for (var i = 0; i < polygon.Vertices.Count / 2; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            var k = i * 3 + j;
                            var l = PolygonWinding[k];

                            var vertex = polygon.Vertices[l];

                            Profiler.BeginSample($"{nameof(DMDViewerPreview)} get vertex");
                            vertices.Add(math.transform(matrix, new Vector3(vertex.X, vertex.Y, vertex.Z)));
                            Profiler.EndSample();

                            Profiler.BeginSample($"{nameof(DMDViewerPreview)} get color");
                            var color1 = polygon.Colors is null
                                ? default(Color?)
                                : polygon.Colors.Count switch
                                {
                                    0 => throw new InvalidDataException("The polygon has no colors."),
                                    1 => polygon.Colors[0],
                                    _ => polygon.Colors[l]
                                };
                            Profiler.EndSample();

                            if (color1.HasValue)
                            {
                                colors1.Add(color1.Value);
                            }

                            colors2.Add(polygonColor);

                            if (textureEnabled)
                            {
                                if (polygon.TextureInfo is not null && polygon.TextureUVs is not null)
                                {
                                    if (atlas is null || atlasIndices is null)
                                        throw new InvalidOperationException();

                                    var id = atlasIndices[polygon.TextureInfo.Value];
                                    var uv = polygon.TextureUVs[l];

                                    Profiler.BeginSample($"{nameof(DMDViewerPreview)} get UV");
                                    uvs.Add(atlas.GetUV(id, uv, false, TextureTransform.FlipY));
                                    Profiler.EndSample();
                                }
                            }

                            indices.Add(indices.Count);
                        }
                    }
                }

                var subMesh = new Mesh();

                subMesh.SetVertices(vertices);

                if (colors1.Any())
                {
                    subMesh.SetUVs(1, colors1);
                }

                if (colors2.Any())
                {
                    subMesh.SetUVs(2, colors2);
                }

                if (textureEnabled)
                {
                    if (group.Key)
                    {
                        subMesh.SetUVs(0, uvs);
                    }
                    else
                    {
                        Assert.IsTrue(uvs.Count is 0);
                    }

                    if (atlas is not null)
                    {
                        DestroyImmediate(atlas);
                    }

                    if (atlasTexture is not null)
                    {
                        texture = atlasTexture;
                    }
                }


                subMesh.SetIndices(indices, MeshTopology.Triangles, 0);

                meshes.Add(subMesh);
            }

            Profiler.BeginSample($"{nameof(DMDViewerPreview)} combine meshes");

            var combine = meshes.Select(s => new CombineInstance { mesh = s }).ToArray();

            mesh = new Mesh();

            mesh.CombineMeshes(combine, false, false, false);

            Profiler.EndSample();
        }

        [Obsolete]
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

    public static class GameObjectExtensions // TODO move
    {
        public static GameObject CreateChild(this GameObject parent, string? name = null, PrimitiveType? primitiveType = null)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var child = primitiveType != null ? GameObject.CreatePrimitive(primitiveType.Value) : new GameObject();

            if (name is not null)
            {
                child.name = name;
            }

            child.transform.SetParent(parent.transform);

            return child;
        }
    }
}