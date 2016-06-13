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
    public class LinkedList<T> : SequenceBase<T>, ISequenced<T>
    {
        #region Fields

        private readonly Node _first, _last;

        private int _sequencedHashCodeVersion = -1, _unsequencedHashCodeVersion = -1;
        private int _sequencedHashCode, _unsequencedHashCode;

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

        public override Speed CountSpeed => Constant;

        public override bool DuplicatesByCounting => false;

        public override SCG.IEqualityComparer<T> EqualityComparer { get; }

        public override bool IsFixedSize => false;

        public override Speed ContainsSpeed => Linear;

        public override bool IsReadOnly => false;

        public override EventTypes ListenableEvents => All;

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
            // Create temporary list from items, which can be inserted at end
            var enumerator = items.GetEnumerator();
            if (!enumerator.MoveNext()) {
                return false;
            }
            var count = Count + 1;
            var first = new Node(enumerator.Current);
            var last = first;
            while (enumerator.MoveNext()) {
                ++count;
                last = new Node(enumerator.Current, last);
            }

            UpdateVersion();
            Count = count;

            // Make last node in existing list and first in new list point to each other
            first.Previous = _last.Previous;
            first.Previous.Next = first;

            // Make last node in new list and _last point to each other
            last.Next = _last;
            _last.Previous = last;

            RaiseForAddRange(EnumerateFrom(first));
            return true;
        }

        public override IDirectedCollectionValue<T> Backwards()
        {
            throw new NotImplementedException();
        }

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

        // TODO: Update hash code when items are added, if the hash code version is not equal to -1
        public override int GetUnsequencedHashCode()
        {
            if (_unsequencedHashCodeVersion != Version) {
                _unsequencedHashCodeVersion = Version;
                _unsequencedHashCode = this.GetUnsequencedHashCode(EqualityComparer);
            }

            return _unsequencedHashCode;
        }

        public override ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
        {
            throw new NotImplementedException();
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

        public override bool RemoveDuplicates(T item) => item == null ? RemoveAllWhere(x => x == null) : RemoveAllWhere(x => Equals(item, x));

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

            var version = Version;

            // Check version at each call to MoveNext() to ensure an exception is thrown even when the enumerator was really finished
            while (CheckVersion(version) & cursor != _first) {
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

            var version = Version;

            // Check version at each call to MoveNext() to ensure an exception is thrown even when the enumerator was really finished
            while (CheckVersion(version) & cursor != _last) {
                yield return cursor.Item;
                cursor = cursor.Next;
            }
        }

        [Pure]
        private bool Equals(T x, T y) => EqualityComparer.Equals(x, y);

        [Pure]
        private int GetHashCode(T x) => EqualityComparer.GetHashCode(x);

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

        #endregion
    }
}