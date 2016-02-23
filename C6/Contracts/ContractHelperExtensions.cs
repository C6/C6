// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using SCG = System.Collections.Generic;

using static System.Diagnostics.Contracts.Contract;


namespace C6
{
    public static class ContractHelperExtensions
    {
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of
        /// a sequence until index <paramref name="startIndex"/>, then bypasses
        /// the next <paramref name="count"/> elements in the sequence and then
        /// returns the remaining elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of
        /// <paramref name="enumerable"/>.</typeparam>
        /// <param name="enumerable">An <see cref="SCG.IEnumerable{T}"/> to return
        /// elements from.</param>
        /// <param name="startIndex">The number of elements to return before
        /// skipping the next <paramref name="count"/> elements.</param>
        /// <param name="count">The number of elements to skip before returning
        /// the remaining elements.</param>
        /// <returns>An <see cref="SCG.IEnumerable{T}"/> containing the elements 
        /// from the specified <see cref="SCG.IEnumerable{T}"/>, but with a range
        /// of items skipped.</returns>
        /// <remarks>This is only intended for code contracts, and is not 
        /// optimal in any sense.</remarks>
        public static SCG.IEnumerable<T> SkipRange<T>(this SCG.IEnumerable<T> enumerable, int startIndex, int count)
        {
            // Argument is non-null
            Requires(enumerable != null);

            // ReSharper disable PossibleMultipleEnumeration
            return enumerable.Take(startIndex).Concat(enumerable.Skip(startIndex + count));
            // ReSharper enable PossibleMultipleEnumeration
        }

        /// <summary>
        /// Determines whether two enumerables contain the same elements in
        /// regards to multiplicity, but not sequence order, using the default
        /// equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input
        /// sequences.</typeparam>
        /// <param name="first">An <see cref="SCG.IEnumerable{T}"/> to compare
        /// to <paramref name="second"/>.</param>
        /// <param name="second">An <see cref="SCG.IEnumerable{T}"/> to compare
        /// to <paramref name="first"/>.</param>
        /// <returns><c>true</c> if the enumerables contain equal items;
        /// otherwise, <c>false</c>.</returns>
        [Pure]
        public static bool UnsequenceEqual<T>(this SCG.IEnumerable<T> first, SCG.IEnumerable<T> second)
        {
            return UnsequenceEqual(first, second, SCG.EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Determines whether two enumerables contain the same elements in
        /// regards to multiplicity, but not sequence order, using a specified
        /// <see cref="SCG.IEqualityComparer{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input
        /// sequences.</typeparam>
        /// <param name="first">An <see cref="SCG.IEnumerable{T}"/> to compare
        /// to <paramref name="second"/>.</param>
        /// <param name="second">An <see cref="SCG.IEnumerable{T}"/> to compare
        /// to <paramref name="first"/>.</param>
        /// <param name="comparer">An <see cref="SCG.IEqualityComparer{T}"/> to
        /// use to compare elements.</param>
        /// <returns><c>true</c> if the enumerables contain equal items;
        /// otherwise, <c>false</c>.</returns>
        [Pure]
        public static bool UnsequenceEqual<T>(this SCG.IEnumerable<T> first, SCG.IEnumerable<T> second, SCG.IEqualityComparer<T> comparer)
        {
            if (ReferenceEquals(first, second)) {
                return true;
            }

            if (first == null || second == null) {
                return false;
            }

            var firstArray = first as T[] ?? first.ToArray();
            var secondArray = second as T[] ?? second.ToArray();

            if (firstArray.Length != secondArray.Length) {
                return false;
            }

            // Use default comparer if none is supplied
            comparer = comparer ?? SCG.EqualityComparer<T>.Default;
            
            // Sort based on hash code
            Comparison<T> hashCodeComparison = (x, y) => comparer.GetHashCode(x).CompareTo(comparer.GetHashCode(y));
            Array.Sort(firstArray, hashCodeComparison);
            Array.Sort(secondArray, hashCodeComparison);
            
            for (var i = 0; i < firstArray.Length; i++) {
                var found = false;
                var firstElement = firstArray[i];

                for (var j = i; j < secondArray.Length; j++) {
                    var secondElement = secondArray[j];
                    
                    if (hashCodeComparison(firstElement, secondElement) != 0) {
                        break;
                    }

                    if (comparer.Equals(firstElement, secondElement)) {
                        secondArray.Swap(i, j);

                        // Continue with next element in first array
                        found = true;
                        break;
                    }
                }
                
                if (!found) {
                    return false;
                }
            }
            
            return true;
        }
    }
}