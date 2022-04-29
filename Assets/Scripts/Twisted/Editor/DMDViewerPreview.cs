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
        // TODO splitting
        // TODO texturing
        // TODO transform
    {
        private static IReadOnlyDictionary<Type, Color> PolygonColors { get; set; } = null!;

        private static int[] PolygonWinding { get; } = { 2, 1, 0, 2, 3, 1 };

        private void OnEnable()
        {
            ConfigurePolygonColors();
        }

        private static void ConfigurePolygonColors()
        {
            var type = typeof(Polygon);

            var types = type.Assembly.GetTypes().Where(s => s.IsAbstract is false && type.IsAssignableFrom(s)).ToArray();

            var state = Random.state;

            Random.InitState(123456789);

            var dictionary = types.ToDictionary(s => s, _ => Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f));

            Random.state = state;

            PolygonColors = new ReadOnlyDictionary<Type, Color>(dictionary);
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

            SceneViewUtility.Frame(gameObject);
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
}