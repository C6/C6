// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using NUnit.Framework;
using NUnit.Framework.Internal;


namespace C6.Tests.Helpers
{
    public abstract class TestBase
    {
        protected static Randomizer Random => TestContext.CurrentContext.Random;
    }
}