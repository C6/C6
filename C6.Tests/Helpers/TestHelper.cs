// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public static class TestHelper
    {
        public static int GetRandomCount(Random random)
            => random.Next(5, 20);

        public static SCG.IEnumerable<int> GetRandomIntEnumerable(Random random)
            => GetRandomIntEnumerable(random, GetRandomCount(random));

        public static SCG.IEnumerable<int> GetRandomIntEnumerable(Random random, int count)
            => Enumerable.Range(0, count).Select(i => random.Next());

        public static SCG.IEnumerable<string> GetRandomStringEnumerable(Randomizer random)
            => GetRandomStringEnumerable(random, GetRandomCount(random));

        public static SCG.IEnumerable<string> GetRandomStringEnumerable(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => random.GetString());

        public static SCG.IEnumerable<string> GetRandomUppercaseStringEnumerable(Randomizer random)
            => GetRandomUppercaseStringEnumerable(random, GetRandomCount(random));

        public static SCG.IEnumerable<string> GetRandomUppercaseStringEnumerable(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => GetRandomUppercaseString(random));

        public static string GetRandomUppercaseString(Randomizer random) => random.GetString(25, "ABCDEFGHJKLMNOPQRSTUVWXYZ");
        public static string GetRandomLowercaseString(Randomizer random) => random.GetString(25, "abcdefghijkmnopqrstuvwxyz");

        public static T SelectRandom<T>(this T[] array, Random random) => array[random.Next(array.Length)];

        public static CollectionEventConstraint<T> RaisingEventsFor<T>(this ConstraintExpression not, ICollectionValue<T> collection) => new CollectionEventConstraint<T>(collection, new CollectionEvent<T>[0]);
    }
}