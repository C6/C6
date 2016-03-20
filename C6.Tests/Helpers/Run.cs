// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using NUnit.Framework;


namespace C6.Tests.Helpers
{
    public static class Run
    {
        public static void If(bool condition, string message = null)
        {
            if (!condition) {
                Assert.Pass(string.IsNullOrEmpty(message) ? "Test skipped" : $"Test skipped: {message}");
            }
        }
    }
}