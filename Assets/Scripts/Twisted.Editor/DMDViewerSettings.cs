using UnityEditor;

namespace Twisted.Editor
{
    [FilePath("UserSettings/DMDViewer.yaml", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class DMDViewerSettings : ScriptableSingleton<DMDViewerSettings>
    {
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