using System;
using System.Threading.Tasks;

namespace Twisted.Editor.Threading
{
    public static class ThreadSwitcher
    {
        public static IThreadSwitcher ResumeTaskAsync()
        {
            return new ThreadSwitcherTask();
        }

        public static IThreadSwitcher ResumeUnityAsync()
        {
            return new ThreadSwitcherUnity();
        }

        public static async Task<T> ExecuteTaskAsync<T>(Func<T> func)
        {
            await ResumeTaskAsync();

            var value = func();

            return value;
        }

        public static async Task<T> ExecuteUnityAsync<T>(Func<T> func)
        {
            await ResumeUnityAsync();

            var value = func();

            return value;
        }
    }
}