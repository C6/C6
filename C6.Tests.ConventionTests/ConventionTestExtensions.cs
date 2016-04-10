// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;
using System.Reflection;


namespace C6.Tests.ConventionTests
{
    internal static class ConventionTestExtensions
    {
        public static bool IsC6CollectionInterface(this Type type)
            => type == typeof(ICollectionValue<>) ||
               type.GetInterfaces().Any(x => x.IsGenericType &&
                                             x.GetGenericTypeDefinition() == typeof(ICollectionValue<>));

        public static bool IsPure(this MemberInfo memberInfo) => true;
    }
}