// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6.Collections
{
    public class CircularQueue<T> : CollectionValueBase<T>
    {
        #region Fields

        private static readonly T[] EmptyArray = new T[0];

        private const int MinArrayLength = 0x00000004;
        private const int MaxArrayLength = 0x7FEFFFFF;

        private int _front, _back;

        private T[] _items;

        #endregion

        #region Constructors

        public CircularQueue(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
        {
            #region Code Contracts

            // ReSharper disable InvocationIsSkipped

            // Argument must be non-null
            Requires(items != null, ArgumentMustBeNonNull);

            // All items must be non-null if collection disallows null values
            Requires(allowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            // The specified enumerable is not equal to the array saved
            Ensures(!ReferenceEquals(items, _items));

            // ReSharper restore InvocationIsSkipped

            #endregion

            AllowsNull = allowsNull;
            //EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;

            var collectionValue = items as ICollectionValue<T>;
            var collection = items as SCG.ICollection<T>;

            // Use ToArray() for ICollectionValue<T>
            if (collectionValue != null) {
                _items = collectionValue.IsEmpty ? EmptyArray : collectionValue.ToArray();
                Count = Capacity;
            }
            // Use CopyTo() for ICollection<T>
            else if (collection != null) {
                Count = collection.Count;
                _items = Count == 0 ? EmptyArray : new T[Count];
                collection.CopyTo(_items, 0);
            }
            else {
                _items = items.ToArray();
            }
        }

        public CircularQueue(int capacity = 0, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
        {
            #region Code Contracts

            // ReSharper disable InvocationIsSkipped

            // Argument must be non-negative
            Requires(0 <= capacity, ArgumentMustBeNonNegative);

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            // ReSharper restore InvocationIsSkipped

            #endregion

            AllowsNull = allowsNull;
            Capacity = capacity;
            // EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;
        }

        #endregion

        #region Properties

        public override bool AllowsNull { get; }

        public override Speed CountSpeed { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets or sets the total number of items the internal data structure can hold without resizing.
        /// </summary>
        /// <value>
        ///     The number of items that the <see cref="ArrayList{T}"/> can contain before resizing is required.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         <see cref="Capacity"/> is the number of items that the <see cref="ArrayList{T}"/> can store before resizing is
        ///         required, whereas <see cref="ICollectionValue{T}.Count"/> is the number of items that are actually in the
        ///         <see cref="ArrayList{T}"/>.
        ///     </para>
        ///     <para>
        ///         If the capacity is significantly larger than the count and you want to reduce the memory used by the
        ///         <see cref="ArrayList{T}"/>, you can decrease capacity by calling the <see cref="TrimExcess"/> method or by
        ///         setting the <see cref="Capacity"/> property explicitly to a lower value. When the value of
        ///         <see cref="Capacity"/> is set explicitly, the internal data structure is also reallocated to accommodate the
        ///         specified capacity, and all the items are copied.
        ///     </para>
        /// </remarks>
        public int Capacity
        {
            get { return _items.Length; }
            set {
                #region Code Contracts

                // Capacity must be at least as big as the number of items
                Requires(value >= Count);

                // Capacity is at least as big as the number of items
                Ensures(value >= Count);

                Ensures(Capacity == value);

                #endregion

                if (value > 0) {
                    if (value == _items.Length) {
                        return;
                    }

                    // TODO: Fix
                    Array.Resize(ref _items, value);
                }
                else {
                    _items = EmptyArray;
                }
            }
        }

        public override T Choose()
        {
            throw new NotImplementedException();
        }

        public override SCG.IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}