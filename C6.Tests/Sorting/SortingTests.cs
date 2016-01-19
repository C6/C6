using System;
using System.Linq;
using NUnit.Framework;
using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class SortingTests
    {
        #region Private Fields

        private const int Count = 10000;

        #endregion


        #region Abstract Methods

        protected abstract void Sort<T>(T[] array, SCG.IComparer<T> comparer = null);

        protected abstract void Sort<T>(T[] array, int start, int count, SCG.IComparer<T> comparer = null);

        #endregion


        #region Utilities
        
        private static Random Random { get; } = new Random();


        private static readonly int[][] IntArrays = {
            GetEmptyArray(),
            GetIncreasingArray(),
            GetDecreasingArray(),
            GetRandomArray(),
            GetRandomDuplicatesArray(),
        };


        private static int[] GetEmptyArray()
            => new int[0];


        private static int[] GetIncreasingArray()
            => Enumerable.Range(0, Count).ToArray();


        private static int[] GetDecreasingArray()
            => Enumerable.Range(0, Count).Reverse().ToArray();


        private static int[] GetRandomArray()
        {
            var random = Random;
            var array = new int[Count];

            for (var i = 0; i < Count; ++i)
            {
                array[i] = random.Next();
            }

            return array;
        }


        private static int[] GetRandomDuplicatesArray()
        {
            var random = Random;
            var array = new int[Count];

            for (var i = 0; i < Count; ++i)
            {
                array[i] = random.Next(3, 23);
            }

            return array;
        }


        private static readonly SCG.IComparer<int>[] IntComparers = {
            null,
            SCG.Comparer<int>.Default,
        };


        #endregion


        #region Tests

        [Test]
        [Combinatorial]
        public void Sort_CompleteArray_IsOrdered(
            [ValueSource(typeof(SortingTests), nameof(IntArrays))] int[] array,
            [ValueSource(typeof(SortingTests), nameof(IntComparers))] SCG.IComparer<int> comparer
        )
        {
            Sort(array, comparer);

            Assert.That(array, Is.Ordered.Using(comparer ?? SCG.Comparer<int>.Default));
        }


        [Test]
        [Combinatorial]
        public void Sort_CompleteArrayUsingStartAndCount_IsOrdered(
            [ValueSource(typeof(SortingTests), nameof(IntArrays))] int[] array,
            [ValueSource(typeof(SortingTests), nameof(IntComparers))] SCG.IComparer<int> comparer
        )
        {
            Sort(array, 0, array.Length, comparer);

            Assert.That(array, Is.Ordered.Using(comparer ?? SCG.Comparer<int>.Default));
        }


        [Test]
        [Combinatorial]
        public void Sort_RandomSubrange_IsOrdered(
            [ValueSource(typeof(SortingTests), nameof(IntArrays))] int[] array,
            [ValueSource(typeof(SortingTests), nameof(IntComparers))] SCG.IComparer<int> comparer
        )
        {
            var random = Random;
            var start = random.Next(0, array.Length);
            var count = random.Next(start, array.Length) - start;

            Sort(array, start, count, comparer);
            var subrange = array.Skip(start).Take(count);

            Assert.That(subrange, Is.Ordered.Using(comparer ?? SCG.Comparer<int>.Default));
        }

        #endregion
    }
}
