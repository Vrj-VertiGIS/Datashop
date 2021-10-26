using System;
using System.Reflection;
using NUnit.Framework;

namespace GNSDatashopTest.TestUtils
{
    /// <summary>
    /// Extension class of Type to enable testing of non-public methods
    /// </summary>
    internal static class NonpublicMethod
    {
        /// <summary>
        /// Gets the instance method.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        internal static MethodInfo GetInstanceMethod(this Type t, string methodName)
        {
            return GetMethod(t, methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Gets the static method.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        internal static MethodInfo GetStaticMethod(this Type t, string methodName)
        {
            return GetMethod(t, methodName, BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns></returns>
        private static MethodInfo GetMethod(IReflect t, string methodName, BindingFlags bindingFlags)
        {
            if (string.IsNullOrEmpty(methodName))
                Assert.Fail("methodName cannot be null or empty");

            var method = t.GetMethod(methodName, bindingFlags);

            if (method == null)
                Assert.Fail(string.Format("{0} method not found", methodName));

            return method;
        }
    }
}