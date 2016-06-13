// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using static C6.EnumerationDirection;


namespace C6.Collections
{
    public abstract class SequenceBase<T> : CollectionBase<T>, ISequenced<T>
    {
        public virtual EnumerationDirection Direction => Forwards;

        public abstract IDirectedCollectionValue<T> Backwards();

        public virtual int GetSequencedHashCode()
        {
            throw new NotImplementedException();
        }

        public virtual bool SequencedEquals(ISequenced<T> otherCollection)
        {
            throw new NotImplementedException();
        }
    }
}