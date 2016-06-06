// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections.Generic;


namespace C6.Collections
{
    [Serializable]
    public abstract class ExtensibleBase<T> : ListenableBase<T>, IExtensible<T>
    {
        #region Properties

        public abstract bool AllowsDuplicates { get; }

        public abstract bool DuplicatesByCounting { get; }

        public abstract IEqualityComparer<T> EqualityComparer { get; }

        public abstract bool IsFixedSize { get; }

        public abstract bool IsReadOnly { get; }

        #endregion

        #region Methods

        public abstract bool Add(T item);

        public abstract bool AddRange(IEnumerable<T> items);

        #endregion
    }
}