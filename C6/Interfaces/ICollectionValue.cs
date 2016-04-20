// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractHelperExtensions;
using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6
{
    /// <summary>
    ///     Represents a generic collection that may be enumerated and can answer efficiently how many items it contains.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the collection.
    /// </typeparam>
    /// <remarks>
    ///     Like <see cref="SCG.IEnumerable{T}"/>, the interface does not prescribe any operations to initialize or update the
    ///     collection. Its main usage is to be the return type of query operations on generic collection.
    /// </remarks>
    [ContractClass(typeof(ICollectionValueContract<>))]
    public interface ICollectionValue<T> : SCG.IEnumerable<T> // TODO: Add IShowable again
    {
        /// <summary>
        ///     Gets a value indicating whether the collection allows <c>null</c> items.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the collection allows <c>null</c> items; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         If the collection disallows <c>null</c> items, none of the items in the collection can be <c>null</c>: adding
        ///         or inserting a <c>null</c> item will result in an error, and any property or method returning an item from the
        ///         collection is guaranteed not to return <c>null</c>. If the collection allows <c>null</c> items, the user must
        ///         check for <c>null</c>.
        ///     </para>
        ///     <para>
        ///         <see cref="AllowsNull"/> does not reflect whether the collection actually contains any <c>null</c> items.
        ///     </para>
        ///     <para>
        ///         If <typeparamref name="T"/> is a value type, then <see cref="AllowsNull"/> is always <c>false</c>.
        ///     </para>
        /// </remarks>
        [Pure]
        bool AllowsNull { get; }

        /// <summary>
        ///     Gets the number of items contained in the collection.
        /// </summary>
        /// <value>
        ///     The number of items contained in the collection.
        /// </value>
        [Pure]
        int Count { get; }

        /// <summary>
        ///     Gets a value characterizing the asymptotic complexity of <see cref="Count"/> proportional to collection size
        ///     (worst-case or amortized as relevant).
        /// </summary>
        /// <value>
        ///     A characterization of the asymptotic speed of <see cref="Count"/> proportional to collection size.
        /// </value>
        [Pure]
        Speed CountSpeed { get; }

        /// <summary>
        ///     Gets a value indicating whether the collection is empty.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the collection is empty; otherwise, <c>false</c>.
        /// </value>
        [Pure]
        bool IsEmpty { get; }

        /// <summary>
        ///     Returns some item from the collection.
        /// </summary>
        /// <returns>
        ///     Some item in the collection.
        /// </returns>
        /// <remarks>
        ///     Implementations must assure that the item returned may be efficiently removed, if the collection allows removal.
        ///     However, it is not required that repeated calls give the same result.
        /// </remarks>
        [Pure]
        T Choose();

        /// <summary>
        ///     Copies the items of the collection to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array"/> that is the destination of the items copied from the collection. The
        ///     <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based arrayIndex in array at which copying begins.
        /// </param>
        [Pure]
        void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        ///     Creates an array from the collection in the same order as the enumerator would output them.
        /// </summary>
        /// <returns>
        ///     An array that contains the items from the collection.
        /// </returns>
        [Pure]
        T[] ToArray();
    }


    [ContractClassFor(typeof(ICollectionValue<>))]
    internal abstract class ICollectionValueContract<T> : ICollectionValue<T>
    {
        // ReSharper disable InvocationIsSkipped

        public bool AllowsNull
        {
            get {
                // No preconditions


                // Value types must return false
                Ensures(!typeof(T).IsValueType || !Result<bool>());


                return default(bool);
            }
        }

        public int Count
        {
            get {
                // No preconditions


                // Returns a non-negative number
                Ensures(Result<int>() >= 0);

                // Returns the same as the number of items in the enumerator
                Ensures(Result<int>() == this.Count());


                return default(int);
            }
        }

        public Speed CountSpeed
        {
            get {
                // No preconditions


                // Result is a valid enum constant
                Ensures(Enum.IsDefined(typeof(Speed), Result<Speed>()));


                return default(Speed);
            }
        }

        public bool IsEmpty
        {
            get {
                // No preconditions


                // Returns true if Count is zero, otherwise false
                Ensures(Result<bool>() == (Count == 0));

                // Returns true if the enumerator is empty, otherwise false
                Ensures(Result<bool>() != this.Any());


                return default(bool);
            }
        }

        public T Choose()
        {
            // Collection must be non-empty
            Requires(!IsEmpty, CollectionMustBeNonEmpty);


            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Return value is from the collection
            Ensures(this.ContainsSame(Result<T>()));


            return default(T);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // Argument must be non-null
            Requires(array != null, ArgumentMustBeNonNull);

            // Argument must be within bounds
            Requires(0 <= arrayIndex, ArgumentMustBeWithinBounds);
            Requires(arrayIndex + Count <= array.Length, ArgumentMustBeWithinBounds);


            // Array contains the collection's items in enumeration order from arrayIndex
            Ensures(array.Skip(arrayIndex).Take(Count).IsSameSequenceAs(this));

            // The rest of the array is unchanged
            Ensures(OldValue(array.Take(arrayIndex).ToList()).IsSameSequenceAs(array.Take(arrayIndex)));
            Ensures(OldValue(array.Skip(arrayIndex + Count).ToList()).IsSameSequenceAs(array.Skip(arrayIndex + Count)));


            return;
        }

        public T[] ToArray()
        {
            // No preconditions


            // Result is non-null
            Ensures(Result<T[]>() != null);

            // Result contains the collection's items in enumeration order
            Ensures(Result<T[]>().IsSameSequenceAs(this));


            return default(T[]);
        }

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        #region SCG.IEnumerable<T>

        // TODO: Ensure that no item is null if AllowsNull is false?
        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IShowable

        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);
        public abstract string ToString(string format, IFormatProvider formatProvider);

        #endregion

        #endregion
    }
}