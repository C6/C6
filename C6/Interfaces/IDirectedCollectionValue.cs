// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractHelperExtensions;

using SCG = System.Collections.Generic;


namespace C6
{
    // TODO: Is the count always directly available?
    // TODO: Is the enumerable always lazy?
    /// <summary>
    ///     Represents a enumerable, generic collection value that can also be reversed and enumerated backwards.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the collection.
    /// </typeparam>
    /// <remarks>
    ///     An <see cref="IDirectedCollectionValue{T}"/> behaves in the same way as an enumerable; if the original collection
    ///     is changed, any operation on the <see cref="IDirectedCollectionValue{T}"/> throws an
    ///     <see cref="InvalidOperationException"/>.
    /// </remarks>
    [ContractClass(typeof(IDirectedCollectionValueContract<>))]
    public interface IDirectedCollectionValue<T> : ICollectionValue<T>
    {
        /// <summary>
        ///     Gets a value indicating the enumeration direction relative to the original collection.
        /// </summary>
        /// <value>
        ///     The enumeration direction relative to the original collection. <see cref="EnumerationDirection.Forwards"/> if the
        ///     same; otherwise, <see cref="EnumerationDirection.Backwards"/>.
        /// </value>
        [Pure]
        EnumerationDirection Direction { get; }

        /// <summary>
        ///     Returns an <see cref="IDirectedCollectionValue{T}"/> that contains the same items as this
        ///     <see cref="IDirectedCollectionValue{T}"/>, but whose enumerator will enumerate the items backwards (in opposite
        ///     order).
        /// </summary>
        /// <returns>
        ///     The <see cref="IDirectedCollectionValue{T}"/> whose enumerator will enumerate the items backwards.
        /// </returns>
        /// <remarks>
        ///     The <see cref="IDirectedCollectionValue{T}"/> becomes invalid, if the original is modified. The method is typically
        ///     used as in <c>
        ///         foreach (var item in Backwards()) {...}
        ///     </c>.
        /// </remarks>
        [Pure]
        new IDirectedCollectionValue<T> Backwards();
    }


    [ContractClassFor(typeof(IDirectedCollectionValue<>))]
    internal abstract class IDirectedCollectionValueContract<T> : IDirectedCollectionValue<T>
    {
        // ReSharper disable InvocationIsSkipped

        public EnumerationDirection Direction
        {
            get {
                // No preconditions


                // Result is a valid enum constant
                Ensures(Enum.IsDefined(typeof(EnumerationDirection), Result<EnumerationDirection>()));


                return default(EnumerationDirection);
            }
        }
        
        public IDirectedCollectionValue<T> Backwards()
        {
            // No preconditions


            // Result is non-null
            Ensures(Result<IDirectedCollectionValue<T>>() != null);

            // Result enumeration is backwards
            Ensures(this.Reverse().IsSameSequenceAs(Result<IDirectedCollectionValue<T>>()));

            // Result direction is opposite
            Ensures(Result<IDirectedCollectionValue<T>>().Direction.IsOppositeOf(Direction));


            return default(IDirectedCollectionValue<T>);
        }

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        #region SCG.IEnumerable<T>

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IShowable

        public abstract string ToString(string format, IFormatProvider formatProvider);
        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);

        #endregion

        #region ICollectionValue<T>

        public abstract EventTypes ActiveEvents { get; }
        public abstract bool AllowsNull { get; }
        public abstract int Count { get; }
        public abstract Speed CountSpeed { get; }
        public abstract bool IsEmpty { get; }
        public abstract EventTypes ListenableEvents { get; }
        public abstract T Choose();
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract T[] ToArray();
        public abstract event EventHandler CollectionChanged;
        public abstract event EventHandler<ClearedEventArgs> CollectionCleared;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemInserted;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsAdded;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsRemoved;

        #endregion

        #endregion
    }
}