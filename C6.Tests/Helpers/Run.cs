// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using NUnit.Framework;


namespace C6.Tests.Helpers
{
    public static class Run
    {
        public static void If(bool condition, string message = null)
        {
            if (!condition) {
                // If we simply pass the test case, errors in the condition might not get caught
                Assert.Ignore(string.IsNullOrEmpty(message) ? "Test skipped" : $"Test skipped: {message}");
            }
        }
    }
}