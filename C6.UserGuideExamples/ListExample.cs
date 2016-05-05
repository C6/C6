// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Collections;


namespace C6.UserGuideExamples
{
    public class ListExample
    {
        public static void Main()
        {
            // Construct list using collection initializer
            var list = new ArrayList<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };

            // Get index of item
            var index = list.IndexOf(23);

            // Get an index range
            var range = list.GetIndexRange(index, 4);

            // Print range in reverse order
            foreach (var prime in range.Backwards()) {
                Console.WriteLine(prime);
            }

            // Remove items within index range
            list.RemoveIndexRange(10, 3);

            // Remove item at index
            var second = list.RemoveAt(1);

            // Remove first item
            var first = list.RemoveFirst();

            // Remove last item
            var last = list.RemoveLast();

            // Create array with items in list
            var array = list.ToArray();

            // Clear list
            list.Clear();

            // Check if list is empty
            var isEmpty = list.IsEmpty;

            // Add item
            list.Add(first);

            // Add items from enumerable
            list.AddRange(array);

            // Insert item into list
            list.Insert(1, second);

            // Add item to the end
            list.Add(last);

            // Check if list is sorted
            var isSorted = list.IsSorted();

            // Reverse list
            list.Reverse();

            // Check if list is sorted
            var reverseComparer = ComparerFactory.CreateComparer<int>((x, y) => y.CompareTo(x));
            isSorted = list.IsSorted(reverseComparer);

            // Shuffle list
            var random = new Random(0);
            list.Shuffle(random);

            // Print list using indexer
            for (var i = 0; i < list.Count; i++) {
                Console.WriteLine($"{i,2}: {list[i],2}");
            }

            // Check if list contains all items in enumerable
            var containsRange = list.ContainsRange(array);

            // Construct list using enumerable
            var otherList = new ArrayList<int>(array);

            // Add every third items from list
            otherList.AddRange(list.Where((x, i) => i % 3 == 0));

            containsRange = list.ContainsRange(otherList);

            // Remove all items not in enumerable
            otherList.RetainRange(list);

            // Remove all items in enumerable from list
            list.RemoveRange(array);

            // Sort list
            list.Sort();

            // Copy to array
            list.CopyTo(array, 2);

            return;
        }
    }
}