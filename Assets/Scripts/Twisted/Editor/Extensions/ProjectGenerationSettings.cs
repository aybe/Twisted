using UnityEditor;

namespace Twisted.Editor.Extensions
{
    [FilePath("ProjectSettings/" + nameof(ProjectGenerationSettings) + ".asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class ProjectGenerationSettings : ScriptableSingleton<ProjectGenerationSettings>
    {
        public bool Nullable = true;

        public bool NullableWarningsDisabled = true;

        public bool PreserveProjectStructure = true;

        public bool RemoveEmptyRootNamespace = true;

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