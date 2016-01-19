﻿using NUnit.Framework;
using SCG = System.Collections.Generic;



namespace C6.Tests
{
    [TestFixture]
    public class InsertionSortTests : SortingTests
    {
        protected override void Sort<T>(T[] array, SCG.IComparer<T> comparer = null)
        {
            Sorting.InsertionSort(array, comparer);
        }


        protected override void Sort<T>(T[] array, int start, int count, SCG.IComparer<T> comparer = null)
        {
            Sorting.InsertionSort(array, start, count, comparer);
        }
    }
}
