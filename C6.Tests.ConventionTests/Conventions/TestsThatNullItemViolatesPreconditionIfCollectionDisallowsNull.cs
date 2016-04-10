// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;
using System.Reflection;

using TestStack.ConventionTests;
using TestStack.ConventionTests.ConventionData;


namespace C6.Tests.ConventionTests
{
    internal class TestsThatNullItemViolatesPreconditionIfCollectionDisallowsNull : IConvention<Types>
    {
        private const string TestAssemblyName = "C6.Tests";
        private static readonly Assembly TestAssembly = Assembly.Load(TestAssemblyName);

        public void Execute(Types data, IConventionResultContext result)
        {
            var interfaces = data.Where(type => type.IsInterface &&
                                                type.IsC6CollectionInterface());

            foreach (var @interface in interfaces) {
                var interfaceTestClass = GetTestClass(@interface);

                foreach (var methodInfo in @interface.GetMethods()) {
                    var methodName = methodInfo.Name;
                    var hides = methodInfo.IsHideBySig;

                    foreach (var parameterInfo in methodInfo.GetParameters()) {
                        var parameterType = parameterInfo.ParameterType;

                        // Parameter is an item
                        if (parameterType.Name == "T") {
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        private static Type GetTestClass(Type @interface) => TestAssembly.GetType(GetTestClassName(@interface));

        private static string GetTestClassName(Type @interface) => TestAssemblyName + "." + @interface.Name.Split('`').First() + "Tests";

        public string ConventionReason { get; }
    }
}