// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;


namespace C6.Collections
{
    [Serializable]
    public abstract class CheckedDirectedCollectionValueBase<T> : CheckedCollectionValueBase<T>, IDirectedCollectionValue<T>
    {
        public virtual EnumerationDirection Direction
        {
            get {
                CheckVersion();
                return DirectionProtected;
            }
        }

        protected abstract EnumerationDirection DirectionProtected { get; }

        public virtual IDirectedCollectionValue<T> Backwards()
        {
            CheckVersion();
            return BackwardsProtected();
        }

        protected abstract IDirectedCollectionValue<T> BackwardsProtected();
    }
}