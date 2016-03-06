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
    public class ArrayList<T> : IExtensible<T>
    {
        #region Fields

        private readonly T[] _array;

        #endregion

        #region Code Contracts

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // ReSharper disable InvocationIsSkipped

            // Array is non-null
            Invariant(_array != null);

            // All items must be non-null if collection disallows null values
            Invariant(AllowsNull || ForAll(this, item => item != null));
            
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
            Requires(items != null);

            // All items must be non-null if collection disallows null values
            Requires(allowsNull || ForAll(items, item => item != null));

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull);

            #endregion

            // TODO: Check for null items when copying?
            _array = items.ToArray();
            Count = _array.Length;

            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;

            AllowsNull = allowsNull;
        }

        public ArrayList(int capacity = 8, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
        {
            #region Code Contracts

            // Argument must be within bounds
            Contract.Requires(0 <= capacity);

            // Value types cannot be null
            Requires(!typeof(T).IsValueType || !allowsNull);

            #endregion
            
            _array = new T[capacity];

            EqualityComparer = equalityComparer ?? SCG.EqualityComparer<T>.Default;

            AllowsNull = allowsNull;
        }

        #endregion

        #region Properties

        public EventTypes ActiveEvents
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllowsDuplicates => true;

        public bool AllowsNull { get; }

        public int Count { get; }

        public Speed CountSpeed => Speed.Constant;

        public bool DuplicatesByCounting => false;

        public SCG.IEqualityComparer<T> EqualityComparer { get; }

        public bool IsEmpty => Count == 0;

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public EventTypes ListenableEvents
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Public Methods

        public bool Add(T item)
        {
            throw new NotImplementedException();
        }

        public void AddAll(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

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
            var result = new T[Count];
            Array.Copy(_array, result, Count);
            return result;
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