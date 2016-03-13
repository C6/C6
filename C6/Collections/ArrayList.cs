// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;

using SCG = System.Collections.Generic;


namespace C6
{
    public class ArrayList<T> : IExtensible<T>
    {
        private const int MinimumCapacity = 8;

        #region Fields

        private T[] _items;

        private event EventHandler _collectionChanged;
        private event EventHandler<ClearedEventArgs> _collectionCleared;
        private event EventHandler<ItemAtEventArgs<T>> _itemInserted, _itemRemovedAt;
        private event EventHandler<ItemCountEventArgs<T>> _itemsAdded, _itemsRemoved;

        #endregion

        #region Code Contracts

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // ReSharper disable InvocationIsSkipped

            // Array is non-null
            Invariant(_items != null);

            // All items must be non-null if collection disallows null values
            Invariant(AllowsNull || ForAll(this, item => item != null));

            // The unused part of the array contains default values
            Invariant(ForAll(Count, _items.Length, i => Equals(_items[i], default(T))));

            // Equality comparer is non-null
            Invariant(EqualityComparer != null);

            // ReSharper restore InvocationIsSkipped
        }

        #endregion

        #region Constructors

        public ArrayList(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(items != null, ArgumentMustBeNonNull);

            // All items must be non-null if collection disallows null values
            Requires(allowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            #endregion

            // TODO: Check for null items when copying?
            _items = items.ToArray();
            Count = _items.Length;

            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;

            AllowsNull = allowsNull;
        }

        public ArrayList(int capacity = MinimumCapacity, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
        {
            #region Code Contracts

            // Argument must be non-negative
            Requires(0 <= capacity, ArgumentMustBeNonNegative);

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            #endregion

            _items = new T[capacity];

            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;

            AllowsNull = allowsNull;
        }

        #endregion

        #region Properties

        public EventTypes ActiveEvents { get; private set; }

        public bool AllowsDuplicates => true;

        public bool AllowsNull { get; }

        public int Count { get; private set; }

        public Speed CountSpeed => Speed.Constant;

        public bool DuplicatesByCounting => false;

        public SCG.IEqualityComparer<T> EqualityComparer { get; }

        public bool IsEmpty => Count == 0;

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public EventTypes ListenableEvents => All;

        #endregion

        #region Public Methods

        public bool Add(T item)
        {
            #region Code Contracts

            // Item is added to the end
            Ensures(this.Last().Equals(item));

            #endregion

            // TODO: Increment stamp

            InsertPrivate(Count, item);

            // TODO: Raise events

            _itemsAdded?.Invoke(this, new ItemCountEventArgs<T>(item, 1));
            _collectionChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public void AddAll(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public T Choose() => _items[Count - 1];

        public void CopyTo(T[] array, int arrayIndex)
            => Array.Copy(_items, 0, array, arrayIndex, Count);

        public SCG.IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++) {
                yield return _items[i];
            }
        }

        public T[] ToArray()
        {
            var array = new T[Count];
            Array.Copy(_items, array, Count);
            return array;
        }

        #endregion

        #region Events

        public event EventHandler CollectionChanged
        {
            add
            {
                _collectionChanged += value;
                ActiveEvents |= Changed;
            }
            remove
            {
                _collectionChanged -= value;
                if (_collectionChanged == null) {
                    ActiveEvents &= ~Changed;
                }
            }
        }

        public event EventHandler<ClearedEventArgs> CollectionCleared
        {
            add
            {
                _collectionCleared += value;
                ActiveEvents |= Cleared;
            }
            remove
            {
                _collectionCleared -= value;
                if (_collectionCleared == null) {
                    ActiveEvents &= ~Cleared;
                }
            }
        }

        public event EventHandler<ItemAtEventArgs<T>> ItemInserted
        {
            add
            {
                _itemInserted += value;
                ActiveEvents |= Inserted;
            }
            remove
            {
                _itemInserted -= value;
                if (_itemInserted == null) {
                    ActiveEvents &= ~Inserted;
                }
            }
        }

        public event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt
        {
            add
            {
                _itemRemovedAt += value;
                ActiveEvents |= RemovedAt;
            }
            remove
            {
                _itemRemovedAt -= value;
                if (_itemRemovedAt == null) {
                    ActiveEvents &= ~RemovedAt;
                }
            }
        }

        public event EventHandler<ItemCountEventArgs<T>> ItemsAdded
        {
            add
            {
                _itemsAdded += value;
                ActiveEvents |= Added;
            }
            remove
            {
                _itemsAdded -= value;
                if (_itemsAdded == null) {
                    ActiveEvents &= ~Added;
                }
            }
        }

        public event EventHandler<ItemCountEventArgs<T>> ItemsRemoved
        {
            add
            {
                _itemsRemoved += value;
                ActiveEvents |= Removed;
            }
            remove
            {
                _itemsRemoved -= value;
                if (_itemsRemoved == null) {
                    ActiveEvents &= ~Removed;
                }
            }
        }

        #endregion

        #region Explicit Implementations

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private Members

        // TODO: Rename?
        private void InsertPrivate(int index, T item)
        {
            #region Code Contracts

            // Argument must be within bounds
            Requires(0 <= index, ArgumentMustBeWithinBounds);
            Requires(index <= Count, ArgumentMustBeWithinBounds);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);

            #endregion

            if (_items.Length == Count) {
                Resize();
            }

            // Move items one index to the right
            if (index < Count) {
                Array.Copy(_items, index, _items, index + 1, Count - index);
            }

            _items[index] = item;
            Count += 1;
        }

        private void Resize()
        {
            var size = Math.Max(Count * 2, MinimumCapacity);
            Resize(size);
        }

        private void Resize(int size)
        {
            #region Code Contracts

            // Array must fit the items in the collection
            Requires(size >= Count);

            #endregion

            // TODO: Ensure size is a power of two
            // TODO: Use empty array

            var array = new T[size];
            Array.Copy(_items, array, Count);
            _items = array;
        }

        #endregion
    }
}