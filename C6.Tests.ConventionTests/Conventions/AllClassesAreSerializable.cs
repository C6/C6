// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using TestStack.ConventionTests;
using TestStack.ConventionTests.ConventionData;


namespace C6.Tests.ConventionTests
{
    public class AllClassesAreSerializable : IConvention<Types>
    {
        public void Execute(Types data, IConventionResultContext result)
        {
            var nonSerializableTypes = data.Where(type => !type.IsSerializable &&
                                                          !type.IsInterface &&
                                                          !IsStatic(type) &&
                                                          !type.IsCompilerGenerated() &&
                                                          !type.Name.Equals("SerializableAttribute"));

            result.Is("Classes must be serializable", nonSerializableTypes);
        }

        [Obsolete("Should be replaced with type.IsStatic() - https://github.com/TestStack/TestStack.ConventionTests/issues/73")]
        private static bool IsStatic(Type type) => type.IsClass & type.IsSealed & type.IsAbstract;

        public string ConventionReason => "All classes should be serializable: https://github.com/sestoft/C5/issues/8";
    }
}