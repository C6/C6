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

using static C6.Collections.ExceptionMessages;
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
    public class LinkedList<T> : CollectionBase<T>, ICollection<T>
    {
        #region Fields

        private readonly Node _first, _last;
        private int _version;

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

        public override Speed ContainsSpeed { get; }

        public override bool IsReadOnly => false;

        public override EventTypes ListenableEvents => All;

        #endregion

        #region Methods

        public override bool Add(T item)
        {
            #region Code Contracts

            // The version is updated
            Ensures(_version != OldValue(_version));

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

        public override T Choose() => _last.Previous.Item;

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsRange(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public override int CountDuplicates(T item)
        {
            throw new NotImplementedException();
        }

        public override bool Find(ref T item)
        {
            throw new NotImplementedException();
        }

        public override ICollectionValue<T> FindDuplicates(T item)
        {
            throw new NotImplementedException();
        }

        public override SCG.IEnumerator<T> GetEnumerator() => EnumerateFrom(_first.Next).GetEnumerator();

        public override int GetUnsequencedHashCode()
        {
            throw new NotImplementedException();
        }

        public override ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
        {
            throw new NotImplementedException();
        }

        public override bool Remove(T item, out T removedItem)
        {
            throw new NotImplementedException();
        }

        public override bool RemoveDuplicates(T item)
        {
            throw new NotImplementedException();
        }

        public override bool RemoveRange(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public override bool RetainRange(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public override ICollectionValue<T> UniqueItems()
        {
            throw new NotImplementedException();
        }

        public override bool UnsequencedEquals(ICollection<T> otherCollection)
        {
            throw new NotImplementedException();
        }

        public override bool Update(T item, out T oldItem)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        [Pure]
        private bool CheckVersion(int version)
        {
            if (version == _version) {
                return true;
            }

            // See https://msdn.microsoft.com/library/system.collections.ienumerator.movenext.aspx
            throw new InvalidOperationException(CollectionWasModified);
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
            Ensures(_version == OldValue(_version));

            #endregion

            var version = _version;

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
            Ensures(_version == OldValue(_version));

            #endregion

            var version = _version;

            // Check version at each call to MoveNext() to ensure an exception is thrown even when the enumerator was really finished
            while (CheckVersion(version) & cursor != _last) {
                yield return cursor.Item;
                cursor = cursor.Next;
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

        private void UpdateVersion() => _version++;

        #endregion

        #region Nested Types

        /// <summary>
        ///     Represents an individual cell in the linked list.
        /// </summary>
        [Serializable]
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