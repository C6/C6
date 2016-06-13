// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using SCG = System.Collections.Generic;

using static C6.Collections.ExceptionMessages;


namespace C6.Collections
{
    [Serializable]
    public abstract class ExtensibleBase<T> : ListenableBase<T>, IExtensible<T>
    {
        #region Properties

        public abstract bool AllowsDuplicates { get; }

        public abstract bool DuplicatesByCounting { get; }

        public abstract SCG.IEqualityComparer<T> EqualityComparer { get; }

        public abstract bool IsFixedSize { get; }

        public abstract bool IsReadOnly { get; }

        #endregion

        #region Methods

        public abstract bool Add(T item);

        public abstract bool AddRange(SCG.IEnumerable<T> items);

        #endregion

        #region Protected Properties

        protected virtual int Version
        {
            [Pure]
            get;
            private set;
        }

        #endregion

        #region Protected Methods

        [Pure]
        protected bool CheckVersion(int version)
        {
            if (version == Version) {
                return true;
            }

            // See https://msdn.microsoft.com/library/system.collections.ienumerator.movenext.aspx
            throw new InvalidOperationException(CollectionWasModified);
        }

        protected virtual void UpdateVersion() => Version++;

        #endregion
    }
}