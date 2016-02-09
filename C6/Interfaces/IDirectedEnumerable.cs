// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;

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


        /// <summary>
        /// Gets a value indicating the enumeration direction relative to the original collection.
        /// </summary>
        /// <value>The enumeration direction relative to the original collection.
        /// <see cref="EnumerationDirection.Forwards"/> if the same;
        /// otherwise, <see cref="EnumerationDirection.Backwards"/>.</value>
        [Pure]
        EnumerationDirection Direction { get; }
    }



    [ContractClassFor(typeof(IDirectedEnumerable<>))]
    internal abstract class IDirectedEnumerableContract<T> : IDirectedEnumerable<T>
    {
        // ReSharper disable InvocationIsSkipped

        public IDirectedEnumerable<T> Backwards()
        {
            // No Requires


            // Result is non-null
            Contract.Ensures(Contract.Result<IDirectedEnumerable<T>>() != null);

            // Result enumeration is backwards
            Contract.Ensures(this.Reverse().SequenceEqual(Contract.Result<IDirectedEnumerable<T>>())); // TODO: Use specific comparer?

            // Result direction is opposite
            Contract.Ensures(Contract.Result<IDirectedEnumerable<T>>().Direction != Direction);


            throw new NotImplementedException();
        }


        public EnumerationDirection Direction {
            get {
                // No Requires


                // Result is a valid enum constant
                Contract.Ensures(Enum.IsDefined(typeof(EnumerationDirection), Contract.Result<EnumerationDirection>()));


                throw new NotImplementedException();
            }
        }

        // ReSharper restore InvocationIsSkipped


        #region Non-Contract Methods

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}