// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

using C6.Contracts;

using SC = System.Collections;
using SCG = System.Collections.Generic;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;
using static C6.EnumerationDirection;
using static C6.EventTypes;
using static C6.Speed;


namespace C6.Collections
{
    /// <summary>
    ///     Represents a generic doubly linked list.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the collection.
    /// </typeparam>
    [Serializable]
    [DebuggerTypeProxy(typeof(CollectionValueDebugView<>))]
    public class LinkedList<T> : SequenceBase<T>, IList<T>
    {
        #region Fields

        private readonly Node _first, _last;

        #endregion

        #region Code Contracts

        [ContractInvariantMethod]
        // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            // ReSharper disable InvocationIsSkipped

            // _first is never null
            Invariant(_first != null);

            // The node before the first is always null
            Invariant(_first.Previous == null);

            // _last is never null
            Invariant(_last != null);

            // The node after the last is always null
            Invariant(_last.Next == null);

            // If collection is empty, _first and _last point at each other
            Invariant(!IsEmpty || _first.Next == _last && _first == _last.Previous);

            // List is equal forwards and backwards
            Invariant(EnumerateFrom(_first.Next).IsSameSequenceAs(EnumerateBackwardsFrom(_last.Previous).Reverse()));

            // All items must be non-null if collection disallows null values
            Invariant(AllowsNull || ForAll(this, item => item != null));

            // List links are correct
            Invariant(ListLinksAreCorrect());

            // ReSharper restore InvocationIsSkipped
        }

        [Pure]
        private bool ListLinksAreCorrect()
        {
            var cursor = _first.Next;

            do {
                if (cursor.Previous.Next != cursor) {
                    return false;
                }
            } while ((cursor = cursor.Next) != null);

            return true;
        }

        #endregion

        #region Constructors

        public LinkedList(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
        {
            _first = new Node();
            _last = new Node(default(T), _first);
            _first.Next = _last;

            AllowsNull = allowsNull;
            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;
        }

        public LinkedList(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) : this(equalityComparer, allowsNull)
        {
            foreach (var item in items) {
                InsertBefore(item, _last);
            }
        }

        #endregion

        #region Properties

        public override bool AllowsDuplicates => true;

        public override bool AllowsNull { get; }

        public override Speed ContainsSpeed => Linear;

        public override Speed CountSpeed => Constant;

        public override bool DuplicatesByCounting => false;

        public override SCG.IEqualityComparer<T> EqualityComparer { get; }

        public T First => _first.Next.Item;

        public Speed IndexingSpeed => Linear;

        public override bool IsFixedSize => false;

        public override bool IsReadOnly => false;

        public T Last => _last.Previous.Item;

        public override EventTypes ListenableEvents => All;

        public T this[int index]
        {
            get { return GetNode(index).Item; }
            set {
                #region Code Contracts

                // The version is updated
                Ensures(Version != OldValue(Version));

                #endregion

                UpdateVersion();
                var node = GetNode(index);
                var oldItem = node.Item;
                node.Item = value;
                RaiseForIndexSetter(oldItem, value, index);
            }
        }

        #endregion

        #region Methods

        public override bool Add(T item)
        {
            #region Code Contracts

            // The version is updated
            Ensures(Version != OldValue(Version));

            #endregion

            UpdateVersion();
            InsertBefore(item, _last);
            RaiseForAdd(item);
            return true;
        }

        public override bool AddRange(SCG.IEnumerable<T> items)
        {
            Node first, last;
            var count = EnumerateToList(items, out first, out last);

            if (count == 0) {
                return false;
            }

            UpdateVersion();
            Count += count;

            // Make last node in existing list and first in new list point to each other
            first.Previous = _last.Previous;
            first.Previous.Next = first;

            // Make last node in new list and _last point to each other
            last.Next = _last;
            _last.Previous = last;

            RaiseForAddRange(EnumerateFrom(first));
            return true;
        }

        public override IDirectedCollectionValue<T> Backwards() => new Range(this, 0, Count, EnumerationDirection.Backwards);

        public override T Choose() => _last.Previous.Item;

        public override void Clear()
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || Version != OldValue(Version));

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

        public override bool Contains(T item)
        {
            Node node;
            return Contains(item, out node);
        }

        public override int CountDuplicates(T item) => this.Count(x => Equals(x, item));

        public override bool Find(ref T item)
        {
            Node node;
            if (Contains(item, out node)) {
                item = node.Item;
                return true;
            }

            return false;
        }

        public override ICollectionValue<T> FindDuplicates(T item) => new Duplicates(this, item);

        public override SCG.IEnumerator<T> GetEnumerator() => EnumerateFrom(_first.Next).GetEnumerator();

        public IDirectedCollectionValue<T> GetIndexRange(int startIndex, int count) => new Range(this, startIndex, count, Forwards);

        public int IndexOf(T item)
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
                var node = _first.Next;
                var index = 0;

                while (node != _last) {
                    if (node.Item == null) {
                        return index;
                    }

                    ++index;
                    node = node.Next;
                }
            }
            else {
                var node = _first.Next;
                var index = 0;

                while (node != _last) {
                    if (Equals(item, node.Item)) {
                        return index;
                    }

                    ++index;
                    node = node.Next;
                }
            }

            return ~Count;
        }

        public void Insert(int index, T item)
        {
            #region Code Contracts

            // The version is updated
            Ensures(Version != OldValue(Version));

            #endregion

            UpdateVersion();
            InsertBefore(item, GetNode(index));
            RaiseForInsert(index, item);
        }

        public void InsertFirst(T item)
        {
            #region Code Contracts

            // The version is updated
            Ensures(Version != OldValue(Version));

            #endregion

            UpdateVersion();
            InsertAfter(item, _first);
            RaiseForInsert(0, item);
        }

        public void InsertLast(T item) => Insert(Count, item);

        public void InsertRange(int index, SCG.IEnumerable<T> items)
        {
            Node first, last;
            var count = EnumerateToList(items, out first, out last);

            if (count == 0) {
                return;
            }

            UpdateVersion();

            var node = GetNode(index);
            Count += count;

            node.Previous.Next = first;
            first.Previous = node.Previous;

            node.Previous = last;
            last.Next = node;

            RaiseForInsertRange(index, EnumerateFromTo(first, node));
        }
        
        public virtual bool IsSorted() => CollectionExtensions.IsSorted(this);
        
        public virtual bool IsSorted(Comparison<T> comparison) => CollectionExtensions.IsSorted(this, comparison);

        public virtual bool IsSorted(SCG.IComparer<T> comparer) => CollectionExtensions.IsSorted(this, comparer);

        public override ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
        {
            throw new NotImplementedException();
        }

        public int LastIndexOf(T item)
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
                var node = _last.Previous;
                var index = Count - 1;

                while (node != _first) {
                    if (node.Item == null) {
                        return index;
                    }

                    --index;
                    node = node.Previous;
                }
            }
            else {
                var node = _last.Previous;
                var index = Count - 1;

                while (node != _first) {
                    if (Equals(item, node.Item)) {
                        return index;
                    }

                    --index;
                    node = node.Previous;
                }
            }

            return ~Count;
        }

        public override bool Remove(T item, out T removedItem)
        {
            Node node;
            if (Contains(item, out node)) {
                UpdateVersion();
                removedItem = Remove(node);
                RaiseForRemove(removedItem);
                return true;
            }

            removedItem = default(T);
            return false;
        }

        public T RemoveAt(int index)
        {
            UpdateVersion();
            var item = Remove(GetNode(index));
            RaiseForRemoveAt(item, index);
            return item;
        }

        public override bool RemoveDuplicates(T item) => item == null ? RemoveAllWhere(x => x == null) : RemoveAllWhere(x => Equals(item, x));

        public T RemoveFirst() => RemoveAt(0);

        public void RemoveIndexRange(int startIndex, int count)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || Version != OldValue(Version));

            #endregion

            if (count == 0) {
                return;
            }

            UpdateVersion();

            // TODO: Find the node is the most optimal way
            Node start = GetNode(startIndex), end = GetNode(startIndex + count - 1);

            Count -= count;
            start.Previous.Next = end.Next;
            end.Next.Previous = start.Previous;

            RaiseForRemoveIndexRange(startIndex, count);
        }

        public T RemoveLast() => RemoveAt(Count - 1);

        public override bool RemoveRange(SCG.IEnumerable<T> items)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || Version != OldValue(Version));

            #endregion

            if (IsEmpty || items.IsEmpty()) {
                return false;
            }

            // TODO: Replace ArrayList<T> with more efficient data structure like HashBag<T>
            var itemsToRemove = new ArrayList<T>(items, EqualityComparer, AllowsNull);
            return RemoveAllWhere(item => itemsToRemove.Remove(item));
        }

        public override bool RetainRange(SCG.IEnumerable<T> items)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || Version != OldValue(Version));

            #endregion

            if (IsEmpty) {
                return false;
            }

            if (items.IsEmpty()) {
                // Optimize call, if no items should be retained
                UpdateVersion();
                var itemsRemoved = _last.Previous;
                ClearPrivate();
                RaiseForRemoveAllWhere(itemsRemoved);
                return true;
            }

            // TODO: Replace ArrayList<T> with more efficient data structure like HashBag<T>
            var itemsToRemove = new ArrayList<T>(items, EqualityComparer, AllowsNull);
            return RemoveAllWhere(item => !itemsToRemove.Remove(item));
        }

        public void Reverse()
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || Version != OldValue(Version));

            #endregion

            if (Count <= 1) {
                return;
            }

            // Only update version if the collection is actually reversed
            UpdateVersion();

            var count = Count / 2;
            Node leftNode = _first, rightNode = _last;

            while (count-- > 0) {
                leftNode = leftNode.Next;
                rightNode = rightNode.Previous;

                // Swap items
                var item = leftNode.Item;
                leftNode.Item = rightNode.Item;
                rightNode.Item = item;
            }

            RaiseForReverse();
        }

        public virtual void Shuffle() => Shuffle(new Random());

        public void Shuffle(Random random)
        {
            #region Code Contracts

            // If collection changes, the version is updated
            Ensures(this.IsSameSequenceAs(OldValue(ToArray())) || Version != OldValue(Version));

            #endregion

            if (Count <= 1) {
                return;
            }

            // Only update version if the collection is shuffled
            UpdateVersion();

            // Shuffle items in an array
            var array = ToArray();
            array.Shuffle(random);

            // Copy them back to the list
            var cursor = _first.Next;
            var i = 0;
            while (cursor != _last) {
                cursor.Item = array[i++];
                cursor = cursor.Next;
            }

            RaiseForShuffle();
        }

        public void Sort()
        {
            throw new NotImplementedException();
        }

        public void Sort(SCG.IComparer<T> comparer)
        {
            throw new NotImplementedException();
        }

        public void Sort(Comparison<T> comparison)
        {
            throw new NotImplementedException();
        }

        public override ICollectionValue<T> UniqueItems() => new ItemSet(this);

        public override bool Update(T item, out T oldItem)
        {
            Node node;
            if (Contains(item, out node)) {
                UpdateVersion();

                oldItem = node.Item;
                node.Item = item;

                RaiseForUpdate(item, oldItem);

                return true;
            }

            oldItem = default(T);
            return false;
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
                CopyTo((T[]) array, index);
            }
            catch (InvalidCastException) {
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

        #region Events

        private void RaiseForRemoveAllWhere(Node items)
        {
            if (ActiveEvents.HasFlag(Removed)) {
                while (items != null) {
                    OnItemsRemoved(items.Item, 1);
                    items = items.Previous;
                }
            }
            OnCollectionChanged();
        }

        #endregion

        #region Private Methods

        private void ClearPrivate()
        {
            _first.Next = _last;
            _last.Previous = _first;
            Count = 0;
        }

        private bool Contains(T item, out Node node)
        {
            node = _first.Next;
            while (node != _last) {
                if (Equals(item, node.Item)) {
                    return true;
                }
                node = node.Next;
            }

            node = null;
            return false;
        }

        [Pure]
        private SCG.IEnumerable<T> EnumerateBackwardsFrom(Node cursor)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(cursor != null, ArgumentMustBeNonNull);

            // Node cannot be the last sentinel node
            Requires(cursor != _last);

            // The version is not updated
            Ensures(Version == OldValue(Version));

            #endregion

            return EnumerateBackwardsFromTo(cursor, _first);
        }

        [Pure]
        private SCG.IEnumerable<T> EnumerateBackwardsFromTo(Node cursor, Node last)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(cursor != null, ArgumentMustBeNonNull);

            // Node cannot be the last sentinel node
            Requires(cursor != _last);

            // The version is not updated
            Ensures(Version == OldValue(Version));

            #endregion

            var version = Version;

            // Check version at each call to MoveNext() to ensure an exception is thrown even when the enumerator was really finished
            while (CheckVersion(version) & cursor != last) {
                yield return cursor.Item;
                cursor = cursor.Previous;
            }
        }

        [Pure]
        private SCG.IEnumerable<T> EnumerateFrom(Node cursor)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(cursor != null, ArgumentMustBeNonNull);

            // Node cannot be the first sentinel node
            Requires(cursor != _first);

            // The version is not updated
            Ensures(Version == OldValue(Version));

            #endregion

            return EnumerateFromTo(cursor, _last);
        }

        [Pure]
        private SCG.IEnumerable<T> EnumerateFromTo(Node cursor, Node last)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(cursor != null, ArgumentMustBeNonNull);

            // Node cannot be the first sentinel node
            Requires(cursor != _first);

            // Argument must be non-null
            Requires(last != null, ArgumentMustBeNonNull);

            // The version is not updated
            Ensures(Version == OldValue(Version));

            #endregion

            var version = Version;

            // Check version at each call to MoveNext() to ensure an exception is thrown even when the enumerator was really finished
            while (CheckVersion(version) & cursor != last) {
                yield return cursor.Item;
                cursor = cursor.Next;
            }
        }

        /// <summary>
        ///     Creates a linked list starting with <paramref name="first"/> and ending with <paramref name="last"/> and returns
        ///     the number of items in the list.
        /// </summary>
        /// <param name="items">
        ///     The enumerable whose items should be copied to the list.
        /// </param>
        /// <param name="first">
        ///     The first node in the list.
        /// </param>
        /// <param name="last">
        ///     The last included node in the list.
        /// </param>
        /// <returns>
        ///     The number of items in the list.
        /// </returns>
        [Pure]
        private static int EnumerateToList(SCG.IEnumerable<T> items, out Node first, out Node last)
        {
            var enumerator = items.GetEnumerator();
            if (!enumerator.MoveNext()) {
                first = last = null;
                return 0;
            }

            var count = 1;
            first = last = new Node(enumerator.Current);

            while (enumerator.MoveNext()) {
                ++count;
                last = new Node(enumerator.Current, last);
            }

            return count;
        }

        [Pure]
        private bool Equals(T x, T y) => EqualityComparer.Equals(x, y);

        [Pure]
        private int GetHashCode(T x) => EqualityComparer.GetHashCode(x);

        [Pure]
        private Node GetNode(int index)
        {
            #region Code Contracts

            // Argument must be within bounds (collection must be non-empty)
            Requires(0 <= index, ArgumentMustBeWithinBounds);
            Requires(index <= Count, ArgumentMustBeWithinBounds);

            // TODO: Ensure it is the right node

            #endregion

            // Closer to beginning
            if (index < Count / 2) {
                var node = _first.Next;
                for (var i = 0; i < index; i++) {
                    node = node.Next;
                }
                return node;
            }
            // Closer to end
            else {
                var node = _last;
                for (var i = Count; i > index; i--) {
                    node = node.Previous;
                }
                return node;
            }
        }

        private Node InsertAfter(T item, Node previous)
        {
            // The incrementation must be before adding the next item, because the incrementation requires a read, which will otherwise violate a contract
            ++Count;
            return new Node(item, previous, previous.Next);
        }

        private Node InsertBefore(T item, Node next)
        {
            // The incrementation must be before adding the next item, because the incrementation requires a read, which will otherwise violate a contract
            ++Count;
            return new Node(item, next.Previous, next);
        }

        private static bool IsCompatibleObject(object value) => value is T || value == null && default(T) == null;

        private T Remove(Node node)
        {
            --Count;
            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;
            return node.Item;
        }

        private bool RemoveAllWhere(Func<T, bool> predicate)
        {
            if (IsEmpty) {
                return false;
            }

            var shouldRememberItems = ActiveEvents.HasFlag(Removed);
            Node itemsRemoved = null;
            var count = Count;

            var node = _first.Next;
            while (node != _last) {
                if (predicate(node.Item)) {
                    Remove(node);

                    if (shouldRememberItems) {
                        node.Previous = itemsRemoved;
                        itemsRemoved = node;
                    }
                }

                node = node.Next;
            }

            // No items were removed
            if (count == Count) {
                return false;
            }

            // Only update version if items are actually removed
            UpdateVersion();
            RaiseForRemoveAllWhere(itemsRemoved);
            return true;
        }

        #endregion

        #region Nested Types

        // TODO: Explicitly check against null to avoid using the (slower) equality comparer
        [Serializable]
        [DebuggerTypeProxy(typeof(CollectionValueDebugView<>))]
        [DebuggerDisplay("{DebuggerDisplay}")]
        private sealed class Duplicates : CollectionValueBase<T>, ICollectionValue<T>
        {
            #region Fields

            private readonly LinkedList<T> _base;
            private readonly IList<T> _list;
            private readonly int _version;
            private readonly T _item;
            private SCG.IEnumerator<T> _enumerator;

            #endregion

            #region Code Contracts

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                // ReSharper disable InvocationIsSkipped

                // List is never null
                Invariant(_list != null);

                // Base list is never null
                Invariant(_base != null);

                // All items in the list are equal to _item
                Invariant(ForAll(_list, x => _base.EqualityComparer.Equals(x, _item)));

                // The items already found are the first list.Count equal items
                Invariant(_base.Where(x => _base.EqualityComparer.Equals(x, _item)).Take(_list.Count).IsSameSequenceAs(_list)); // TODO: Check if valid?

                // If the enumerator is used, all duplicates have been found
                Invariant(!AllDuplicatesFound || _base.Where(x => _base.EqualityComparer.Equals(x, _item)).IsSameSequenceAs(_list));

                // ReSharper restore InvocationIsSkipped
            }

            #endregion

            #region Constructors

            // TODO: Document
            public Duplicates(LinkedList<T> list, T item)
            {
                #region Code Contracts

                // Argument must be non-null
                Requires(list != null, ArgumentMustBeNonNull);

                // Argument must be non-null if collection disallows null values
                Requires(list.AllowsNull || item != null, ItemMustBeNonNull);

                #endregion

                _base = list;
                _version = _base.Version;
                _item = item;
                _enumerator = list.GetEnumerator();
                _list = new ArrayList<T>(equalityComparer: _base.EqualityComparer, allowsNull: _base.AllowsNull);
            }

            #endregion

            #region Properties

            public override bool AllowsNull => CheckVersion() & _base.AllowsNull;

            public override int Count
            {
                get {
                    CheckVersion();
                    FindAll();
                    return _list.Count;
                }
            }

            public override Speed CountSpeed => CheckVersion() & AllDuplicatesFound ? Constant : Linear;

            public override bool IsEmpty => CheckVersion() & AllDuplicatesFound ? _list.IsEmpty : _list.IsEmpty && FindNext();

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
                FindAll();
                _list.CopyTo(array, arrayIndex);
            }

            public override bool Equals(object obj) => CheckVersion() & base.Equals(obj);

            public override SCG.IEnumerator<T> GetEnumerator()
            {
                // If all duplicates have been found, simply enumerate the list
                if (AllDuplicatesFound) {
                    using (var enumerator = _list.GetEnumerator()) {
                        while (CheckVersion() & enumerator.MoveNext()) {
                            yield return enumerator.Current;
                        }
                    }
                }
                // Otherwise, evaluate lazily
                else {
                    var index = 0;
                    while (CheckVersion() & index < _list.Count || FindNext()) {
                        Assert(index < _list.Count);
                        yield return _list[index++];
                    }
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
                FindAll();
                return _list.ToArray();
            }

            #endregion

            #region Private Members

            private bool AllDuplicatesFound => _enumerator == null;

            private bool CheckVersion() => _base.CheckVersion(_version);

            private string DebuggerDisplay => _version == _base.Version ? ToString() : "Expired collection value; original collection was modified since range was created.";

            /// <summary>
            ///     Finds all duplicates in the base collection.
            /// </summary>
            private void FindAll()
            {
                while (FindNext()) {}
            }

            private bool FindNext()
            {
                if (AllDuplicatesFound) {
                    return false;
                }

                while (CheckVersion()) {
                    // Check if enumerator is done
                    if (!_enumerator.MoveNext()) {
                        // Set enumerator to null to indicate that the base has been fully enumerated
                        _enumerator = null;

                        return false;
                    }

                    // Add duplicate to list, or continue the loop
                    if (_base.Equals(_enumerator.Current, _item)) {
                        _list.Add(_enumerator.Current);
                        return true;
                    }
                }

                // This is never executed as CheckVersion() throws an exception instead of returning false
                return false;
            }

            #endregion
        }


        // TODO: Introduce base class?
        // TODO: Consider using HashedArrayList<T> instead of Distinct()
        [Serializable]
        [DebuggerTypeProxy(typeof(CollectionValueDebugView<>))]
        [DebuggerDisplay("{DebuggerDisplay}")]
        private sealed class ItemSet : CollectionValueBase<T>, ICollectionValue<T>
        {
            #region Fields

            private readonly LinkedList<T> _base;
            private readonly int _version;
            private readonly IList<T> _list;
            private SCG.IEnumerator<T> _enumerator;

            #endregion

            #region Code Contracts

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                // ReSharper disable InvocationIsSkipped

                // List is never null
                Invariant(_list != null);

                // Base list is never null
                Invariant(_base != null);

                // All items in the list are distinct
                Invariant(_list.Distinct(_base.EqualityComparer).IsSameSequenceAs(_list));

                // The items already found are the first list.Count distinct items
                Invariant(_base.Distinct(_base.EqualityComparer).Take(_list.Count).IsSameSequenceAs(_list));

                // If the enumerator is used, all duplicates have been found
                Invariant(!AllUniqueItemsFound || _base.Distinct(_base.EqualityComparer).IsSameSequenceAs(_list));

                // ReSharper restore InvocationIsSkipped
            }

            #endregion

            #region Constructors

            // TODO: Document
            public ItemSet(LinkedList<T> list)
            {
                #region Code Contracts

                // Argument must be non-null
                Requires(list != null, ArgumentMustBeNonNull);

                #endregion

                _base = list;
                _version = _base.Version;
                _enumerator = list.Distinct(list.EqualityComparer).GetEnumerator();
                _list = new ArrayList<T>(equalityComparer: list.EqualityComparer, allowsNull: list.AllowsNull);
            }

            #endregion

            #region Properties

            public override bool AllowsNull => CheckVersion() & _base.AllowsNull;

            public override int Count
            {
                get {
                    CheckVersion();
                    FindAll();
                    return _list.Count;
                }
            }

            public override Speed CountSpeed => CheckVersion() & AllUniqueItemsFound ? Constant : Linear;

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
                FindAll();
                _list.CopyTo(array, arrayIndex);
            }

            public override bool Equals(object obj) => CheckVersion() & base.Equals(obj);

            public override SCG.IEnumerator<T> GetEnumerator()
            {
                // If all duplicates have been found, simply enumerate the list
                if (AllUniqueItemsFound) {
                    using (var enumerator = _list.GetEnumerator()) {
                        while (CheckVersion() & enumerator.MoveNext()) {
                            yield return enumerator.Current;
                        }
                    }
                }
                // Otherwise, evaluate lazily
                else {
                    var index = 0;
                    while (CheckVersion() & index < _list.Count || FindNext()) {
                        Assert(index < _list.Count);
                        yield return _list[index++];
                    }
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
                FindAll();
                return _list.ToArray();
            }

            #endregion

            #region Private Members

            private bool AllUniqueItemsFound => _enumerator == null;

            private bool CheckVersion() => _base.CheckVersion(_version);

            private string DebuggerDisplay => _version == _base.Version ? ToString() : "Expired collection value; original collection was modified since range was created.";

            /// <summary>
            ///     Finds all duplicates in the base collection.
            /// </summary>
            private void FindAll()
            {
                while (FindNext()) {}
            }

            private bool FindNext()
            {
                if (AllUniqueItemsFound) {
                    return false;
                }

                // Check if enumerator is done
                if (CheckVersion() & !_enumerator.MoveNext()) {
                    // Set enumerator to null to indicate that the base has been fully enumerated
                    _enumerator = null;

                    return false;
                }

                _list.Add(_enumerator.Current);
                return true;
            }

            #endregion
        }


        /// <summary>
        ///     Represents an individual cell in the linked list.
        /// </summary>
        [Serializable]
        [DebuggerDisplay("Node({Item})")]
        private class Node
        {
            public Node Previous, Next;
            public T Item;

            public Node() {}

            public Node(T item)
            {
                Item = item;
            }

            public Node(T item, Node previous)
            {
                Requires(previous != null, ItemMustBeNonNull);

                Item = item;

                // Set previous' pointers
                Previous = previous;
                previous.Next = this;
            }

            public Node(T item, Node previous, Node next)
            {
                Requires(previous != null, ItemMustBeNonNull);
                Requires(next != null, ItemMustBeNonNull);

                Item = item;

                // Set previous' pointers
                Previous = previous;
                previous.Next = this;

                // Set next's
                Next = next;
                next.Previous = this;
            }
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

            private readonly int _startIndex, _count, _version;
            private readonly Node _leftNode, _rightNode;
            private readonly LinkedList<T> _base;
            private readonly EnumerationDirection _direction;

            #endregion

            #region Constructors

            public Range(LinkedList<T> list, int startIndex, int count, EnumerationDirection direction)
            {
                #region Code Contracts

                // Argument must be non-null
                Requires(list != null, ArgumentMustBeNonNull);

                // Argument must be within bounds
                Requires(0 <= startIndex, ArgumentMustBeWithinBounds);
                Requires(startIndex + count <= list.Count, ArgumentMustBeWithinBounds);

                // Argument must be non-negative
                Requires(0 <= count, ArgumentMustBeNonNegative);


                // Argument must be valid enum constant
                Requires(direction.IsDefined(), EnumMustBeDefined);


                Ensures(_base != null);
                Ensures(_version == _base.Version);

                #endregion

                _base = list;
                _version = list.Version;
                _startIndex = startIndex;
                _count = count;
                _direction = direction;

                if (count > 0) {
                    _leftNode = list.GetNode(startIndex);
                    _rightNode = list.GetNode(startIndex + count - 1);
                }
            }

            /// <summary>
            ///     Creates a new <see cref="Range"/> with the opposite direction of <paramref name="range"/>.
            /// </summary>
            /// <param name="range">
            ///     The range to make a backwards version of.
            /// </param>
            private Range(Range range)
            {
                _base = range._base;
                _version = range._version;
                _startIndex = range._startIndex;
                _count = range._count;
                _direction = range._direction.Opposite();
                _leftNode = range._leftNode;
                _rightNode = range._rightNode;
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
                return new Range(this);
            }

            public override T Choose()
            {
                CheckVersion();
                // Select the highest index in the range
                return _rightNode.Item;
            }

            public override void CopyTo(T[] array, int arrayIndex)
            {
                CheckVersion();
                base.CopyTo(array, arrayIndex);
            }

            public override bool Equals(object obj) => CheckVersion() & base.Equals(obj);

            public override SCG.IEnumerator<T> GetEnumerator()
            {
                CheckVersion();

                if (IsEmpty) {
                    yield break;
                }

                var count = _count;

                if (_direction.IsForward()) {
                    var cursor = _leftNode;
                    yield return cursor.Item;

                    while (--count > 0) {
                        cursor = cursor.Next;
                        CheckVersion();
                        yield return cursor.Item;
                    }
                }
                else {
                    var cursor = _rightNode;
                    yield return cursor.Item;

                    while (--count > 0) {
                        cursor = cursor.Previous;
                        CheckVersion();
                        yield return cursor.Item;
                    }
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

            private string DebuggerDisplay => _version == _base.Version ? ToString() : "Expired collection value; original collection was modified since range was created.";

            private bool CheckVersion() => _base.CheckVersion(_version);

            #endregion
        }

        #endregion
    }
}