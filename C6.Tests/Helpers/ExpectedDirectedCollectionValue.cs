// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;
using System.Text;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class ExpectedDirectedCollectionValue<T> : IDirectedCollectionValue<T>, IEquatable<IDirectedCollectionValue<T>>
    {
        #region Fields

        private readonly T[] _items;
        //private readonly T _chooseItem;
        private readonly SCG.IEqualityComparer<T> _equalityComparer;

        #endregion

        #region Constructors

        public ExpectedDirectedCollectionValue(IExtensible<T> originalCollection, SCG.IEnumerable<T> items, EnumerationDirection direction, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            // Copy the array instead of referencing it
            _items = items.ToArray();

            AllowsNull = originalCollection.AllowsNull;
            Direction = direction;

            /*Func<T> chooseItem = null,
            _chooseItem =
                Count == 0
                    ? default(T)
                    : chooseItem != null
                        ? chooseItem.Invoke()
                        : (direction.IsForward()
                            ? _items[Count - 1]
                            : _items[0]);*/
            _equalityComparer = equalityComparer ?? originalCollection.EqualityComparer;
        }

        #endregion

        #region Properties

        public bool AllowsNull { get; }

        public int Count => _items.Length;

        public Speed CountSpeed
        {
            get { throw new NotImplementedException(); }
        }

        public EnumerationDirection Direction { get; }

        public bool IsEmpty => Count == 0;

        #endregion

        #region Methods

        public IDirectedCollectionValue<T> Backwards()
        {
            throw new NotImplementedException();
        }

        public T Choose()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDirectedCollectionValue<T> other)
        {
            return
                other != null
                && AllowsNull == other.AllowsNull
                && Count == other.Count
                    //&& CountSpeed == other.CountSpeed // TODO: How do we handle this?
                && Direction == other.Direction
                && _items.SequenceEqual(other, _equalityComparer)
                ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj == null) {
                return false;
            }
            var that = obj as IDirectedCollectionValue<T>;
            return that != null && Equals(that);
        }

        public SCG.IEnumerator<T> GetEnumerator() => ((SCG.IEnumerable<T>) _items).GetEnumerator();

        public bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public T[] ToArray()
        {
            throw new NotImplementedException();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Explicit Implementations

        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}