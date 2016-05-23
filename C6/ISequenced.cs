// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6
{
    /// <summary>
    ///     Represents an editable generic collection that maintains a particular item sequence order.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The sequence order can be determined either by insertion order or item ordering.
    ///     </para>
    ///     <para>
    ///         Implementations of this interface must compute the hash code and equality exactly as prescribed in the method
    ///         definitions in order to be consistent with other collection classes implementing this interface.
    ///     </para>
    /// </remarks>
    [ContractClass(typeof(ISequencedContract<>))]
    public interface ISequenced<T> : ICollection<T>, IDirectedCollectionValue<T>
    {
        /// <summary>
        ///     Returns the sequenced (order-sensitive) hash code of the collection.
        /// </summary>
        /// <returns>
        ///     The sequenced hash code of the collection.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The collection's sequenced hash code is defined as the sum of a transformation of the hash codes of its items,
        ///         each computed using the collection's <see cref="IExtensible{T}.EqualityComparer"/>. The hash code is defined as
        ///         <c>
        ///             h(...h(h(h(x1),x2),x3),...,xn)
        ///         </c>
        ///         for <c>h(a,b)=CONSTANT*a+b</c> and the x's the hash codes of the items of this collection.
        ///     </para>
        ///     <para>
        ///         The implementations must use a fixed transformation that allows serialization and the hash code must be cached
        ///         and thus not recomputed unless the collection has changed since the last call to this method. The hash code
        ///         must be equal to that of
        ///         <c>
        ///             SequencedEqualityComparer.GetSequencedHashCode(collection, collection.EqualityComparer)
        ///         </c>.
        ///     </para>
        /// </remarks>
        [Pure]
        int GetSequencedHashCode();

        /// <summary>
        ///     Compares the items in this collection to the items in the other collection with regards to multiplicities and
        ///     sequence order.
        /// </summary>
        /// <param name="otherCollection">
        ///     The collection to compare this collection to.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the collections contain equal items in the same order; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Enumeration of the collections must yield equal items, place for place. The comparison uses <b>this</b>
        ///         collection's <see cref="IExtensible{T}.EqualityComparer"/> to determine item equality. If the two collections
        ///         use different notions of item equality, there is no guarantee that this method is symmetric, i.e. the following
        ///         test is undetermined:
        ///         <code>
        ///             // Undetermined when coll1.EqualityComparer and coll2.EqualityComparer are not equal
        ///             coll1.SequencedEquals(coll2) == coll2.SequencedEquals(coll1)
        ///         </code>
        ///     </para>
        ///     <para>
        ///         This method is equivalent to <c>
        ///             Enumerable.SequenceEqual(coll1, coll2, coll1.EqualityComparer)
        ///         </c>, but might be more efficient.
        ///     </para>
        /// </remarks>
        /// <seealso cref="GetSequencedHashCode"/>
        /// <seealso cref="Enumerable.SequenceEqual{T}(SCG.IEnumerable{T}, SCG.IEnumerable{T}, SCG.IEqualityComparer{T})"/>
        [Pure]
        bool SequencedEquals(ISequenced<T> otherCollection);
    }


    [ContractClassFor(typeof(ISequenced<>))]
    internal abstract class ISequencedContract<T> : ISequenced<T>
    {
        // ReSharper disable InvocationIsSkipped

        public int GetSequencedHashCode()
        {
            // No preconditions


            // Result is equal to that of SequencedEqualityComparer
            Ensures(Result<int>() == this.GetSequencedHashCode(EqualityComparer));


            return default(int);
        }

        public bool SequencedEquals(ISequenced<T> otherCollection)
        {
            // No preconditions


            // Enumeration of the collections must yield equal items
            Ensures(Result<bool>() == (otherCollection != null && this.SequenceEqual(otherCollection, EqualityComparer)));
            Ensures(Result<bool>() == this.SequencedEquals(otherCollection, EqualityComparer));


            return default(bool);
        }

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        #region SCG.IEnumerable<T>

        public abstract SCG.IEnumerator<T> GetEnumerator();
        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IShowable

        public abstract string ToString(string format, IFormatProvider formatProvider);
        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);

        #endregion

        #region ICollectionValue<T>

        public abstract bool AllowsNull { get; }
        public abstract Speed CountSpeed { get; }
        public abstract bool IsEmpty { get; }
        public abstract T Choose();
        public abstract T[] ToArray();

        #endregion

        #region IListenable<T>

        public abstract EventTypes ActiveEvents { get; }
        public abstract EventTypes ListenableEvents { get; }
        public abstract event EventHandler CollectionChanged;
        public abstract event EventHandler<ClearedEventArgs> CollectionCleared;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemInserted;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsAdded;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsRemoved;

        #endregion

        #region IDirectedCollectionValue<T>

        public abstract EnumerationDirection Direction { get; }
        public abstract IDirectedCollectionValue<T> Backwards();

        #endregion

        #region IExtensible

        public abstract bool AllowsDuplicates { get; }
        public abstract bool DuplicatesByCounting { get; }
        public abstract SCG.IEqualityComparer<T> EqualityComparer { get; }
        public abstract bool IsFixedSize { get; }
        public abstract bool AddRange(SCG.IEnumerable<T> items);

        #endregion

        #region SCG.ICollection<T>

        void SCG.ICollection<T>.Add(T item) {}

        #endregion

        #region ICollection<T>

        public abstract Speed ContainsSpeed { get; }
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }
        public abstract bool Add(T item);
        public abstract void Clear();
        public abstract bool Contains(T item);
        public abstract bool ContainsRange(SCG.IEnumerable<T> items);
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract int CountDuplicates(T item);
        public abstract bool Find(ref T item);
        public abstract ICollectionValue<T> FindDuplicates(T item);
        public abstract bool FindOrAdd(ref T item);
        public abstract int GetUnsequencedHashCode();
        public abstract ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities();
        public abstract bool Remove(T item);
        public abstract bool Remove(T item, out T removedItem);
        public abstract bool RemoveDuplicates(T item);
        public abstract bool RemoveRange(SCG.IEnumerable<T> items);
        public abstract bool RetainRange(SCG.IEnumerable<T> items);
        public abstract ICollectionValue<T> UniqueItems();
        public abstract bool UnsequencedEquals(ICollection<T> otherCollection);
        public abstract bool Update(T item);
        public abstract bool Update(T item, out T oldItem);
        public abstract bool UpdateOrAdd(T item);
        public abstract bool UpdateOrAdd(T item, out T oldItem);

        #endregion

        #endregion
    }
}