using System;
using System.Collections.Generic;
using Twisted.Formats.Graphics2D;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Twisted.Editor
{
    internal sealed class ViewerTexturing : IDisposable
    {
        public ViewerTexturing(
            TextureAtlas atlas,
            Dictionary<TextureInfo, int> atlasIndices,
            Texture2D atlasTexture,
            Dictionary<TextureInfo, Texture2D> atlasTextures)
        {
            Atlas         = atlas;
            AtlasIndices  = atlasIndices;
            AtlasTexture  = atlasTexture;
            AtlasTextures = atlasTextures;
        }

        public TextureAtlas Atlas { get; }

        public Dictionary<TextureInfo, int> AtlasIndices { get; }

        public Texture2D AtlasTexture { get; }

        public Dictionary<TextureInfo, Texture2D> AtlasTextures { get; }

        public void Dispose()
        {
            var editor = Application.isEditor;

            foreach (var texture in AtlasTextures.Values)
            {
                if (editor)
                {
                    Object.DestroyImmediate(texture);
                }
                else
                {
                    Object.Destroy(texture);
                }
            }
        }
    }
}