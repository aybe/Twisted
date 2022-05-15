using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Twisted.Controls;
using Twisted.Formats.Binary;
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
    internal sealed class ViewerPreview : MonoBehaviour, ISingleton
        // TODO splitting
        // TODO texturing
        // TODO transform
        // TODO there's no need for an MB at all!?
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

        public void ConfigureNodes(ViewerFactory factory, DMDNode[] nodes, bool frame, bool split, Progress? progress = null)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            if (nodes is null)
                throw new ArgumentNullException(nameof(nodes));

            if (nodes.Any(s => s.Root != nodes.First().Root)) // TODO delete
                throw new ArgumentOutOfRangeException(nameof(nodes), "The nodes must all have the same root node.");

            var currentInfos = nodes.SelectMany(GetTextureInfos).ToArray().Distinct().ToArray();
            var currentNodes = nodes.Sum(s => s.TraverseDfs().Count());

            var progress1 = progress == null ? null : new Progress("Texturing", currentInfos.Length, progress);
            var progress2 = progress == null ? null : new Progress("Debugging", currentInfos.Length, progress);
            var progress3 = progress == null ? null : new Progress("Hierarchy", currentNodes,        progress);

            // generate textures

            var texturing = factory.GetTextureAtlas(currentInfos, progress1);
            
            if (texturing.AtlasTexture != null)
                texturing.AtlasTexture.filterMode = FilterMode.Point;

            // generate textures for debugging

            factory.ExportData(texturing, Path.Combine(Application.dataPath, "../.temp", DateTime.Now.ToString("u").Replace(":", "-")), progress2);

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

            var progressNodesCount = 0;

            while (stack.Count > 0)
            {
                progress3?.Report(1.0f / currentNodes * ++progressNodesCount);

                var (node, parent) = stack.Pop();

                if (ExcludeFromHierarchy(node))
                {
                    // continue; // TODO not working with progress reporting
                }

                var child = new GameObject($"0x{node.NodeType:X8} ({node.GetType().Name}) @ {node.Position}")
                {
                    transform = { parent = parent.transform }
                };

                ConfigureNode(child, node, texturing, split);

                foreach (var item in node.Cast<DMDNode>().Reverse())
                {
                    stack.Push(new KeyValuePair<DMDNode, GameObject>(item, child));
                }
            }

            texturing.Dispose();

            if (frame) // TODO move
            {
                ViewerUtility.Frame(gameObject);
            }
        }

        private static void ConfigureNode(GameObject parent, DMDNode node, ViewerTexturing texturing, bool split)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (texturing == null)
                throw new ArgumentNullException(nameof(texturing));

            switch (node)
            {
                case DMD:
                    break;
                case DMDNode0010:
                    break;
                case DMDNode00FF node00FF:

                    var lists = split ? node00FF.Polygons.Select(s => new[] { s }).ToArray() : new[] { node00FF.Polygons };

                    var index = 0;

                    foreach (var polygons in lists)
                    {
                        var value = polygons.Single();
                        var label = split
                            ? $"Polygon {++index:D3}, Type = 0x{value.GetType().Name.Replace("Polygon", string.Empty)}, Position = {value.Position}"
                            : parent.name;

                        var child = split ? new GameObject(label) { transform = { parent = parent.transform } } : parent;

                        var mesh = ConfigureModel(node00FF, texturing, polygons);

                        mesh.name = label;

                        var mc = child.AddComponent<MeshCollider>();
                        mc.cookingOptions = MeshColliderCookingOptions.None; // BUG [Physics.PhysX] cleaning the mesh failed
                        mc.sharedMesh     = mesh;

                        var mf = child.AddComponent<MeshFilter>();
                        mf.sharedMesh = mesh;

                        var mr = child.AddComponent<MeshRenderer>();
                        mr.material = new Material(Shader.Find("Twisted/DMDViewer")) { mainTexture = texturing.AtlasTexture };
                    }

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

            ConfigureNodeTransform(node, parent);
        }

        private static void ConfigureNodeTransform(DMDNode node, GameObject host)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (host == null)
                throw new ArgumentNullException(nameof(host));

            switch (node)
            {
                case DMD:
                case DMDNode0010:
                    break;
                case DMDNode00FF:
                case DMDNode0107:
                case DMDNode020X:
                case DMDNodeXXXX:
                    if (node.HasParent<DMDNode050B>()) // cancel 050B transform on its children, goes in pair with the code below
                    {
                        host.transform.localPosition = Vector3.zero;
                        host.transform.localRotation = Quaternion.identity;
                        host.transform.localScale    = Vector3.one;
                    }
                    break;
                case DMDNode0305:
                case DMDNode040B:
                    break;
                case DMDNode050B node050B:

                    if (node050B.Flag1 is 0)
                    {
                        // this is a mix of reverse-engineering, really boring calculations, lots of trial and error, but it works...

                        var matrix = node050B.Rotation;

                        matrix = new float3x3(
                            -matrix.c0.x, -matrix.c1.x, -matrix.c2.x,
                            -matrix.c0.y, -matrix.c1.y, -matrix.c2.y,
                            -matrix.c0.z, -matrix.c1.z, -matrix.c2.z
                        );

                        var vector = node050B.Vector1;

                        var position = math.transform(DMDNode050B.TRS(matrix), vector);

                        position = position.yzx;

                        position.z = -position.z;

                        host.transform.position = position;

                        var scale = Vector3.one;

                        if (position.x is not 0)
                            scale.x = -scale.x;

                        if (position.y is not 0)
                            scale.y = -scale.y;

                        if (position.z is not 0)
                            scale.z = -scale.z;

                        host.transform.localScale = scale;
                    }
                    else
                    {
                        // these matrices are really weird as they don't seem to follow a particular convention
                        // there doesn't seem to be any other flag that could give any indication about that...
                        // while this is hard-coded, it just works and things are correctly positioned in scene

                        const int zero = 0;
                        const int fore = +4096;
                        const int back = -4096;

                        var temp = new int3x3(node050B.Rotation);

                        var mat1 = new int3x3(
                            zero, back, zero,
                            fore, zero, zero,
                            zero, zero, fore
                        );

                        var mat2 = new int3x3(
                            back, zero, zero,
                            zero, back, zero,
                            zero, zero, fore
                        );

                        var mat3 = new int3x3(
                            zero, fore, zero,
                            back, zero, zero,
                            zero, zero, fore
                        );

                        switch (temp)
                        {
                            case var _ when temp.Equals(mat1):
                                host.transform.localPosition = node050B.Vector1.yzx * new float3(+1.0f, +1.0f, -1.0f);
                                host.transform.localRotation = Quaternion.identity;
                                host.transform.localScale    = new float3(-1.0f, +1.0f, +1.0f);
                                break;
                            case var _ when temp.Equals(mat2):
                                host.transform.localPosition = node050B.Vector1.yzx * new float3(+1.0f, +1.0f, -1.0f);
                                host.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                                host.transform.localScale    = new float3(-1.0f, +1.0f, +1.0f);
                                break;
                            case var _ when temp.Equals(mat3):
                                host.transform.localPosition = node050B.Vector1.xyz;
                                host.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                                host.transform.localScale    = Vector3.one;
                                break;
                            default:
                                Debug.LogError(
                                    $"Non-implemented transform for {node.GetType().Name}, " +
                                    $"{nameof(node050B.Position)} = {node050B.Position}, " +
                                    $"{nameof(node050B.Rotation)} = {node050B.Rotation}, " +
                                    $"{nameof(node050B.Vector1)} = {node050B.Vector1}",
                                    host
                                );
                                break;
                        }
                    }

                    break;
                case DMDNode07FF:
                case DMDNode08FF:
                case DMDNode0903:
                case DMDNode0B06:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(node));
            }
        }

        [SuppressMessage("ReSharper", "CommentTypo")]
        private static bool ExcludeFromHierarchy(DMDNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

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
                    return true;
                case 0x040B_9101: // power up 
                case 0x040B_9201: // power up 
                case 0x040B_9301: // power up 
                case 0x040B_9501: // power up 
                case 0x040B_9A01: // power up 
                case 0x040B_9C01: // power up 
                case 0x040B_9E01: // power up 
                    return true;
            }

            // mesh with highest LOD when there are many

            if (node is DMDNodeXXXX { Parent: { } } nodeXXXX)
            {
                if (nodeXXXX.NodeKind != nodeXXXX.Parent.OfType<DMDNodeXXXX>().Max(s => s.NodeKind))
                {
                    return true;
                }
            }

            // various stuff

            var role = node.NodeRole.ReverseEndianness();

            switch (role)
            {
                case 0x00D2: // weapon texture
                case 0x00D4: // weapon texture
                case 0x00D5: // weapon texture
                case 0x00D6: // weapon texture
                case 0x028A: // flame animation
                case 0x028B: // smoke animation
                case 0x028C: // contrail animation
                case 0x028D: // spark animation
                case 0x028E: // explosion animation
                case 0x028F: // burst animation
                case 0x0290: // burn animation
                case 0x029E: // frag texture
                case 0x029F: // frag texture
                case 0x02A0: // frag texture
                case 0x02A1: // frag texture
                case 0x02EF: // pedestrian
                case 0x02F4: // pedestrian
                case 0x02F8: // pedestrian
                case 0x0304: // pedestrian
                case 0x0305: // pedestrian
                case 0x0307: // pedestrian
                case 0x0308: // pedestrian
                case 0x0309: // pedestrian
                case 0x030A: // pedestrian
                case 0x030D: // pedestrian
                case 0x030E: // pedestrian
                case 0x02EE: // hover cop
                case 0x02F0: // hover cop
                case 0x02F1: // static cop
                case 0x02F2: // static cop
                case 0x02F3: // static cop
                case 0x04D8: // dashboard
                case 0x04DF: // rear view mirror
                case 0x04EC: // dashboard icons
                case 0x0500: // steering wheel
                case 0x012D: // special bullet
                case 0x012E: // special bullet
                case 0x0134: // special bullet
                case 0x0135: // special bullet
                    return true;
                case 0x0398: // health stand
                case 0x0399: // health stand lightniing
                case 0x0514: // car occupants
                    break;
            }

            return false;
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

        private static Mesh ConfigureModel(DMDNode00FF node, ViewerTexturing texturing, IReadOnlyList<Polygon> polygons)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (texturing is null)
                throw new ArgumentNullException(nameof(texturing));

            var meshes = new List<Mesh>();
            var groups = polygons.GroupBy(s => s.TextureInfo.HasValue).ToArray();
            var matrix = node.GetWorldTransform();

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

                            Profiler.BeginSample($"{nameof(ViewerPreview)} get vertex");
                            vertices.Add(math.transform(matrix, new Vector3(vertex.X, vertex.Y, vertex.Z)));
                            Profiler.EndSample();

                            Profiler.BeginSample($"{nameof(ViewerPreview)} get color");
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
                                var key = polygon.TextureInfo.Value;

                                if (!texturing.AtlasIndices.TryGetValue(key, out var index))
                                {
                                    Debug.LogError($"Couldn't find texture in atlas dictionary, Node = {node}, Key = {key}"); // BUG
                                }

                                var uv = polygon.TextureUVs[l];

                                Profiler.BeginSample($"{nameof(ViewerPreview)} get UV");
                                uvs.Add(texturing.Atlas.GetUV(index, uv, false, TextureTransform.FlipY));
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

            Profiler.BeginSample($"{nameof(ViewerPreview)} combine meshes");

            var combine = meshes.Select(s => new CombineInstance { mesh = s }).ToArray();

            var mesh = new Mesh();

            mesh.CombineMeshes(combine, false, false, false);

            Profiler.EndSample();

            return mesh;
        }
    }
}