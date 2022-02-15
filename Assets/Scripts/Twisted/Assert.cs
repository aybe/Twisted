
using System;
#if UNITY_ASSERTIONS
using UnityEngine.Assertions;
using AssertImpl = UnityEngine.Assertions.Assert;

#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertImpl = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
#endif

namespace Twisted
{
    [Obsolete("Inline these members.")]
    internal static class Assert
    {
        public static void AreEqual<T>(T expected, T actual, string? message = null)
        {
            AssertImpl.AreEqual(expected, actual, message);
        }

        public static void AreNotEqual<T>(T expected, T actual, string? message = null)
        {
            AssertImpl.AreNotEqual(expected, actual, message);
        }

        public static void Fail(string? message = null)
        {
#if UNITY_ASSERTIONS
            throw new AssertionException(message, null);
#else
            throw new AssertFailedException(message);
#endif
        }

        public static void IsFalse(bool condition, string? message = null)
        {
            AssertImpl.IsFalse(condition, message);
        }

        public static void IsTrue(bool condition, string? message = null)
        {
            AssertImpl.IsTrue(condition, message);
        }

        public static void IsNotNull(object value, string? message = null)
        {
#if UNITY_ASSERTIONS
            AssertImpl.IsNotNull(value, message);
#else
            AssertImpl.IsNotNull(value, message);

#endif
        }

        public static void IsNull(object value, string? message = null)
        {
#if UNITY_ASSERTIONS
            AssertImpl.IsNull(value, message);
#else
            AssertImpl.IsNull(value, message);
#endif
        }
    }
}