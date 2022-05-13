namespace Twisted.Editor
{
    public sealed class ProgressChangedEventArgs
    {
        public ProgressChangedEventArgs(Progress source, float percentage)
        {
            Source     = source;
            Percentage = percentage;
        }

        public Progress Source { get; }

        public float Percentage { get; }
    }
}