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
        public static int GetCount(Random random)
            => random.Next(5, 20);

        public static int[] GetIntegers(Random random)
            => GetIntegers(random, GetCount(random));

        public static int[] GetIntegers(Random random, int count)
            => Enumerable.Range(0, count).Select(i => random.Next()).ToArray();

        public static string[] GetStrings(Randomizer random)
            => GetStrings(random, GetCount(random));

        public static string[] GetStrings(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => random.GetString()).ToArray();

        public static string[] GetUppercaseStrings(Randomizer random)
            => GetUppercaseStrings(random, GetCount(random));

        public static string[] GetUppercaseStrings(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => GetUppercaseString(random)).ToArray();

        public static string GetUppercaseString(Randomizer random) => random.GetString(25, "ABCDEFGHJKLMNOPQRSTUVWXYZ");
        public static string GetLowercaseString(Randomizer random) => random.GetString(25, "abcdefghijkmnopqrstuvwxyz");

        public static T Choose<T>(this T[] array, Random random) => array[random.Next(array.Length)];

        public static T[] WithNull<T>(this T[] array, Random random) where T : class
        {
            array[random.Next(0, array.Length)] = null;
            return array;
        }

        public static SCG.IEqualityComparer<string> ReferenceEqualityComparer => ComparerFactory.CreateReferenceEqualityComparer<string>();

        public static CollectionEventConstraint<T> RaisingEventsFor<T>(this ConstraintExpression not, ICollectionValue<T> collection) => new CollectionEventConstraint<T>(collection, new CollectionEvent<T>[0]);

        public static BadEnumerable<T> AsBadEnumerable<T>(this SCG.IEnumerable<T> enumerable) => new BadEnumerable<T>(enumerable);
    }
}