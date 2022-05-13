using System;
using System.Threading;
using System.Threading.Tasks;

namespace Twisted.Editor.Threading
{
    internal struct ThreadSwitcherTask : IThreadSwitcher
    {
        public IThreadSwitcher GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted => SynchronizationContext.Current == null;

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            Task.Run(continuation);
        }
    }
}