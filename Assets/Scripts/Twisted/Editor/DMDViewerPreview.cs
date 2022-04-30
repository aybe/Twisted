#define DMD_DEBUG_PREVIEW_SKIP_COMMON_NODES // TODO this should be toggleable from editor

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Twisted.Controls;
using Twisted.Formats.Database;
using Twisted.Formats.Graphics2D;
using Twisted.Formats.Graphics3D;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
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

            if (nodes.Any(s => s.Root != nodes.First().Root))
            {
                throw new ArgumentOutOfRangeException(nameof(nodes), "The nodes must all have the same root node.");
            }

            var infos1 = nodes.SelectMany(GetTextureInfos).ToArray();
            var infos2 = infos1.Distinct().ToArray();

            var stopwatch = Stopwatch.StartNew();
            Profiler.BeginSample("TEX ATLAS");
            factory.GetTextureAtlas(infos2, out var atlas, out var atlasTexture, out var atlasIndices);
            Profiler.EndSample();
            Debug.Log($"Atlas generated in {stopwatch.Elapsed} ({infos1.Length} -> {infos2.Length})");

            var texturing = new DMDTexturing(atlas, atlasIndices, atlasTexture);

            if (atlasTexture != null)
            {
                atlasTexture.filterMode = FilterMode.Point;
            }

            if (atlasTexture != null)
            {
                File.WriteAllBytes(".temp/atlas.png", atlasTexture.EncodeToPNG());
            }

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


#if DMD_DEBUG_PREVIEW_SKIP_COMMON_NODES

                // ReSharper disable CommentTypo

                if (node.Parent == node.Root && node.NodeType is not (0x0107_0100 or 0x0107_0300))
                    continue; // not scenery, not ground

                switch (node.NodeType)
                {
                    case 0x0107_0A00: // sweet tooth
                    case 0x0107_1400: // yellow jacket
                    case 0x0107_1E00: // darkside
                    case 0x0107_2800: // hammerhead
                    case 0x0107_3200: // outlaw
                    case 0x0107_3C00: // crimson fury
                    case 0x0107_4600: // warthog
                    case 0x0107_5000: // mr grimm
                    case 0x0107_5A00: // pit viper
                    case 0x0107_6400: // thump
                    case 0x0107_6E00: // spectre
                    case 0x0107_7800: // road kill
                        continue;
                }

                switch (node.NodeType)
                {
                    case 0x0107_EC04: // HUD elements
                    case 0x6472_0600: // grandstand (only?) low poly (high poly is 0x409C_0000)
                        continue;
                }

                if ((node.NodeType & 0x040B_9000) is 0x040B_9000)
                    continue; // power up

                // ReSharper restore CommentTypo
#endif

                var child = parent.CreateChild($"0x{node.NodeType:X8} @ {node.Position}");

                ConfigureNode(child, node, factory, texturing);

                foreach (var item in node.Cast<DMDNode>().Reverse())
                {
                    stack.Push(new KeyValuePair<DMDNode, GameObject>(item, child));
                }
            }

            if (atlas != null)
            {
                DestroyImmediate(atlas);
            }

            if (frame)
            {
                SceneViewUtility.Frame(gameObject);
            }
        }

        private static void ConfigureNode(GameObject parent, DMDNode node, DMDViewerFactory factory, DMDTexturing texturing)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (texturing == null)
                throw new ArgumentNullException(nameof(texturing));

            var info = node.GetNodeInfo();

            if (info is not null)
            {
                Debug.Log(
                    $"Kind = 0x{node.NodeKind:X4}, Role = 0x{node.NodeRole:X4}, Position = {node.Position}, Info = {info}"
                );
            }

            switch (node)
            {
                case DMD:
                    break;
                case DMDNode0010:
                    break;
                case DMDNode00FF node00FF:
                    var mesh = ConfigureModel(node00FF, texturing);

                    mesh.name = parent.name;

                    var mc = parent.AddComponent<MeshCollider>();
                    mc.cookingOptions = MeshColliderCookingOptions.None; // BUG [Physics.PhysX] cleaning the mesh failed
                    mc.sharedMesh     = mesh;

                    var mf = parent.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;

                    var mr = parent.AddComponent<MeshRenderer>();
                    mr.material = new Material(Shader.Find("Twisted/DMDViewer")) { mainTexture = texturing.AtlasTexture };

                    break;
                case DMDNode0107:
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
                    break;
                case DMDNode08FF: // scenery
                    break;
                case DMDNode0903: // scenery
                    break;
                case DMDNode0B06:
                    break;
                case DMDNodeXXXX: // scenery
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(node));
            }
        }

        private static List<TextureInfo> GetTextureInfos(DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var infos = new List<TextureInfo>();

            foreach (var ff in node.TraverseDfs().OfType<DMDNode00FF>())
            {
                foreach (var polygon in ff.Polygons)
                {
                    if (polygon.TextureInfo.HasValue)
                    {
                        infos.Add(polygon.TextureInfo.Value);
                    }
                }
            }

            return infos;
        }

        private static Mesh ConfigureModel(DMDNode00FF node, DMDTexturing texturing)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (texturing is null)
                throw new ArgumentNullException(nameof(texturing));

            var polygons = node.Polygons;

            var meshes = new List<Mesh>();
            var groups = polygons.GroupBy(s => s.TextureInfo.HasValue).ToArray();
            var matrix = node.WorldTransform;

            foreach (var group in groups)
            {
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

                            if (polygon.TextureInfo is not null && polygon.TextureUVs is not null)
                            {
                                var id = texturing.AtlasIndices[polygon.TextureInfo.Value];
                                var uv = polygon.TextureUVs[l];

                                Profiler.BeginSample($"{nameof(DMDViewerPreview)} get UV");
                                uvs.Add(texturing.Atlas.GetUV(id, uv, false, TextureTransform.FlipY));
                                Profiler.EndSample();
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

                if (group.Key)
                {
                    subMesh.SetUVs(0, uvs);
                }
                else
                {
                    Assert.IsTrue(uvs.Count is 0);
                }

                subMesh.SetIndices(indices, MeshTopology.Triangles, 0);

                meshes.Add(subMesh);
            }

            Profiler.BeginSample($"{nameof(DMDViewerPreview)} combine meshes");

            var combine = meshes.Select(s => new CombineInstance { mesh = s }).ToArray();

            var mesh = new Mesh();

            mesh.CombineMeshes(combine, false, false, false);

            Profiler.EndSample();

            return mesh;
        }

        private sealed class DMDTexturing
        {
            public DMDTexturing(
                TextureAtlas                          atlas,
                IReadOnlyDictionary<TextureInfo, int> atlasIndices,
                Texture2D                             atlasTexture)
            {
                Atlas        = atlas;
                AtlasIndices = atlasIndices;
                AtlasTexture = atlasTexture;
            }

            public TextureAtlas Atlas { get; }

            public IReadOnlyDictionary<TextureInfo, int> AtlasIndices { get; }

            public Texture2D AtlasTexture { get; }
        }
    }
}