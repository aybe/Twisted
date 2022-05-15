namespace Twisted.Editor
{
    public sealed class ProgressChangedEventArgs
    {
        public ProgressChangedEventArgs(Progress head, Progress leaf)
        {
            Head = head;
            Leaf = leaf;
        }

        public Progress Head { get; }

        public Progress Leaf { get; }
    }
}