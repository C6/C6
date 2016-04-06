// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using C6.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;
using static C6.ExceptionMessages;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6
{
    [Serializable]
    public class ArrayList<T> : ICollection<T>
    {
        #region Fields

        private static readonly T[] EmptyArray = new T[0];

        private const int MinArrayLength = 0x00000004;
        private const int MaxArrayLength = 0x7FEFFFFF;

        private T[] _items;

        private int _version;

        private int _unsequencedHashCodeVersion = -1;
        private int _unsequencedHashCode;

        private event EventHandler _collectionChanged;
        private event EventHandler<ClearedEventArgs> _collectionCleared;
        private event EventHandler<ItemAtEventArgs<T>> _itemInserted , _itemRemovedAt;
        private event EventHandler<ItemCountEventArgs<T>> _itemsAdded , _itemsRemoved;

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
            Invariant(ForAll(Count, Capacity, i => Equals(_items[i], default(T))));

            // Equality comparer is non-null
            Invariant(EqualityComparer != null);

            // ReSharper restore InvocationIsSkipped
        }

        #endregion

        #region Constructors

        public ArrayList(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
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

            _items = items.ToArray();
            Count = Capacity;

            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;

            AllowsNull = allowsNull;
        }

        public ArrayList(int capacity = 0, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
        {
            #region Code Contracts

            // ReSharper disable InvocationIsSkipped

            // Argument must be non-negative
            Requires(0 <= capacity, ArgumentMustBeNonNegative);

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            // ReSharper restore InvocationIsSkipped

            #endregion

            _items = capacity > 0 ? new T[capacity] : EmptyArray;

            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;

            AllowsNull = allowsNull;
        }

        #endregion

        #region Properties

        public EventTypes ActiveEvents { get; private set; }

        public bool AllowsDuplicates => true;

        public bool AllowsNull { get; }

        public Speed ContainsSpeed => Speed.Linear;

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
            UpdateVersion();

            InsertPrivate(Count, item);

            RaiseForAdd(item);
            return true;
        }

        // TODO: Use InsertAll?
        public void AddAll(SCG.IEnumerable<T> items)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            // A bad enumerator will throw an exception here
            var array = items.ToArray();

            var length = array.Length;

            if (length == 0) {
                return;
            }

            // Only update version if items are actually added
            UpdateVersion();

            EnsureCapacity(Count + length);

            Array.Copy(array, 0, _items, Count, length);
            Count += length;

            RaiseForAddAll(array);
        }

        public T Choose() => _items[Count - 1];

        public void Clear()
        {
            if (IsEmpty) {
                return;
            }

            // Only update version if the collection is actually cleared
            UpdateVersion();

            var oldCount = Count;

            _items = EmptyArray;
            Count = 0;

            RaiseForClear(oldCount);
        }

        public bool Contains(T item) => IndexOfPrivate(item) >= 0;

        public bool ContainsAll(SCG.IEnumerable<T> items)
        {
            // TODO: Replace ArrayList<T> with more efficient data structure like HashBag<T>
            // TODO: use aux hash bag to obtain linear time procedure (old comment)
            var itemsToContain = new ArrayList<T>(items, EqualityComparer, AllowsNull);

            if (itemsToContain.IsEmpty) {
                return true;
            }

            foreach (var item in this) {
                if (itemsToContain.Remove(item) && itemsToContain.IsEmpty) {
                    return true;
                }
            }
            return false;
        }

        // TODO: Test performance?
        public int ContainsCount(T item) => _items.Take(Count).Count(x => Equals(x, item));

        public void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, Count);

        public bool Find(ref T item)
        {
            var index = IndexOfPrivate(item);

            if (index >= 0) {
                item = _items[index];
                return true;
            }

            return false;
        }

        public bool FindOrAdd(ref T item)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            if (Find(ref item)) {
                return true;
            }

            // Let Add handle version update and events
            Add(item);

            return false;
        }

        public SCG.IEnumerator<T> GetEnumerator()
        {
            // Cache count to ensure that clearing while enumerating throws an exception
            var count = Count;
            var version = _version;

            for (var i = 0; i < count; i++) {
                CheckVersion(version);
                yield return _items[i];
            }
        }

        public int GetUnsequencedHashCode()
        {
            if (_unsequencedHashCodeVersion != _version) {
                _unsequencedHashCodeVersion = _version;
                _unsequencedHashCode = this.GetUnsequencedHashCode(EqualityComparer);
            }

            return _unsequencedHashCode;
        }

        public ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            T removedItem;
            return Remove(item, out removedItem);
        }

        public bool Remove(T item, out T removedItem)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            // TODO: Work based on IsFifo
            var index = IndexOfPrivate(item);

            if (index >= 0) {
                UpdateVersion();
                removedItem = RemoveAt(index);
                RaiseForRemove(removedItem);
                return true;
            }

            removedItem = default(T);
            return false;
        }

        public bool RemoveAll(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public void RetainAll(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public T[] ToArray()
        {
            var array = new T[Count];
            Array.Copy(_items, array, Count);
            return array;
        }

        public ICollectionValue<T> UniqueItems()
        {
            throw new NotImplementedException();
        }

        public bool UnsequencedEquals(ICollection<T> otherCollection)
        {
            throw new NotImplementedException();
        }

        public bool Update(T item)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            T oldItem;
            return Update(item, out oldItem);
        }

        public bool Update(T item, out T oldItem)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            var index = IndexOfPrivate(item);

            if (index >= 0) {
                // Only update version if item is actually updated
                UpdateVersion();

                oldItem = _items[index];
                _items[index] = item;

                RaiseForUpdate(item, oldItem);

                return true;
            }

            oldItem = default(T);
            return false;
        }

        public bool UpdateOrAdd(T item)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrAdd(T item, out T oldItem)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        public event EventHandler CollectionChanged
        {
            add {
                _collectionChanged += value;
                ActiveEvents |= Changed;
            }
            remove {
                _collectionChanged -= value;
                if (_collectionChanged == null) {
                    ActiveEvents &= ~Changed;
                }
            }
        }

        public event EventHandler<ClearedEventArgs> CollectionCleared
        {
            add {
                _collectionCleared += value;
                ActiveEvents |= Cleared;
            }
            remove {
                _collectionCleared -= value;
                if (_collectionCleared == null) {
                    ActiveEvents &= ~Cleared;
                }
            }
        }

        public event EventHandler<ItemAtEventArgs<T>> ItemInserted
        {
            add {
                _itemInserted += value;
                ActiveEvents |= Inserted;
            }
            remove {
                _itemInserted -= value;
                if (_itemInserted == null) {
                    ActiveEvents &= ~Inserted;
                }
            }
        }

        public event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt
        {
            add {
                _itemRemovedAt += value;
                ActiveEvents |= RemovedAt;
            }
            remove {
                _itemRemovedAt -= value;
                if (_itemRemovedAt == null) {
                    ActiveEvents &= ~RemovedAt;
                }
            }
        }

        public event EventHandler<ItemCountEventArgs<T>> ItemsAdded
        {
            add {
                _itemsAdded += value;
                ActiveEvents |= Added;
            }
            remove {
                _itemsAdded -= value;
                if (_itemsAdded == null) {
                    ActiveEvents &= ~Added;
                }
            }
        }

        public event EventHandler<ItemCountEventArgs<T>> ItemsRemoved
        {
            add {
                _itemsRemoved += value;
                ActiveEvents |= Removed;
            }
            remove {
                _itemsRemoved -= value;
                if (_itemsRemoved == null) {
                    ActiveEvents &= ~Removed;
                }
            }
        }

        #endregion

        #region Explicit Implementations

        void SCG.ICollection<T>.Add(T item) => Add(item);

        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Private Members

        private int Capacity
        {
            get { return _items.Length; }
            set {
                #region Code Contracts

                // Capacity must be at least as big as the number of items
                Requires(value >= Count);

                // Capacity is at least as big as the number of items
                Ensures(value >= Count);

                #endregion

                if (value == _items.Length) {
                    return;
                }
                if (value > 0) {
                    Array.Resize(ref _items, value);
                }
                else {
                    _items = EmptyArray;
                }
            }
        }

        private void EnsureCapacity(int requiredCapacity)
        {
            #region Code Contracts

            Ensures(Capacity >= requiredCapacity);

            #endregion

            if (Capacity >= requiredCapacity) {
                return;
            }

            var capacity = IsEmpty ? MinArrayLength : Capacity*2;

            if ((uint) capacity > MaxArrayLength) {
                capacity = MaxArrayLength;
            }
            if (capacity < requiredCapacity) {
                capacity = requiredCapacity;
            }

            Capacity = capacity;
        }

        [Pure]
        private bool Equals(T x, T y) => EqualityComparer.Equals(x, y);

        // TODO: Inline in IndexOf
        // TODO: Make version that works with IsFifo
        [Pure]
        private int IndexOfPrivate(T item)
        {
            #region Code Contracts

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null);


            // TODO: Add contract to IList<T>.IndexOf
            // Result is a valid index
            Ensures(Contains(item)
                ? 0 <= Result<int>() && Result<int>() < Count
                : ~Result<int>() == Count);

            // Item at index is the first equal to item
            Ensures(Result<int>() < 0 || !this.Take(Result<int>()).Contains(item, EqualityComparer) && EqualityComparer.Equals(item, this.Skip(Result<int>()).First()));

            #endregion

            for (var i = 0; i < Count; i++) {
                if (Equals(item, _items[i])) {
                    return i;
                }
            }

            return ~Count;
        }

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

            if (Capacity == Count) {
                EnsureCapacity(Count + 1);
            }

            // Move items one to the right
            if (index < Count) {
                Array.Copy(_items, index, _items, index + 1, Count - index);
            }

            _items[index] = item;
            Count++;
        }

        private T RemoveAt(int index)
        {
            var item = _items[index];

            if (--Count > index) {
                Array.Copy(_items, index + 1, _items, index, Count - index);
            }

            _items[Count] = default(T);

            return item;
        }

        private void UpdateVersion() => _version++;

        private void CheckVersion(int version)
        {
            if (version != _version) {
                // See https://msdn.microsoft.com/library/system.collections.ienumerator.movenext.aspx
                throw new InvalidOperationException(CollectionModified);
            }
        }

        #region Event Helpers

        #region Invoking Methods

        private void OnCollectionChanged()
            => _collectionChanged?.Invoke(this, EventArgs.Empty);

        private void OnCollectionCleared(bool full, int count, int? start = null)
            => _collectionCleared?.Invoke(this, new ClearedEventArgs(full, count, start));

        private void OnItemsAdded(T item, int count)
            => _itemsAdded?.Invoke(this, new ItemCountEventArgs<T>(item, count));

        private void OnItemsRemoved(T item, int count)
            => _itemsRemoved?.Invoke(this, new ItemCountEventArgs<T>(item, count));

        private void OnItemInserted(T item, int index)
            => _itemInserted?.Invoke(this, new ItemAtEventArgs<T>(item, index));

        private void OnItemRemovedAt(T item, int index)
            => _itemRemovedAt?.Invoke(this, new ItemAtEventArgs<T>(item, index));

        #endregion

        #region Method-Specific Helpers

        private void RaiseForAdd(T item)
        {
            OnItemsAdded(item, 1);
            OnCollectionChanged();
        }

        private void RaiseForAddAll(SCG.IEnumerable<T> items)
        {
            if (ActiveEvents.HasFlag(Added)) {
                foreach (var item in items) {
                    OnItemsAdded(item, 1);
                }
            }
            OnCollectionChanged();
        }

        private void RaiseForClear(int count)
        {
            OnCollectionCleared(true, count);
            OnCollectionChanged();
        }

        private void RaiseForRemove(T item)
        {
            OnItemsRemoved(item, 1);
            OnCollectionChanged();
        }

        private void RaiseForUpdate(T item, T oldItem)
        {
            OnItemsRemoved(oldItem, 1);
            OnItemsAdded(item, 1);
            OnCollectionChanged();
        }

        #endregion

        #endregion

        #endregion
    }
}