// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Reflection;


namespace C6.Tests.ConventionTests
{
    public abstract class ConventionTestBase
    {
        internal Assembly C6 { get; } = Assembly.Load("C6");
    }
}