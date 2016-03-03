// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using SCG = System.Collections.Generic;


namespace C6
{
    /// <summary>
    /// Describes an enumerable that can be enumerated forwards and backwards.
    /// </summary>
    /// <typeparam name="T">The type of items to enumerate.</typeparam>
    [ContractClass(typeof(IDirectedEnumerableContract<>))]
    public interface IDirectedEnumerable<out T> : SCG.IEnumerable<T>
    {
        /// <summary>
        /// Gets a value indicating the enumeration direction relative to the original collection.
        /// </summary>
        /// <value>The enumeration direction relative to the original collection.
        /// <see cref="EnumerationDirection.Forwards"/> if the same;
        /// otherwise, <see cref="EnumerationDirection.Backwards"/>.</value>
        [Pure]
        EnumerationDirection Direction { get; }

        /// <summary>
        /// Returns an <see cref="IDirectedEnumerable{T}"/> that contains the
        /// same items as this <see cref="IDirectedEnumerable{T}"/>, but whose
        /// enumerator will enumerate the items backwards (in opposite order).
        /// </summary>
        /// <returns>The <see cref="IDirectedEnumerable{T}"/> whose enumerator
        /// will enumerate the items backwards.</returns>
        /// <remarks>The <see cref="IDirectedEnumerable{T}"/> becomes invalid, 
        /// if the original is modified. The method is typically used as in 
        /// <c>foreach (var item in coll.Backwards()) {...}</c>.</remarks>
        [Pure]
        IDirectedEnumerable<T> Backwards();
    }


    [ContractClassFor(typeof(IDirectedEnumerable<>))]
    internal abstract class IDirectedEnumerableContract<T> : IDirectedEnumerable<T>
    {
        // ReSharper disable InvocationIsSkipped

        public EnumerationDirection Direction
        {
            get
            {
                // No preconditions


                // Result is a valid enum constant
                Ensures(Enum.IsDefined(typeof(EnumerationDirection), Result<EnumerationDirection>()));


                return default(EnumerationDirection);
            }
        }

        public IDirectedEnumerable<T> Backwards()
        {
            // No preconditions


            // Result is non-null
            Ensures(Result<IDirectedEnumerable<T>>() != null);

            // Result enumeration is backwards
            Ensures(this.Reverse().SequenceEqual(Result<IDirectedEnumerable<T>>())); // TODO: Use specific comparer?

            // Result direction is opposite
            Ensures(Result<IDirectedEnumerable<T>>().Direction != Direction);


            return default(IDirectedEnumerable<T>);
        }

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        #region SCG.IEnumerable<T>

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #endregion
    }
}