// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;



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
        /// <param name="enumerable">An <see cref="IEnumerable{T}"/> to return
        /// elements from.</param>
        /// <param name="startIndex">The number of elements to return before
        /// skipping the next <paramref name="count"/> elements.</param>
        /// <param name="count">The number of elements to skip before returning
        /// the remaining elements.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the elements 
        /// from the specified <see cref="IEnumerable{T}"/>, but with a range
        /// of items skipped.</returns>
        /// <remarks>This is only intended for code contracts, and is not 
        /// optimal in any sense.</remarks>
        public static IEnumerable<T> SkipRange<T>(this IEnumerable<T> enumerable, int startIndex, int count)
        {
            // Argument is non-null
            Contract.Requires(enumerable != null);

            // ReSharper disable PossibleMultipleEnumeration
            return enumerable.Take(startIndex).Concat(enumerable.Skip(startIndex + count));
            // ReSharper enable PossibleMultipleEnumeration
        }
    }
}
