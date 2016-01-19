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

        public static void BinaryInsertionSort<T>(T[] array, SCG.IComparer<T> comparer = null)
        {
            Contract.Requires(array != null, "array cannot be null.");

            new Sorter<T>(array, comparer).BinaryInsertionSort(0, array.Length);
        }

        public static void BinaryInsertionSort<T>(T[] array, int start, int count, SCG.IComparer<T> comparer)
        {
            ContractAbbreviatorSortingParameters(array, start, count);

            new Sorter<T>(array, comparer).BinaryInsertionSort(start, start + count);
        }

        #endregion


        private partial class Sorter<T>
        {
            public void BinaryInsertionSort(int start, int end)
            {
                for (var j = start + 1; j < end; ++j)
                {
                    // Get next
                    var next = _array[j];

                    var i = j - 1;

                    // Continue if next doesn't need moving
                    if (Compare(_array[i], next) <= 0)
                        continue;

                    // Search for position
                    var low = -1;
                    while (low + 1 < i)
                    {
                        var middle = low + (i - low >> 1);

                        if (Compare(next, _array[middle]) <= 0)
                            i = middle;
                        else
                            low = middle;
                    }

                    // Move items that are in the wrong place
                    Array.Copy(_array, i, _array, i + 1, j - i);

                    _array[i] = next;
                }
            }
        }
    }
}
