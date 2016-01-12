// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SCG = System.Collections.Generic;

namespace C6
{
    public static class Extensions
    {
        /// <summary>
        /// Appends the specified element at the end of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements the <see cref="IEnumerable{T}"/> contains.</typeparam>
        /// <param name="enumerable">The enumerable to which the item should be appended.</param>
        /// <param name="item">The item to append.</param>
        /// <returns>An <see cref="IEnumerable{T}"/>, enumerating first the
        /// items in the existing enumerable, then the item.</returns>
        [Pure]
        public static SCG.IEnumerable<T> Append<T>(this SCG.IEnumerable<T> enumerable, T item)
        {
            // Argument must be non-null
            Contract.Requires(enumerable != null); // TODO: Use <ArgumentNullException>?
            // Argument must be non-null
            // Contract.Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Result is never null
            Contract.Ensures(Contract.Result<SCG.IEnumerable<T>>() != null);


            foreach (var t in enumerable) yield return t;
            yield return item;
        }
    }
}