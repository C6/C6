// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using SCG = System.Collections.Generic;


namespace C6.Collections
{
    [Serializable]
    public abstract class CollectionBase<T> : ExtensibleBase<T>, ICollection<T>
    {
        #region Properties

        public abstract Speed ContainsSpeed { get; }

        #endregion

        #region Methods

        public abstract void Clear();

        public abstract bool Contains(T item);

        public abstract bool ContainsRange(SCG.IEnumerable<T> items);

        public abstract int CountDuplicates(T item);

        public abstract bool Find(ref T item);

        public abstract ICollectionValue<T> FindDuplicates(T item);

        public virtual bool FindOrAdd(ref T item)
        {
            if (Find(ref item)) {
                return true;
            }

            Add(item);
            return false;
        }

        public abstract int GetUnsequencedHashCode();

        public abstract ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities();

        public virtual bool Remove(T item)
        {
            T removedItem;
            return Remove(item, out removedItem);
        }

        public abstract bool Remove(T item, out T removedItem);

        public abstract bool RemoveDuplicates(T item);

        public abstract bool RemoveRange(SCG.IEnumerable<T> items);

        public abstract bool RetainRange(SCG.IEnumerable<T> items);

        public abstract ICollectionValue<T> UniqueItems();
        
        public virtual bool UnsequencedEquals(ICollection<T> otherCollection) => this.UnsequencedEquals(otherCollection, EqualityComparer);

        public virtual bool Update(T item)
        {
            T oldItem;
            return Update(item, out oldItem);
        }

        public abstract bool Update(T item, out T oldItem);

        public virtual bool UpdateOrAdd(T item)
        {
            T oldItem;
            return UpdateOrAdd(item, out oldItem);
        }

        public virtual bool UpdateOrAdd(T item, out T oldItem)
        {
            if (Update(item, out oldItem)) {
                return true;
            }

            Add(item);
            return false;
        }

        #endregion

        #region Explicit Implementations

        void SCG.ICollection<T>.Add(T item) => Add(item);

        void SCG.ICollection<T>.Clear() => Clear();

        bool SCG.ICollection<T>.Contains(T item) => Contains(item);

        bool SCG.ICollection<T>.Remove(T item) => Remove(item);

        #endregion
    }
}