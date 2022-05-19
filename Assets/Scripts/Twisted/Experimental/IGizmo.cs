using System;
using System.Linq;
using UnityEngine;

namespace Twisted.Experimental
{
    /// <summary>
    ///     Z-sorted gizmos.
    /// </summary>
    public interface IGizmo
    {
        Vector3 Position { get; }

        Color Color { get; }

        Matrix4x4 Matrix { get; }

        private protected void Draw();

        public static void DrawGizmos(Camera camera, params IGizmo[] gizmos)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));

            var matrix = Gizmos.matrix;
            var color  = Gizmos.color;

            foreach (var gizmo in gizmos.OrderBy(s => (camera.worldToCameraMatrix * s.Matrix).MultiplyPoint(s.Position).z))
            {
                Gizmos.matrix = gizmo.Matrix;
                Gizmos.color  = gizmo.Color;
                gizmo.Draw();
            }

            Gizmos.matrix = matrix;
            Gizmos.color  = color;
        }

        public readonly struct Cube : IGizmo
        {
            public Cube(Vector3 center, Vector3 size, Color? color = default, Matrix4x4? matrix = default)
            {
                Center = center;
                Size   = size;
                Color  = color ?? Color.white;
                Matrix = matrix ?? Matrix4x4.identity;
            }

            private Vector3 Center { get; }

            private Vector3 Size { get; }

            public Vector3 Position => Center;

            public Color Color { get; }

            public Matrix4x4 Matrix { get; }

            void IGizmo.Draw()
            {
                Gizmos.DrawCube(Center, Size);
            }
        }

        public readonly struct Line : IGizmo
        {
            public Line(Vector3 from, Vector3 to, Color? color = default, Matrix4x4? matrix = default)
            {
                From   = from;
                To     = to;
                Color  = color ?? Color.white;
                Matrix = matrix ?? Matrix4x4.identity;
            }

            private Vector3 From { get; }

            private Vector3 To { get; }

            public Vector3 Position => From;

            public Color Color { get; }

            public Matrix4x4 Matrix { get; }

            void IGizmo.Draw()
            {
                Gizmos.DrawLine(From, To);
            }
        }

        public readonly struct Ray : IGizmo
        {
            public Ray(Vector3 from, Vector3 direction, Color? color = default, Matrix4x4? matrix = default)
            {
                From      = from;
                Direction = direction;
                Color     = color ?? Color.white;
                Matrix    = matrix ?? Matrix4x4.identity;
            }

            private Vector3 From { get; }

            private Vector3 Direction { get; }

            public Vector3 Position => From;

            public Color Color { get; }

            public Matrix4x4 Matrix { get; }

            void IGizmo.Draw()
            {
                Gizmos.DrawRay(From, Direction);
            }
        }

        public readonly struct Sphere : IGizmo
        {
            public Sphere(Vector3 center, float size, Color? color = default, Matrix4x4? matrix = default)
            {
                Center = center;
                Size   = size;
                Color  = color ?? Color.white;
                Matrix = matrix ?? Matrix4x4.identity;
            }

            private Vector3 Center { get; }

            private float Size { get; }

            public Vector3 Position => Center;

            public Color Color { get; }

            public Matrix4x4 Matrix { get; }

            void IGizmo.Draw()
            {
                Gizmos.DrawSphere(Center, Size);
            }
        }

        public readonly struct WireCube : IGizmo
        {
            public WireCube(Vector3 center, Vector3 size, Color? color = default, Matrix4x4? matrix = default)
            {
                Center = center;
                Size   = size;
                Color  = color ?? Color.white;
                Matrix = matrix ?? Matrix4x4.identity;
            }

            private Vector3 Center { get; }

            private Vector3 Size { get; }

            Vector3 IGizmo.Position => Center;

            public Color Color { get; }

            public Matrix4x4 Matrix { get; }

            void IGizmo.Draw()
            {
                Gizmos.DrawWireCube(Center, Size);
            }
        }

        public readonly struct WireSphere : IGizmo
        {
            public WireSphere(Vector3 center, float size, Color? color = default, Matrix4x4? matrix = default)
            {
                Center = center;
                Size   = size;
                Color  = color ?? Color.white;
                Matrix = matrix ?? Matrix4x4.identity;
            }

            private Vector3 Center { get; }

            private float Size { get; }

            public Vector3 Position => Center;

            public Color Color { get; }

            public Matrix4x4 Matrix { get; }

            void IGizmo.Draw()
            {
                Gizmos.DrawWireSphere(Center, Size);
            }
        }
    }
}