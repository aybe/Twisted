using System;
using System.Threading;
using UnityEditor;

namespace Twisted.Editor.Threading
{
    internal struct ThreadSwitcherUnity : IThreadSwitcher
    {
        public IThreadSwitcher GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted => SynchronizationContext.Current == Context;

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            Context.Post(PostCallback, continuation);
        }

        private static void PostCallback(object state)
        {
            ((Action)state).Invoke();
        }

        private static SynchronizationContext Context { get; set; } = null!;

        [InitializeOnLoadMethod]
        private static void ContextCapture()
        {
            Context = SynchronizationContext.Current;
        }
    }
}