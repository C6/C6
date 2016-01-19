// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using SCG = System.Collections.Generic;



namespace C6
{
    public static partial class Sorting
    {
        /// <summary>
        /// Sort part of array in place using IntroSort
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the <code>start</code>
        /// and <code>count</code> arguments does not describe a valid range.</exception>
        /// <param name="array">Array to sort</param>
        /// <param name="start">Index of first position to sort</param>
        /// <param name="count">Number of elements to sort</param>
        /// <param name="comparer">IComparer&lt;T&gt; to sort by</param>
        public static void IntroSort<T>(T[] array, int start, int count, SCG.IComparer<T> comparer)
        {
            ContractAbbreviatorSortingParameters(array, start, count, comparer);

            new Sorter<T>(array, comparer).IntroSort(start, start + count);
        }

        /// <summary>
        /// Sort an array in place using IntroSort and default comparer
        /// </summary>
        /// <exception cref="NotComparableException">If T is not comparable</exception>
        /// <param name="array">Array to sort</param>
        public static void IntroSort<T>(T[] array, SCG.IComparer<T> comparer = null)
        {
            ContractAbbreviatorSortingParameters(array, 0, array.Length); // TODO: Make start and length optional?


            new Sorter<T>(array, comparer).IntroSort(0, array.Length);
        }

        private partial class Sorter<T>
        {

            internal void IntroSort(int f, int b)
            {
                if (b - f > 31)
                {
                    int depth_limit = (int)Math.Floor(2.5 * Math.Log(b - f, 2));

                    introSort(f, b, depth_limit);
                }
                else
                    InsertionSort(f, b);
            }


            private void introSort(int f, int b, int depth_limit)
            {
                const int size_threshold = 14;//24;

                if (depth_limit-- == 0)
                    HeapSort(f, b);
                else if (b - f <= size_threshold)
                    InsertionSort(f, b);
                else
                {
                    int p = partition(f, b);

                    introSort(f, p, depth_limit);
                    introSort(p, b, depth_limit);
                }
            }
            private int partition(int f, int b)
            {
                int bot = f, mid = (b + f) / 2, top = b - 1;
                T abot = _array[bot], amid = _array[mid], atop = _array[top];

                if (Compare(abot, amid) < 0)
                {
                    if (Compare(atop, abot) < 0)//atop<abot<amid
                    { _array[top] = amid; amid = _array[mid] = abot; _array[bot] = atop; }
                    else if (Compare(atop, amid) < 0) //abot<=atop<amid
                    { _array[top] = amid; amid = _array[mid] = atop; }
                    //else abot<amid<=atop
                }
                else
                {
                    if (Compare(amid, atop) > 0) //atop<amid<=abot
                    { _array[bot] = atop; _array[top] = abot; }
                    else if (Compare(abot, atop) > 0) //amid<=atop<abot
                    { _array[bot] = amid; amid = _array[mid] = atop; _array[top] = abot; }
                    else //amid<=abot<=atop
                    { _array[bot] = amid; amid = _array[mid] = abot; }
                }

                int i = bot, j = top;

                while (true)
                {
                    while (Compare(_array[++i], amid) < 0) ;

                    while (Compare(amid, _array[--j]) < 0) ;

                    if (i < j)
                    {
                        T tmp = _array[i]; _array[i] = _array[j]; _array[j] = tmp;
                    }
                    else
                        return i;
                }
            }
        }
    }
}
