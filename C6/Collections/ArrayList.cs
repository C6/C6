﻿// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Linq;

using SCG = System.Collections.Generic;


namespace C6
{
    public class ArrayList<T> : ICollectionValue<T>
    {
        #region Fields

        private readonly T[] _array;

        #endregion

        #region Constructors

        public ArrayList() : this(Enumerable.Empty<T>()) {}

        public ArrayList(SCG.IEnumerable<T> enumerable)
        {
            _array = enumerable.ToArray();
            Count = _array.Length;
        }

        #endregion

        #region Properties

        public EventTypes ActiveEvents
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllowsNull
        {
            get { throw new NotImplementedException(); }
        }

        public int Count { get; }

        public Speed CountSpeed => Speed.Constant;

        public bool IsEmpty => Count == 0;

        public EventTypes ListenableEvents
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Public Methods

        public SCG.IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++) {
                yield return _array[i];
            }
        }

        public T Choose()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
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