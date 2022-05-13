using System.Runtime.CompilerServices;

namespace Twisted.Editor.Threading
{
    public interface IThreadSwitcher : INotifyCompletion
    {
        bool IsCompleted { get; }

        IThreadSwitcher GetAwaiter();

        void GetResult();
    }
}