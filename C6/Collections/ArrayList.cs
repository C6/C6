// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using SCG = System.Collections.Generic;


namespace C6
{
    public class ArrayList<T> : ICollectionValue<T>
    {
        #region Fields

        private readonly T[] _array;

        #endregion

        #region Code Contracts

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // ReSharper disable InvocationIsSkipped

            Invariant(_array != null);

            Invariant(AllowsNull || ForAll(this, item => item != null));

            // ReSharper restore InvocationIsSkipped
        }

        #endregion

        #region Constructors

        // TODO: Document
        public ArrayList() : this(false) {}

        // TODO: Document
        public ArrayList(bool allowsNull) : this(Enumerable.Empty<T>(), allowsNull)
        {
            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull);
        }

        // TODO: Document
        public ArrayList(SCG.IEnumerable<T> items) : this(items, false)
        {
            // Argument must be non-null
            Requires(items != null); // TODO: Use <ArgumentNullException>?
        }

        // TODO: Document
        public ArrayList(SCG.IEnumerable<T> items, bool allowsNull)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(items != null); // TODO: Use <ArgumentNullException>?

            // All items must be non-null if collection disallows null values
            Requires(allowsNull || ForAll(items, item => item != null)); // TODO: Use <ArgumentNullException>?

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull);

            #endregion

            _array = items.ToArray();
            Count = _array.Length;

            AllowsNull = allowsNull;
        }

        #endregion

        #region Properties

        public EventTypes ActiveEvents
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllowsNull { get; }

        public int Count { get; }

        public Speed CountSpeed => Speed.Constant;

        public bool IsEmpty => Count == 0;

        public EventTypes ListenableEvents
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Public Methods

        public T Choose() => _array[Count - 1];

        public void CopyTo(T[] array, int arrayIndex)
            => Array.Copy(_array, 0, array, arrayIndex, Count);

        public SCG.IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++) {
                yield return _array[i];
            }
        }

        public T[] ToArray()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        public event EventHandler CollectionChanged;
        public event EventHandler<ClearedEventArgs> CollectionCleared;
        public event EventHandler<ItemAtEventArgs<T>> ItemInserted;
        public event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;
        public event EventHandler<ItemCountEventArgs<T>> ItemsAdded;
        public event EventHandler<ItemCountEventArgs<T>> ItemsRemoved;

        #endregion

        #region Explicit Implementations

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}