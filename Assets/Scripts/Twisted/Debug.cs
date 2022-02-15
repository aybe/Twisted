#if UNITY_DEBUG
using AssertImpl = UnityEngine.Debug;
#else
using AssertImpl = System.Diagnostics.Debug;
#endif

namespace Twisted
{
    internal static class Debug
    {
        public static void WriteLine(object? message)
        {
#if UNITY_DEBUG
            AssertImpl.Log(message);
#else
            AssertImpl.WriteLine(message);
#endif
        }
    }
}