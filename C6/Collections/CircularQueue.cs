// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;
using static C6.Speed;

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

        #region Code Contracts

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // ReSharper disable InvocationIsSkipped

            // Array is non-null
            Invariant(_items != null);

            // Count is not bigger than the capacity
            Invariant(Count <= Capacity);

            // All items must be non-null if collection disallows null values
            Invariant(AllowsNull || ForAll(this, item => item != null));

            // Pointers are within bounds
            Invariant(0 <= _front && _front < Capacity || Count == 0 && _front == 0);
            Invariant(0 <= _back && _back < Capacity || Count == 0 && _back == 0);
            
            // _front points to the first item in the queue, _back points to the index after the last item, or to the first index if the queue is at the end of the array
            Invariant(_back - _front == Count || Capacity - _front + _back == Count || _front == _back && Count == Capacity);

            // The unused part of the array contains default values
            // TODO: Invariant(ForAll(Count, Capacity, i => Equals(_items[i], default(T))));

            // Equality comparer is non-null
            // TODO: Invariant(EqualityComparer != null);

            // Empty array is always empty
            Invariant(EmptyArray.IsEmpty());

            // ReSharper restore InvocationIsSkipped
        }

        #endregion

        #region Constructors

        private CircularQueue(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
        {
            #region Code Contracts

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            #endregion


            //EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;
            AllowsNull = allowsNull;
        }

        public CircularQueue(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) : this(equalityComparer, allowsNull)
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

            // Count is equal to the number of items in the enumerable
            Ensures(base.Count == items.Count());

            // ReSharper restore InvocationIsSkipped

            #endregion
            
            // TODO: Check enumerable type
            _items = items.ToArray();
            base.Count = Capacity;
        }

        public CircularQueue(int capacity = 0, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) : this(equalityComparer, allowsNull)
        {
            #region Code Contracts

            // ReSharper disable InvocationIsSkipped

            // Argument must be non-negative
            Requires(0 <= capacity, ArgumentMustBeNonNegative);

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            // ReSharper restore InvocationIsSkipped

            #endregion

            _items = EmptyArray;
            Capacity = capacity;
        }

        #endregion

        #region Properties

        public override bool AllowsNull { get; }

        public override Speed CountSpeed => Constant;

        #endregion

        #region Methods

        /// <summary>
        ///     Gets or sets the total number of items the internal data structure can hold without resizing.
        /// </summary>
        /// <value>
        ///     The number of items that the <see cref="CircularQueue{T}"/> can contain before resizing is required.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         <see cref="Capacity"/> is the number of items that the <see cref="CircularQueue{T}"/> can store before resizing
        ///         is required, whereas <see cref="ICollectionValue{T}.Count"/> is the number of items that are actually in the
        ///         <see cref="CircularQueue{T}"/>.
        ///     </para>
        ///     <para>
        ///         If the capacity is significantly larger than the count and you want to reduce the memory used by the
        ///         <see cref="CircularQueue{T}"/>, you can decrease capacity by calling the <see cref="TrimExcess"/> method or by
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

                    var array = new T[value];
                    CopyTo(array, 0);
                    _items = array;
                }
                else {
                    _items = EmptyArray;
                }
            }
        }

        public override T Choose() => _items[_front];

        public override SCG.IEnumerator<T> GetEnumerator()
        {
            var index = _front;
            var end = _front < _back ? _back : Capacity;

            while (index < end) {
                // TODO: CheckVersion(version);
                yield return _items[index++];
            }

            if (_front > _back) {
                index = 0;
                while (index < _back) {
                    // TODO: CheckVersion(version);
                    yield return _items[index++];
                }
            }
        }

        #endregion
    }
}