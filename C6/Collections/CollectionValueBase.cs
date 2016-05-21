// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Text;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6.Collections
{
    [Serializable]
    public abstract class CollectionValueBase<T> : ICollectionValue<T>
    {
        #region Constructor

        protected CollectionValueBase(bool allowsNull = false)
        {
            AllowsNull = allowsNull;
        }

        #endregion

        #region Properties

        public virtual bool AllowsNull { get; }

        public virtual int Count { get; protected set; }

        public abstract Speed CountSpeed { get; }

        public virtual bool IsEmpty => Count == 0;

        #endregion

        #region Methods

        public abstract T Choose();

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in this) {
                array[arrayIndex++] = item;
            }
        }

        public abstract SCG.IEnumerator<T> GetEnumerator();

        public virtual bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider) => Showing.Show(this, stringBuilder, ref rest, formatProvider);

        public virtual T[] ToArray()
        {
            var array = new T[Count];
            CopyTo(array, 0);
            return array;
        }

        public override string ToString() => ToString(null, null);

        public virtual string ToString(string format, IFormatProvider formatProvider) => Showing.ShowString(this, format, formatProvider);

        #endregion

        #region Explicit Implementations

        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}