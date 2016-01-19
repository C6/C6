// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using SCG = System.Collections.Generic;


namespace C6
{
    public static partial class Sorting
    {
        #region Sorting Methods

        /// <summary>
        /// Sorts part of array in-place using Insertion Sort.
        /// </summary>
        /// <param name="array">Array to sort</param>
        /// <param name="start">Index of first position to sort</param>
        /// <param name="count">Number of elements to sort</param>
        /// <param name="comparer">IComparer&lt;T&gt; to sort by</param>
        public static void InsertionSort<T>(T[] array, SCG.IComparer<T> comparer)
        {
            new Sorter<T>(array, comparer).InsertionSort(0, array.Length);
        }


        /// <summary>
        /// Sorts part of array in-place using Insertion Sort.
        /// </summary>
        /// <param name="array">Array to sort</param>
        /// <param name="start">Index of first position to sort</param>
        /// <param name="count">Number of elements to sort</param>
        /// <param name="comparer">IComparer&lt;T&gt; to sort by</param>
        public static void InsertionSort<T>(T[] array, int start, int count, SCG.IComparer<T> comparer)
        {
            new Sorter<T>(array, comparer).InsertionSort(start, start + count);
        }
        
        #endregion


        private partial class Sorter<T>
        {
            public void InsertionSort(int low, int high)
            {
                for (var j = low + 1; j < high; ++j)
                {
                    T key = _array[j], other;
                    var i = j - 1;

                    if (_comparer.Compare(other = _array[i], key) > 0)
                    {
                        _array[j] = other;

                        while (i > low && _comparer.Compare(other = _array[i - 1], key) > 0)
                        {
                            _array[i--] = other;
                        }

                        _array[i] = key;
                    }
                }
            }
        }
    }
}
