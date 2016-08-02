// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;
using static C6.Speed;

using SCG = System.Collections.Generic;


namespace C6.Collections
{
    public class CircularArrayQueue<T> : ListenableBase<T>, IQueue<T>
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

        private CircularArrayQueue(bool allowsNull = false)
        {
            #region Code Contracts

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            #endregion

            AllowsNull = allowsNull;
        }

        public CircularArrayQueue(SCG.IEnumerable<T> items, bool allowsNull = false) : this(allowsNull)
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
            base.Count = _items.Length;
        }

        public CircularArrayQueue(int capacity = 0, bool allowsNull = false) : this(allowsNull)
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

        /// <summary>
        ///     Gets or sets the total number of items the internal data structure can hold without resizing.
        /// </summary>
        /// <value>
        ///     The number of items that the <see cref="CircularArrayQueue{T}"/> can contain before resizing is required.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         <see cref="Capacity"/> is the number of items that the <see cref="CircularArrayQueue{T}"/> can store before
        ///         resizing is required, whereas <see cref="ICollectionValue{T}.Count"/> is the number of items that are actually
        ///         in the
        ///         <see cref="CircularArrayQueue{T}"/>.
        ///     </para>
        ///     <para>
        ///         If the capacity is significantly larger than the count and you want to reduce the memory used by the
        ///         <see cref="CircularArrayQueue{T}"/>, you can decrease capacity by calling the <see cref="TrimExcess"/> method
        ///         or by setting the <see cref="Capacity"/> property explicitly to a lower value. When the value of
        ///         <see cref="Capacity"/> is set explicitly, the internal data structure is also reallocated to accommodate the
        ///         specified capacity, and all the items are copied.
        ///     </para>
        /// </remarks>
        public virtual int Capacity
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

                    // Store Count to avoid reading it after having copied the items, as Count has a postcondition that enumerates the collection
                    var count = Count;

                    var array = new T[value];
                    CopyTo(array, 0);
                    _items = array;

                    _front = 0;
                    _back = count;
                }
                else {
                    _items = EmptyArray;
                }
            }
        }

        public override Speed CountSpeed => Constant;

        public virtual EnumerationDirection Direction
        {
            get { throw new NotImplementedException(); }
        }

        public override EventTypes ListenableEvents => All;

        public virtual T this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Methods

        public virtual IDirectedCollectionValue<T> Backwards()
        {
            throw new NotImplementedException();
        }

        public override T Choose() => _items[_front];

        public override void CopyTo(T[] array, int arrayIndex)
        {
            if (IsEmpty) {
                return;
            }

            var count = _front < _back ? Count : Capacity - _front;
            Array.Copy(_items, _front, array, arrayIndex, count);

            // TODO: Test
            if (_front > _back) {
                Array.Copy(_items, 0, array, arrayIndex + count, _back);
            }
        }

        public virtual T Dequeue()
        {
            throw new NotImplementedException();
        }

        public virtual void Enqueue(T item)
        {
            UpdateVersion();

            EnsureCapacity(Count + 1);

            ++Count;
            _items[_back++] = item;

            if (_back == Capacity) {
                _back = 0;
            }

            RaiseForEnqueue(item);
        }

        public override SCG.IEnumerator<T> GetEnumerator()
        {
            if (IsEmpty) {
                yield break;
            }

            var version = Version;
            var index = _front;
            var end = _front < _back ? _back : Capacity;

            while (CheckVersion(version) & index < end) {
                yield return _items[index++];
            }

            if (_front > _back) {
                index = 0;
                while (CheckVersion(version) & index < _back) {
                    yield return _items[index++];
                }
            }
        }

        #endregion

        #region Private Members

        private void EnsureCapacity(int requiredCapacity)
        {
            #region Code Contracts

            Requires(requiredCapacity >= 0);

            Requires(requiredCapacity >= Count);

            Ensures(Capacity >= requiredCapacity);

            Ensures(MinArrayLength <= Capacity && Capacity <= MaxArrayLength);

            #endregion

            if (Capacity >= requiredCapacity) {
                return;
            }

            var capacity = Capacity * 2;

            if ((uint) capacity > MaxArrayLength) {
                capacity = MaxArrayLength;
            }
            else if (capacity<MinArrayLength) {
                capacity = MinArrayLength;
            }

            if (capacity<requiredCapacity) {
                capacity = requiredCapacity;
            }

            Capacity = capacity;
        }


        #endregion
    }
}