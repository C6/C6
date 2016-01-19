using NUnit.Framework;
using SCG = System.Collections.Generic;



namespace C6.Tests
{
    [TestFixture]
    public class IntroSortTests : SortingTests
    {
        protected override void Sort<T>(T[] array, SCG.IComparer<T> comparer = null)
        {
            Sorting.IntroSort(array, comparer);
        }


        protected override void Sort<T>(T[] array, int start, int count, SCG.IComparer<T> comparer = null)
        {
            Sorting.IntroSort(array, start, count, comparer);
        }
    }
}
