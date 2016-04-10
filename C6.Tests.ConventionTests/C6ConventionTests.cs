// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Reflection;

using NUnit.Framework;

using TestStack.ConventionTests;
using TestStack.ConventionTests.ConventionData;


namespace C6.Tests.ConventionTests
{
    [TestFixture]
    public class C6ConventionTests : ConventionTestBase
    {
        [Test]
        public void Convention_AllClassesMustBeSerializable()
        {
            var types = Types.InAssembly(C6);
            Convention.Is(new AllClassesAreSerializable(), types);
        }
    }
}