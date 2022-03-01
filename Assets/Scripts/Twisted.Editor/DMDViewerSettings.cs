using UnityEditor;

namespace Twisted.Editor
{
    [FilePath("UserSettings/DMDViewer.yaml", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class DMDViewerSettings : ScriptableSingleton<DMDViewerSettings>
    {
        public string? LastDirectory;

        public bool SceneViewFraming = true;

        public bool DistinctFiltering = true;

        private void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            Save(true);
        }
    }
}