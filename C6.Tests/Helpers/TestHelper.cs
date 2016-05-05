// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public static class TestHelper
    {
        public static int GetCount(Random random)
            => random.Next(10, 20);

        public static int[] GetIntegers(Random random)
            => GetIntegers(random, GetCount(random));

        public static int[] GetIntegers(Random random, int count)
            => Enumerable.Range(0, count).Select(i => random.Next()).ToArray();

        public static KeyValuePair<int, int>[] GetKeyValuePairs(Random random)
            => GetKeyValuePairs(random, GetCount(random));

        public static KeyValuePair<int, int>[] GetKeyValuePairs(Random random, int count)
            => Enumerable.Range(0, count).Select(i => new KeyValuePair<int, int>(random.Next(), random.Next())).ToArray();

        public static int GetIndex<T>(ICollectionValue<T> collection, Random random, bool includeCount = false) => random.Next(0, collection.Count + (includeCount ? 1 : 0));

        public static T[] InsertItem<T>(this SCG.IEnumerable<T> enumerable, int index, T item) => enumerable.Take(index).Append(item).Concat(enumerable.Skip(index)).ToArray();
        public static T[] InsertItems<T>(this SCG.IEnumerable<T> enumerable, int index, SCG.IEnumerable<T> items) => enumerable.Take(index).Concat(items).Concat(enumerable.Skip(index)).ToArray();

        public static string[] GetStrings(Randomizer random)
            => GetStrings(random, GetCount(random));

        public static string[] GetStrings(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => random.GetString()).ToArray();

        public static string[] GetUppercaseStrings(Randomizer random)
            => GetUppercaseStrings(random, GetCount(random));

        public static string[] GetUppercaseStrings(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => GetUppercaseString(random)).ToArray();

        public static string[] GetLowercaseStrings(Randomizer random)
            => GetLowercaseStrings(random, GetCount(random));

        public static string[] GetLowercaseStrings(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => GetLowercaseString(random)).ToArray();

        public static string GetUppercaseString(Randomizer random) => random.GetString(25, "ABCDEFGHJKLMNOPQRSTUVWXYZ");
        public static string GetLowercaseString(Randomizer random) => random.GetString(25, "abcdefghijkmnopqrstuvwxyz");

        public static T Choose<T>(this T[] array, Random random) => array[random.Next(array.Length)];

        public static int IndexOf<T>(this T[] array, T item, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            Contract.Requires(array.Contains(item, equalityComparer));

            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }

            for (var i = 0; i < array.Length; i++) {
                if (equalityComparer.Equals(array[i], item)) {
                    return i;
                }
            }

            throw new Exception();
        }

        public static int LastIndexOf<T>(this T[] array, T item, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            Contract.Requires(array.Contains(item, equalityComparer));

            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }

            var index = -1;
            for (var i = 0; i < array.Length; i++) {
                if (equalityComparer.Equals(array[i], item)) {
                    index = i;
                }
            }

            if (index >= 0) {
                return index;
            }

            throw new Exception();
        }

        public static SCG.IEnumerable<string> NoStrings => Enumerable.Empty<string>();

        public static T[] WithNull<T>(this T[] array, Random random) where T : class
        {
            array[random.Next(0, array.Length)] = null;
            return array;
        }

        public static SCG.IEqualityComparer<string> ReferenceEqualityComparer => ComparerFactory.CreateReferenceEqualityComparer<string>();

        public static SCG.IEqualityComparer<KeyValuePair<TKey, TValue>> KeyEqualityComparer<TKey, TValue>() => ComparerFactory.CreateEqualityComparer<KeyValuePair<TKey, TValue>>((x, y) => x.Key.Equals(y.Key), x => x.Key.GetHashCode());

        public static CollectionEventHolder<T> Raises<T>(CollectionEvent<T>[] expectedEvents) => new CollectionEventHolder<T>(expectedEvents);

        public static CollectionEventConstraint<T> RaisesNoEventsFor<T>(IListenable<T> collection) => new CollectionEventConstraint<T>(collection, new CollectionEvent<T>[0]);
        public static CollectionEventConstraint<T> RaisesCollectionChangedEventFor<T>(IListenable<T> collection) => new CollectionEventConstraint<T>(collection, new[] { CollectionEvent.Changed(collection) });

        public static EqualConstraint Because(this ExactTypeConstraint constraint, string exceptionMessage) => constraint.With.Message.EqualTo(exceptionMessage);

        public static BadEnumerable<T> AsBadEnumerable<T>(this SCG.IEnumerable<T> enumerable) => new BadEnumerable<T>(enumerable);

        public static T DifferentItem<T>(this SCG.IEnumerable<T> items, Func<T> newItem, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }

            var item = newItem();
            while (items.Contains(item, equalityComparer)) {
                item = newItem();
            }
            return item;
        }

        public static T[] Repeat<T>(this T item, int count) => Repeat(() => item, count);

        public static T[] Repeat<T>(this Func<T> item, int count) => Enumerable.Range(0, count).Select(i => item()).ToArray();

        public static T[] WithRepeatedItem<T>(this SCG.IEnumerable<T> items, Func<T> item, int count, Random random)
        {
            var array = items.Concat(item.Repeat(count)).ToArray();
            array.Shuffle(random);
            return array;
        }

        public static T[] ShuffledCopy<T>(this SCG.IEnumerable<T> enumerable, Random random)
        {
            var copy = enumerable.ToArray();
            copy.Shuffle(random);
            return copy;
        }
    }
}