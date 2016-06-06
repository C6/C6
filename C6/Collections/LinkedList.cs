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
    public class LinkedList<T> : ListenableBase<T>, IExtensible<T>
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

            // List is equal forwards and backwards
            Invariant(EnumerateFrom(_first.Next).IsSameSequenceAs(EnumerateBackwardsFrom(_last.Previous).Reverse()));

            // All items must be non-null if collection disallows null values
            Invariant(AllowsNull || ForAll(this, item => item != null));

            // ReSharper restore InvocationIsSkipped
        }

        #endregion

        #region Constructors

        public LinkedList(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) : base(allowsNull)
        {
            _first = new Node(default(T));
            _last = new Node(default(T), _first);
            _first.Next = _last;

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

        public bool AllowsDuplicates => true;

        public override Speed CountSpeed => Constant;

        public bool DuplicatesByCounting => false;

        public SCG.IEqualityComparer<T> EqualityComparer { get; }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        #endregion

        #region Methods

        public bool Add(T item)
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

        public bool AddRange(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public override T Choose() => _last.Previous.Item;

        public override SCG.IEnumerator<T> GetEnumerator() => EnumerateFrom(_first.Next).GetEnumerator();

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

        Node InsertAfter(T item, Node previous)
        {
            // The incrementation must be before adding the next item, because the incrementation requires a read, which will otherwise violate a contract
            ++Count;
            return new Node(item, previous, previous.Next);
        }

        Node InsertBefore(T item, Node next)
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

            public Node(T item)
            {
                Item = item;
            }

            public Node(T item, Node previous)
            {
                Item = item;

                // Set previous' pointers
                Previous = previous;
                previous.Next = this;
            }

            public Node(T item, Node previous, Node next)
            {
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