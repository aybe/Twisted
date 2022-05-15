using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Twisted.Editor
{
    public static class ViewerUtility
    {
        public static void Frame(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

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
    }
}