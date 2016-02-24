// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using SCG = System.Collections.Generic;


namespace C6
{
    public static class Extensions
    {
        /// <summary>
        /// Appends the specified element at the end of the
        /// <see cref="SCG.IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements the
        /// <see cref="SCG.IEnumerable{T}"/> contains.</typeparam>
        /// <param name="enumerable">The enumerable to which the item should be
        /// appended.</param>
        /// <param name="item">The item to append.</param>
        /// <returns>An <see cref="SCG.IEnumerable{T}"/>, enumerating first the
        /// items in the existing enumerable, then the item.</returns>
        [Pure]
        public static SCG.IEnumerable<T> Append<T>(this SCG.IEnumerable<T> enumerable, T item)
        {
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?
            // Argument must be non-null
            // Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Result is non-null
            Ensures(Result<SCG.IEnumerable<T>>() != null);


            foreach (var t in enumerable) {
                yield return t;
            }

            yield return item;
        }


        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="SCG.IEnumerable{T}"/> is empty.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="SCG.IEnumerable{T}"/> is empty;
        /// otherwise, <c>false</c>.
        /// </value>
        [Pure]
        public static bool IsEmpty<T>(this SCG.IEnumerable<T> enumerable)
        {
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?


            // Returns true if Count is zero, otherwise false
            Ensures(Result<bool>() != enumerable.Any());


            return (enumerable as ICollectionValue<T>)?.IsEmpty ?? !enumerable.Any();
        }


        [Pure]
        public static bool Find<T>(this SCG.IEnumerable<T> enumerable, Func<T, bool> predicate, out T item)
        {
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-null
            Requires(predicate != null); // TODO: Use <ArgumentNullException>?


            // Returns true if p(x) returned true for some item x; otherwise false
            Ensures(Result<bool>() == enumerable.Any(predicate));

            // Result item equals the first (or default) item satisfying the predicate
            Ensures(ValueAtReturn(out item).Equals(enumerable.FirstOrDefault(predicate)));


            bool result;

            using (var enumerator = enumerable.Where(predicate).GetEnumerator()) {
                // ReSharper disable once AssignmentInConditionalExpression
                item = (result = enumerator.MoveNext()) ? enumerator.Current : default(T);
            }

            return result;
        }


        // TODO: Implement and document
        [Pure]
        public static int FindIndex<T>(this IIndexed<T> collection, Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }


        // TODO: Implement and document
        [Pure]
        public static int FindLastIndex<T>(this IIndexed<T> collection, Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}