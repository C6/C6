// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using static C6.EnumerationDirection;


namespace C6.Collections
{
    public abstract class SequenceBase<T> : CollectionBase<T>, ISequenced<T>
    {
        #region Fields

        private int _sequencedHashCode;

        #endregion

        #region Properties

        public virtual EnumerationDirection Direction => Forwards;

        #endregion

        #region Methods

        public abstract IDirectedCollectionValue<T> Backwards();

        // TODO: Update hash code when items are added, if the hash code version is not equal to -1
        public virtual int GetSequencedHashCode()
        {
            if (SequencedHashCodeVersion != Version) {
                SequencedHashCodeVersion = Version;
                _sequencedHashCode = this.GetSequencedHashCode(EqualityComparer);
            }

            return _sequencedHashCode;
        }
        
        public virtual bool SequencedEquals(ISequenced<T> otherCollection) => this.SequencedEquals(otherCollection, EqualityComparer);

        #endregion

        #region Protected Properties

        protected int SequencedHashCodeVersion { get; set; } = -1;

        #endregion
    }
}