// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

using C6.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Collections.ExceptionMessages;
using static C6.Contracts.ContractMessage;
using static C6.EventTypes;
using static C6.Speed;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6.Collections
{
    /// <summary>
    ///     Represents a generic list whose items that can be accessed efficiently by index.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the collection.
    /// </typeparam>
    /// <remarks>
    ///     <para>
    ///         <see cref="ArrayList{T}"/> uses an internal array whose size is dynamically increased as required. Item access
    ///         by index takes constant time. Items are added to the end of the list in amortized constant time, but insertion
    ///         of one or more items takes time proportional to the number of items that must be moved to make room for the new
    ///         item(s). The collection allows duplicates and stores them explicitly.
    ///     </para>
    ///     <para>
    ///         Changing the state of an item while it is stored in an <see cref="ArrayList{T}"/> does not affect the
    ///         <see cref="ArrayList{T}"/>. It might, however, affect any accessible <see cref="ICollectionValue{T}"/> returned
    ///         from the collection, if that <see cref="ICollectionValue{T}"/> relies on the state of the item, e.g. when the
    ///         change affects the item's hash code while unique items are enumerated.
    ///     </para>
    /// </remarks>
    [Serializable]
    [DebuggerTypeProxy(typeof(CollectionValueDebugView<>))]
    public class ArrayList<T> : CollectionValueBase<T>, IList<T>, IStack<T>
    {
        #region Fields

        private static readonly T[] EmptyArray = new T[0];

        private const int MinArrayLength = 0x00000004;
        private const int MaxArrayLength = 0x7FEFFFFF;

        private T[] _items;

        private int _version, _sequencedHashCodeVersion = -1, _unsequencedHashCodeVersion = -1;
        private int _sequencedHashCode, _unsequencedHashCode;

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

            // Count is not bigger than the capacity
            Invariant(Count <= Capacity);

            // All items must be non-null if collection disallows null values
            Invariant(AllowsNull || ForAll(this, item => item != null));

            // The unused part of the array contains default values
            Invariant(ForAll(Count, Capacity, i => Equals(_items[i], default(T))));

            // Equality comparer is non-null
            Invariant(EqualityComparer != null);

            // Empty array is always empty
            Invariant(EmptyArray.IsEmpty());

            // ReSharper restore InvocationIsSkipped
        }

        #endregion

        #region Constructors

        public ArrayList(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) : base(allowsNull)
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

            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;

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
                _items = EmptyArray;
                AddRange(items);
            }
        }

        public ArrayList(int capacity = 0, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) : base(allowsNull)
        {
            #region Code Contracts

            // ReSharper disable InvocationIsSkipped

            // Argument must be non-negative
            Requires(0 <= capacity, ArgumentMustBeNonNegative);

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull, AllowsNullMustBeFalseForValueTypes);

            // ReSharper restore InvocationIsSkipped

            #endregion

            Capacity = capacity;
            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;
        }

        #endregion

        #region Properties

        public virtual EventTypes ActiveEvents { get; private set; }

        public virtual bool AllowsDuplicates => true;

        /// <summary>
        ///     Gets or sets the total number of items the internal data structure can hold without resizing.
        /// </summary>
        /// <value>
        ///     The number of items that the <see cref="ArrayList{T}"/> can contain before resizing is required.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         <see cref="Capacity"/> is the number of items that the <see cref="ArrayList{T}"/> can store before resizing is
        ///         required, whereas <see cref="Count"/> is the number of items that are actually in the
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

                    Array.Resize(ref _items, value);
                }
                else {
                    _items = EmptyArray;
                }
            }
        }

        public virtual Speed ContainsSpeed => Linear;

        public override Speed CountSpeed => Constant;

        public virtual EnumerationDirection Direction => EnumerationDirection.Forwards;

        public virtual bool DuplicatesByCounting => false;

        public virtual SCG.IEqualityComparer<T> EqualityComparer { get; }

        public virtual T First => _items[0];

        public virtual Speed IndexingSpeed => Constant;

        public virtual bool IsFixedSize => false;

        public virtual bool IsReadOnly => false;

        public virtual T Last => _items[Count - 1];

        public virtual EventTypes ListenableEvents => All;

        public virtual T this[int index]
        {
            get { return _items[index]; }
            set {
                #region Code Contracts

                // The version is updated
                Ensures(_version != OldValue(_version));

                #endregion

                UpdateVersion();
                var oldItem = _items[index];
                _items[index] = value;
                RaiseForIndexSetter(oldItem, value, index);
            }
        }

        #endregion

        #region Public Methods

        public virtual bool Add(T item)
        {
            #region Code Contracts

            // The version is updated
            Ensures(_version != OldValue(_version));

            #endregion

            InsertPrivate(Count, item);
            RaiseForAdd(item);
            return true;
        }

        public virtual bool AddRange(SCG.IEnumerable<T> items)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            // TODO: Handle ICollectionValue<T> and ICollection<T>

            // TODO: Avoid creating an array? Requires a lot of extra code, since we need to properly handle items already added from a bad enumerable
            // A bad enumerator will throw an exception here
            var array = items.ToArray();

            if (array.IsEmpty()) {
                return false;
            }

            InsertRangePrivate(Count, array);
            RaiseForAddRange(array);
            return true;
        }

        // Only creates one Range instead of two as with GetIndexRange(0, Count).Backwards()
        public virtual IDirectedCollectionValue<T> Backwards() => new Range(this, Count - 1, Count, EnumerationDirection.Backwards);

        public override T Choose() => _items[Count - 1];

        public virtual void Clear()
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            if (IsEmpty) {
                return;
            }

            // Only update version if the collection is actually cleared
            UpdateVersion();

            var oldCount = Count;
            ClearPrivate();
            RaiseForClear(oldCount);
        }

        public virtual bool Contains(T item) => IndexOf(item) >= 0;

        public virtual bool ContainsRange(SCG.IEnumerable<T> items)
        {
            if (items.IsEmpty()) {
                return true;
            }

            if (IsEmpty) {
                return false;
            }

            // TODO: Replace ArrayList<T> with more efficient data structure like HashBag<T>
            var itemsToContain = new ArrayList<T>(items, EqualityComparer, AllowsNull);

            if (itemsToContain.Count > Count) {
                return false;
            }

            return this.Any(item => itemsToContain.Remove(item) && itemsToContain.IsEmpty);
        }

        public override void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, Count);

        // Explicitly check against null to avoid using the (slower) equality comparer
        public virtual int CountDuplicates(T item) => item == null ? this.Count(x => x == null) : this.Count(x => Equals(x, item));

        public virtual bool Find(ref T item)
        {
            var index = IndexOf(item);

            if (index >= 0) {
                item = _items[index];
                return true;
            }

            return false;
        }
        
        public virtual ICollectionValue<T> FindDuplicates(T item) => null;

        public virtual bool FindOrAdd(ref T item)
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

        public override SCG.IEnumerator<T> GetEnumerator()
        {
            #region Code Contracts

            // The version is not updated
            Ensures(_version == OldValue(_version));

            #endregion

            var version = _version;
            // Check version at each call to MoveNext() to ensure an exception is thrown even when the enumerator was really finished
            for (var i = 0; CheckVersion(version) & i < Count; i++) {
                yield return _items[i];
            }
        }

        public virtual IDirectedCollectionValue<T> GetIndexRange(int startIndex, int count) => new Range(this, startIndex, count, EnumerationDirection.Forwards);

        // TODO: Update hash code when items are added, if the hash code version is not equal to -1
        public virtual int GetSequencedHashCode()
        {
            if (_sequencedHashCodeVersion != _version) {
                _sequencedHashCodeVersion = _version;
                _sequencedHashCode = this.GetSequencedHashCode(EqualityComparer);
            }

            return _sequencedHashCode;
        }

        // TODO: Update hash code when items are added, if the hash code version is not equal to -1
        public virtual int GetUnsequencedHashCode()
        {
            if (_unsequencedHashCodeVersion != _version) {
                _unsequencedHashCodeVersion = _version;
                _unsequencedHashCode = this.GetUnsequencedHashCode(EqualityComparer);
            }

            return _unsequencedHashCode;
        }

        [Pure]
        public virtual int IndexOf(T item)
        {
            #region Code Contracts

            // TODO: Add contract to IList<T>.IndexOf
            // Result is a valid index
            Ensures(Contains(item)
                ? 0 <= Result<int>() && Result<int>() < Count
                : ~Result<int>() == Count);

            // Item at index is the first equal to item
            Ensures(Result<int>() < 0 || !this.Take(Result<int>()).Contains(item, EqualityComparer) && EqualityComparer.Equals(item, this.ElementAt(Result<int>())));

            #endregion

            if (item == null) {
                for (var i = 0; i < Count; i++) {
                    // Explicitly check against null to avoid using the (slower) equality comparer
                    if (_items[i] == null) {
                        return i;
                    }
                }
            }
            else {
                for (var i = 0; i < Count; i++) {
                    if (Equals(item, _items[i])) {
                        return i;
                    }
                }
            }

            return ~Count;
        }

        public virtual void Insert(int index, T item)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            InsertPrivate(index, item);
            RaiseForInsert(index, item);
        }

        public virtual void InsertFirst(T item) => Insert(0, item);

        public virtual void InsertLast(T item) => Insert(Count, item);

        public virtual void InsertRange(int index, SCG.IEnumerable<T> items)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            // TODO: Handle ICollectionValue<T> and ICollection<T>

            // TODO: Avoid creating an array? Requires a lot of extra code, since we need to properly handle items already added from a bad enumerable
            // A bad enumerator will throw an exception here
            var array = items.ToArray();

            if (array.IsEmpty()) {
                return;
            }

            InsertRangePrivate(index, array);
            RaiseForInsertRange(index, array);
        }

        public virtual bool IsSorted() => IsSorted(SCG.Comparer<T>.Default.Compare);

        public virtual bool IsSorted(Comparison<T> comparison)
        {
            // TODO: Can we check that comparison doesn't alter the collection?
            for (var i = 1; i < Count; i++) {
                if (comparison(_items[i - 1], _items[i]) > 0) {
                    return false;
                }
            }

            return true;
        }

        public virtual bool IsSorted(SCG.IComparer<T> comparer) => IsSorted((comparer ?? SCG.Comparer<T>.Default).Compare);

        // TODO: Defer execution
        public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
        {
            throw new NotImplementedException();
        }

        public virtual int LastIndexOf(T item)
        {
            #region Code Contracts

            // TODO: Add contract to IList<T>.LastIndexOf
            // Result is a valid index
            Ensures(Contains(item)
                ? 0 <= Result<int>() && Result<int>() < Count
                : ~Result<int>() == Count);

            // Item at index is the first equal to item
            Ensures(Result<int>() < 0 || !this.Skip(Result<int>() + 1).Contains(item, EqualityComparer) && EqualityComparer.Equals(item, this.ElementAt(Result<int>())));

            #endregion

            if (item == null) {
                for (var i = Count - 1; i >= 0; i--) {
                    // Explicitly check against null to avoid using the (slower) equality comparer
                    if (_items[i] == null) {
                        return i;
                    }
                }
            }
            else {
                for (var i = Count - 1; i >= 0; i--) {
                    if (Equals(item, _items[i])) {
                        return i;
                    }
                }
            }

            return ~Count;
        }

        public virtual T Pop() => RemoveLast();

        public virtual void Push(T item) => InsertLast(item);

        public virtual bool Remove(T item)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            T removedItem;
            return Remove(item, out removedItem);
        }

        public virtual bool Remove(T item, out T removedItem)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            // Remove last instance of item, since this moves the fewest items
            var index = LastIndexOf(item);

            if (index >= 0) {
                removedItem = RemoveAtPrivate(index);
                RaiseForRemove(removedItem);
                return true;
            }

            removedItem = default(T);
            return false;
        }

        public virtual T RemoveAt(int index)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            var item = RemoveAtPrivate(index);
            RaiseForRemovedAt(item, index);
            return item;
        }

        // Explicitly check against null to avoid using the (slower) equality comparer
        public virtual bool RemoveDuplicates(T item) => item == null ? RemoveAllWhere(x => x == null) : RemoveAllWhere(x => Equals(item, x));

        public virtual T RemoveFirst() => RemoveAt(0);

        public virtual void RemoveIndexRange(int startIndex, int count)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            if (count == 0) {
                return;
            }

            // Only update version if item is actually removed
            UpdateVersion();

            if ((Count -= count) > startIndex) {
                Array.Copy(_items, startIndex + count, _items, startIndex, Count - startIndex);
            }
            Array.Clear(_items, Count, count);

            RaiseForRemoveIndexRange(startIndex, count);
        }

        public virtual T RemoveLast() => RemoveAt(Count - 1);

        public virtual bool RemoveRange(SCG.IEnumerable<T> items)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            if (IsEmpty || items.IsEmpty()) {
                return false;
            }

            // TODO: Replace ArrayList<T> with more efficient data structure like HashBag<T>
            var itemsToRemove = new ArrayList<T>(items, EqualityComparer, AllowsNull);
            return RemoveAllWhere(item => itemsToRemove.Remove(item));
        }

        public virtual bool RetainRange(SCG.IEnumerable<T> items)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            if (IsEmpty) {
                return false;
            }

            if (items.IsEmpty()) {
                // Optimize call, if no items should be retained
                UpdateVersion();
                var itemsRemoved = _items;
                ClearPrivate();
                RaiseForRemoveAllWhere(itemsRemoved);
                return true;
            }

            // TODO: Replace ArrayList<T> with more efficient data structure like HashBag<T>
            var itemsToRemove = new ArrayList<T>(items, EqualityComparer, AllowsNull);
            return RemoveAllWhere(item => !itemsToRemove.Remove(item));
        }

        public virtual void Reverse()
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            if (Count <= 1) {
                return;
            }

            // Only update version if the collection is actually reversed
            UpdateVersion();

            Array.Reverse(_items, 0, Count);
            RaiseForReverse();
        }

        public virtual bool SequencedEquals(ISequenced<T> otherCollection) => this.SequencedEquals(otherCollection, EqualityComparer);

        public virtual void Shuffle() => Shuffle(new Random());

        public virtual void Shuffle(Random random)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            if (Count <= 1) {
                return;
            }

            // Only update version if the collection is shuffled
            UpdateVersion();

            _items.Shuffle(0, Count, random);
            RaiseForShuffle();
        }

        public virtual void Sort() => Sort((SCG.IComparer<T>) null);

        // TODO: It seems that Array.Sort(T[], Comparison<T>) is the only method that takes an Comparison<T>, not allowing us to set bounds on the sorting
        public virtual void Sort(Comparison<T> comparison) => Sort(comparison.ToComparer());

        public virtual void Sort(SCG.IComparer<T> comparer)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            if (comparer == null) {
                comparer = SCG.Comparer<T>.Default;
            }

            if (IsSorted(comparer)) {
                return;
            }

            // Only update version if the collection is actually sorted
            UpdateVersion();
            Array.Sort(_items, 0, Count, comparer);
            RaiseForSort();
        }

        public virtual T[] ToArray()
        {
            var array = new T[Count];
            Array.Copy(_items, array, Count);
            return array;
        }

        public override string ToString() => ToString(null, null);

        public virtual string ToString(string format, IFormatProvider formatProvider) => Showing.ShowString(this, format, formatProvider);

        /// <summary>
        ///     Sets the capacity to the actual number of items in the <see cref="ArrayList{T}"/>, if that number is less than a
        ///     threshold value.
        /// </summary>
        /// <remarks>
        ///     This method can be used to minimize a collection's memory overhead if no new items will be added to the collection.
        ///     The cost of reallocating and copying a large <see cref="ArrayList{T}"/>
        ///     can be considerable, however, so the <see cref="TrimExcess"/> method does nothing if the list is at more than 90
        ///     percent of capacity. This avoids incurring a large reallocation cost for a relatively small gain. The current
        ///     threshold of 90 percent might change in future releases.
        /// </remarks>
        public virtual void TrimExcess()
        {
            if (Capacity * 0.9 <= Count) {
                return;
            }
            Capacity = Count;
        }

        public virtual ICollectionValue<T> UniqueItems() => new ItemSet(this);

        public virtual bool UnsequencedEquals(ICollection<T> otherCollection) => this.UnsequencedEquals(otherCollection, EqualityComparer);

        public virtual bool Update(T item)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            T oldItem;
            return Update(item, out oldItem);
        }

        public virtual bool Update(T item, out T oldItem)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || _version != OldValue(_version));

            #endregion

            var index = IndexOf(item);

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

        public virtual bool UpdateOrAdd(T item)
        {
            #region Code Contracts

            // The version is updated
            Ensures(_version != OldValue(_version));

            #endregion

            T oldItem;
            return UpdateOrAdd(item, out oldItem);
        }

        public virtual bool UpdateOrAdd(T item, out T oldItem)
        {
            #region Code Contracts

            // The version is updated
            Ensures(_version != OldValue(_version));

            #endregion

            if (Update(item, out oldItem)) {
                return true;
            }

            Add(item);
            return false;
        }

        #endregion

        #region Events

        public virtual event EventHandler CollectionChanged
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

        public virtual event EventHandler<ClearedEventArgs> CollectionCleared
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

        public virtual event EventHandler<ItemAtEventArgs<T>> ItemInserted
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

        public virtual event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt
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

        public virtual event EventHandler<ItemCountEventArgs<T>> ItemsAdded
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

        public virtual event EventHandler<ItemCountEventArgs<T>> ItemsRemoved
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

        bool SC.ICollection.IsSynchronized => false;

        object SC.ICollection.SyncRoot { get; } = new object();

        object SC.IList.this[int index]
        {
            get { return this[index]; }
            set {
                try {
                    this[index] = (T) value;
                }
                catch (InvalidCastException) {
                    throw new ArgumentException($"The value \"{value}\" is not of type \"{typeof(T)}\" and cannot be used in this generic collection.{Environment.NewLine}Parameter name: {nameof(value)}");
                }
            }
        }

        int SC.IList.Add(object value)
        {
            try {
                return Add((T) value) ? Count - 1 : -1;
            }
            catch (InvalidCastException) {
                throw new ArgumentException($"The value \"{value}\" is not of type \"{typeof(T)}\" and cannot be used in this generic collection.{Environment.NewLine}Parameter name: {nameof(value)}");
            }
        }

        void SCG.ICollection<T>.Add(T item) => Add(item);

        bool SC.IList.Contains(object value) => IsCompatibleObject(value) && Contains((T) value);

        void SC.ICollection.CopyTo(Array array, int index)
        {
            try {
                Array.Copy(_items, 0, array, index, Count);
            }
            catch (ArrayTypeMismatchException) {
                throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
            }
        }

        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();

        int SC.IList.IndexOf(object value) => IsCompatibleObject(value) ? Math.Max(-1, IndexOf((T) value)) : -1;

        // Explicit implementation is needed, since C6.IList<T>.IndexOf(T) breaks SCG.IList<T>.IndexOf(T)'s precondition: Result<T>() >= -1
        int SCG.IList<T>.IndexOf(T item) => Math.Max(-1, IndexOf(item));

        void SC.IList.Insert(int index, object value)
        {
            try {
                Insert(index, (T) value);
            }
            catch (InvalidCastException) {
                throw new ArgumentException($"The value \"{value}\" is not of type \"{typeof(T)}\" and cannot be used in this generic collection.{Environment.NewLine}Parameter name: {nameof(value)}");
            }
        }

        void SC.IList.Remove(object value)
        {
            if (IsCompatibleObject(value)) {
                Remove((T) value);
            }
        }

        void SC.IList.RemoveAt(int index) => RemoveAt(index);

        void SCG.IList<T>.RemoveAt(int index) => RemoveAt(index);

        #endregion

        #region Private Members

        private bool CheckVersion(int version)
        {
            if (version == _version) {
                return true;
            }

            // See https://msdn.microsoft.com/library/system.collections.ienumerator.movenext.aspx
            throw new InvalidOperationException(CollectionWasModified);
        }

        private void ClearPrivate()
        {
            _items = EmptyArray;
            Count = 0;
        }

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
            else if (capacity < MinArrayLength) {
                capacity = MinArrayLength;
            }

            if (capacity < requiredCapacity) {
                capacity = requiredCapacity;
            }

            Capacity = capacity;
        }

        [Pure]
        private bool Equals(T x, T y) => EqualityComparer.Equals(x, y);

        [Pure]
        private int GetHashCode(T x) => EqualityComparer.GetHashCode(x);

        private void InsertPrivate(int index, T item)
        {
            #region Code Contracts

            // Argument must be within bounds
            Requires(0 <= index, ArgumentMustBeWithinBounds);
            Requires(index <= Count, ArgumentMustBeWithinBounds);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);

            #endregion

            // Only update version if items are actually added
            UpdateVersion();

            // TODO: Check if Count == Capacity?
            EnsureCapacity(Count + 1);

            // Move items one to the right
            if (index < Count) {
                Array.Copy(_items, index, _items, index + 1, Count - index);
            }
            _items[index] = item;
            Count++;
        }

        private void InsertRangePrivate(int index, T[] items)
        {
            #region Code Contracts

            // Argument must be within bounds
            Requires(0 <= index, ArgumentMustBeWithinBounds);
            Requires(index <= Count, ArgumentMustBeWithinBounds);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);

            #endregion

            // Only update version if items are actually added
            UpdateVersion();

            var count = items.Length;
            EnsureCapacity(Count + count);

            if (index < Count) {
                Array.Copy(_items, index, _items, index + count, Count - index);
            }
            Array.Copy(items, 0, _items, index, count);
            Count += count;
        }

        private static bool IsCompatibleObject(object value) => value is T || value == null && default(T) == null;

        private bool RemoveAllWhere(Func<T, bool> predicate)
        {
            // If result is false, the collection remains unchanged
            Ensures(Result<bool>() || this.IsSameSequenceAs(OldValue(ToArray())));

            if (IsEmpty) {
                return false;
            }

            var shouldRememberItems = ActiveEvents.HasFlag(Removed);
            IExtensible<T> itemsRemoved = null;

            // TODO: Use bulk moves - consider using predicate(item) ^ something
            var j = 0;
            for (var i = 0; i < Count; i++) {
                var item = _items[i];

                if (predicate(item)) {
                    if (shouldRememberItems) {
                        (itemsRemoved ?? (itemsRemoved = new ArrayList<T>())).Add(item);
                    }
                }
                else {
                    // Avoid overriding an item with itself
                    if (j != i) {
                        _items[j] = item;
                    }
                    j++;
                }
            }

            // No items were removed
            if (Count == j) {
                Assert(itemsRemoved == null);
                return false;
            }

            // Only update version if items are actually removed
            UpdateVersion();

            // Clean up
            Array.Clear(_items, j, Count - j);
            Count = j;

            RaiseForRemoveAllWhere(itemsRemoved);

            return true;
        }

        private T RemoveAtPrivate(int index)
        {
            UpdateVersion();
            var item = _items[index];

            if (--Count > index) {
                Array.Copy(_items, index + 1, _items, index, Count - index);
            }
            _items[Count] = default(T);

            return item;
        }

        private void UpdateVersion() => _version++;

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

        private void RaiseForAddRange(SCG.IEnumerable<T> items)
        {
            Requires(items != null);

            if (ActiveEvents.HasFlag(Added)) {
                foreach (var item in items) {
                    OnItemsAdded(item, 1);
                }
            }
            OnCollectionChanged();
        }

        private void RaiseForClear(int count)
        {
            Requires(count >= 1);

            OnCollectionCleared(true, count);
            OnCollectionChanged();
        }

        private void RaiseForIndexSetter(T oldItem, T newItem, int index)
        {
            if (ActiveEvents != None) {
                OnItemRemovedAt(oldItem, index);
                OnItemsRemoved(oldItem, 1);
                OnItemInserted(newItem, index);
                OnItemsAdded(newItem, 1);
                OnCollectionChanged();
            }
        }

        private void RaiseForInsert(int index, T item)
        {
            OnItemInserted(item, index);
            OnItemsAdded(item, 1);
            OnCollectionChanged();
        }

        private void RaiseForInsertRange(int index, T[] array)
        {
            if (ActiveEvents.HasFlag(Inserted | Added)) {
                for (var i = 0; i < array.Length; i++) {
                    var item = array[i];
                    OnItemInserted(item, index + i);
                    OnItemsAdded(item, 1);
                }
            }
            OnCollectionChanged();
        }

        private void RaiseForRemove(T item)
        {
            OnItemsRemoved(item, 1);
            OnCollectionChanged();
        }

        private void RaiseForRemovedAt(T item, int index)
        {
            OnItemRemovedAt(item, index);
            OnItemsRemoved(item, 1);
            OnCollectionChanged();
        }

        private void RaiseForRemoveIndexRange(int startIndex, int count)
        {
            OnCollectionCleared(false, count, startIndex);
            OnCollectionChanged();
        }

        private void RaiseForRemoveAllWhere(SCG.IEnumerable<T> items)
        {
            if (ActiveEvents.HasFlag(Removed)) {
                foreach (var item in items) {
                    OnItemsRemoved(item, 1);
                }
            }
            OnCollectionChanged();
        }

        private void RaiseForReverse() => OnCollectionChanged();

        private void RaiseForShuffle() => OnCollectionChanged();

        private void RaiseForSort() => OnCollectionChanged();

        private void RaiseForUpdate(T item, T oldItem)
        {
            Requires(Equals(item, oldItem));

            OnItemsRemoved(oldItem, 1);
            OnItemsAdded(item, 1);
            OnCollectionChanged();
        }

        #endregion

        #endregion

        #endregion

        #region Nested Types

        // TODO: Introduce base class?
        [Serializable]
        [DebuggerTypeProxy(typeof(CollectionValueDebugView<>))]
        [DebuggerDisplay("{DebuggerDisplay}")]
        private sealed class ItemSet : CollectionValueBase<T>, ICollectionValue<T>
        {
            #region Fields

            private readonly ArrayList<T> _base;
            private readonly int _version;
            // TODO: Replace with HashedArrayList<T>
            private SCG.HashSet<T> _set;

            #endregion

            #region Code Contracts

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                // ReSharper disable InvocationIsSkipped

                // Base list is never null
                Invariant(_base != null);

                // Either the set has not been created, or it contains the same as the base list's distinct items
                Invariant(_set == null || _set.UnsequenceEqual(_base.Distinct(_base.EqualityComparer), _base.EqualityComparer));

                // ReSharper restore InvocationIsSkipped
            }

            #endregion

            #region Constructors

            // TODO: Document
            public ItemSet(ArrayList<T> list)
            {
                #region Code Contracts

                // Argument must be non-null
                Requires(list != null, ArgumentMustBeNonNull);

                #endregion

                _base = list;
                _version = _base._version;
            }

            #endregion

            #region Properties

            public override bool AllowsNull => CheckVersion() & _base.AllowsNull;

            public override int Count
            {
                get {
                    CheckVersion();
                    return Set.Count;
                }
            }

            public override Speed CountSpeed
            {
                get {
                    CheckVersion();
                    // TODO: Always use Linear?
                    return _set == null ? Linear : Constant;
                }
            }

            public override bool IsEmpty => CheckVersion() & _base.IsEmpty;

            #endregion

            #region Public Methods

            public override T Choose()
            {
                CheckVersion();
                return _base.Choose(); // TODO: Is this necessarily an item in the collection value?!
            }

            public override void CopyTo(T[] array, int arrayIndex)
            {
                CheckVersion();
                Set.CopyTo(array, arrayIndex);
            }

            public override bool Equals(object obj) => CheckVersion() & base.Equals(obj);

            public override SCG.IEnumerator<T> GetEnumerator()
            {
                // If a set already exists, enumerate that
                if (_set != null) {
                    var enumerator = Set.GetEnumerator();
                    while (CheckVersion() & enumerator.MoveNext()) {
                        yield return enumerator.Current;
                    }
                }
                // Otherwise, evaluate lazily
                else {
                    var set = new SCG.HashSet<T>(_base.EqualityComparer);

                    var enumerator = _base.GetEnumerator();
                    while (CheckVersion() & enumerator.MoveNext()) {
                        // Only return new items
                        if (set.Add(enumerator.Current)) {
                            yield return enumerator.Current;
                        }
                    }

                    // Save set for later (re)user
                    _set = set;
                }
            }

            public override int GetHashCode()
            {
                CheckVersion();
                return base.GetHashCode();
            }

            public override T[] ToArray()
            {
                CheckVersion();
                return Set.ToArray();
            }

            #endregion

            #region Private Members

            private string DebuggerDisplay => _version == _base._version ? ToString() : "Expired collection value; original collection was modified since range was created.";

            private bool CheckVersion() => _base.CheckVersion(_version);

            // TODO: Replace with HashedArrayList<T>!
            private SCG.ISet<T> Set => _set ?? (_set = new SCG.HashSet<T>(_base, _base.EqualityComparer));

            #endregion
        }


        /// <summary>
        ///     Represents a range of an <see cref="ArrayList{T}"/>.
        /// </summary>
        [Serializable]
        [DebuggerTypeProxy(typeof(CollectionValueDebugView<>))]
        [DebuggerDisplay("{DebuggerDisplay}")]
        private sealed class Range : CollectionValueBase<T>, IDirectedCollectionValue<T>
        {
            #region Fields

            private readonly ArrayList<T> _base;
            private readonly int _version, _startIndex, _count, _sign;
            private readonly EnumerationDirection _direction;

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Range"/> class that starts at the specified index and spans the next
            ///     <paramref name="count"/> items in the specified direction.
            /// </summary>
            /// <param name="list">
            ///     The underlying <see cref="ArrayList{T}"/>.
            /// </param>
            /// <param name="startIndex">
            ///     The zero-based <see cref="ArrayList{T}"/> index at which the range starts.
            /// </param>
            /// <param name="count">
            ///     The number of items in the range.
            /// </param>
            /// <param name="direction">
            ///     The direction of the range.
            /// </param>
            public Range(ArrayList<T> list, int startIndex, int count, EnumerationDirection direction)
            {
                #region Code Contracts

                // Argument must be non-null
                Requires(list != null, ArgumentMustBeNonNull);

                // Argument must be within bounds
                Requires(-1 <= startIndex, ArgumentMustBeWithinBounds);
                Requires(startIndex < list.Count || startIndex == 0 && count == 0, ArgumentMustBeWithinBounds);

                // Argument must be within bounds
                Requires(0 <= count, ArgumentMustBeWithinBounds);
                Requires(direction.IsForward() ? startIndex + count <= list.Count : count <= startIndex + 1, ArgumentMustBeWithinBounds);

                // Argument must be valid enum constant
                Requires(Enum.IsDefined(typeof(EnumerationDirection), direction), EnumMustBeDefined);


                Ensures(_base != null);
                Ensures(_version == _base._version);
                Ensures(_sign == (direction.IsForward() ? 1 : -1));
                Ensures(-1 <= _startIndex);
                Ensures(_startIndex < _base.Count || _startIndex == 0 && _base.Count == 0);
                Ensures(-1 <= _startIndex + _sign * _count);
                Ensures(_startIndex + _sign * _count <= _base.Count);

                #endregion

                _base = list;
                _version = list._version;
                _sign = (int) direction;
                _startIndex = startIndex;
                _count = count;
                _direction = direction;
            }

            #endregion

            #region Properties

            public override bool AllowsNull => CheckVersion() & _base.AllowsNull;

            public override int Count
            {
                get {
                    CheckVersion();
                    return _count;
                }
            }

            public override Speed CountSpeed
            {
                get {
                    CheckVersion();
                    return Constant;
                }
            }

            public EnumerationDirection Direction
            {
                get {
                    CheckVersion();
                    return _direction;
                }
            }

            #endregion

            #region Public Methods

            public IDirectedCollectionValue<T> Backwards()
            {
                CheckVersion();
                var startIndex = _startIndex + (_count - 1) * _sign;
                var direction = Direction.Opposite();
                return new Range(_base, startIndex, _count, direction);
            }

            public override T Choose()
            {
                CheckVersion();
                // Select the highest index in the range
                var index = _direction.IsForward() ? _startIndex + _count - 1 : _startIndex;
                return _base._items[index];
            }

            public override void CopyTo(T[] array, int arrayIndex)
            {
                CheckVersion();
                if (_direction.IsForward()) {
                    // Copy array directly
                    Array.Copy(_base._items, _startIndex, array, arrayIndex, _count);
                }
                else {
                    // Use enumerator instead of copying and then reversing
                    base.CopyTo(array, arrayIndex);
                }
            }

            public override bool Equals(object obj) => CheckVersion() & base.Equals(obj);

            public override SCG.IEnumerator<T> GetEnumerator()
            {
                var items = _base._items;
                for (var i = 0; i < Count; i++) {
                    yield return items[_startIndex + _sign * i];
                }
            }

            public override int GetHashCode()
            {
                CheckVersion();
                return base.GetHashCode();
            }

            public override T[] ToArray()
            {
                CheckVersion();
                return base.ToArray();
            }

            #endregion

            #region Private Members

            private string DebuggerDisplay => _version == _base._version ? ToString() : "Expired collection value; original collection was modified since range was created.";

            private bool CheckVersion() => _base.CheckVersion(_version);

            #endregion
        }

        #endregion
    }
}