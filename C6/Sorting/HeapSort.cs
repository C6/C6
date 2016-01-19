// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using SCG = System.Collections.Generic;



namespace C6
{
    public static partial class Sorting
    {
        /// <summary>
        /// Heapsorts the items in an <see cref="Array"/> using either the
        /// provided <see cref="SCG.IComparer{T}"/> implementation provided
        /// or the default <see cref="SCG.Comparer{T}"/>).
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based <see cref="Array"/> to sort.</param>
        /// <param name="comparer">The <see cref="SCG.IComparer{T}"/> generic
        /// interface implementation to use when comparing elements, or
        /// <c>null</c> to use the <see cref="SCG.IComparer{T}"/> generic
        /// interface implementation of each element.</param>
        public static void HeapSort<T>(T[] array, SCG.IComparer<T> comparer = null)
        {
            ContractAbbreviatorSortingParameters(array, 0, array.Length, comparer);

            new Sorter<T>(array, comparer).HeapSort(0, array.Length);
        }



        /// <summary>
        /// </summary>
        /// <param name="array">Array to sort</param>
        /// <param name="start">Index of first position to sort</param>
        /// <param name="count">Number of elements to sort</param>
        /// <param name="comparer">IComparer&lt;T&gt; to sort by</param>
        public static void HeapSort<T>(T[] array, int start, int count, SCG.IComparer<T> comparer)
        {
            new Sorter<T>(array, comparer).HeapSort(start, start + count);
        }



        private partial class Sorter<T>
        {

            internal void HeapSort(int f, int b)
            {
                for (int i = (b + f) / 2; i >= f; i--) Heapify(f, b, i);

                for (int i = b - 1; i > f; i--)
                {
                    T tmp = _array[f]; _array[f] = _array[i]; _array[i] = tmp;
                    Heapify(f, i, f);
                }
            }

            private void Heapify(int f, int b, int i)
            {
                T pv = _array[i], lv, rv, max = pv;
                int j = i, maxpt = j;

                while (true)
                {
                    int l = 2 * j - f + 1, r = l + 1;

                    if (l < b && Compare(lv = _array[l], max) > 0) { maxpt = l; max = lv; }

                    if (r < b && Compare(rv = _array[r], max) > 0) { maxpt = r; max = rv; }

                    if (maxpt == j)
                        break;

                    _array[j] = max;
                    max = pv;
                    j = maxpt;
                }

                if (j > i)
                    _array[j] = pv;
            }
        }
    }
}
