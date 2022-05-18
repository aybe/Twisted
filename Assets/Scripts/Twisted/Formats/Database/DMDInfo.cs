namespace Twisted.Formats.Database
{
    public readonly struct DMDInfo
    {
        public DMDInfoCategory Category { get; }

        public string? Description { get; }

        public DMDInfo(DMDInfoCategory category = default, string? description = null)
        {
            Category    = category;
            Description = description;
        }

        public override string ToString()
        {
            return $"{nameof(Category)}: {Category}, {nameof(Description)}: {Description}";
        }
    }
}