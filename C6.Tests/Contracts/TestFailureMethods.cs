// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;


// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global


namespace C6.Tests.Contracts
{
    /// <summary>
    ///     Provides custom exception throwing for contract tests.
    /// </summary>
    /// <remarks>
    ///     The class is used by Code Contracts in the assembly properties. See the Code Contracts User Manual section 7.7.
    /// </remarks>
    public static class TestFailureMethods
    {
        public static void Requires(bool condition, string userMessage, string conditionText)
        {
            if (!condition) {
                throw new PreconditionException(userMessage, conditionText);
            }
        }


        public static void Requires<TException>(bool condition, string userMessage, string conditionText) where TException : Exception
        {
            if (!condition) {
                throw new PreconditionException(userMessage, conditionText, typeof(TException));
            }
        }
    }
}