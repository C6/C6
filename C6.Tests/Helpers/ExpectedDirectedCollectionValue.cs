// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;
using System.Text;

using static C6.Speed;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class ExpectedDirectedCollectionValue<T> : IDirectedCollectionValue<T>, IEquatable<IDirectedCollectionValue<T>>
    {
        #region Fields

        private readonly SCG.IEqualityComparer<T> _equalityComparer;
        private readonly T[] _items;
        private readonly IExtensible<T> _originalCollection;

        #endregion

        #region Constructors

        public ExpectedDirectedCollectionValue(IExtensible<T> originalCollection, SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer = null, EnumerationDirection direction = EnumerationDirection.Forwards)
        {
            _equalityComparer = equalityComparer ?? originalCollection.EqualityComparer;
            // Copy the array instead of referencing it
            _items = items.ToArray();
            _originalCollection = originalCollection;

            AllowsNull = originalCollection.AllowsNull;
            Direction = direction;
        }

        #endregion

        #region Properties

        public bool AllowsNull { get; }

        public int Count => _items.Length;

        public Speed CountSpeed => Constant;

        public EnumerationDirection Direction { get; }

        public bool IsEmpty => Count == 0;

        #endregion

        #region Methods

        public IDirectedCollectionValue<T> Backwards() => new ExpectedDirectedCollectionValue<T>(
            _originalCollection,
            _items.Reverse(),
            _equalityComparer,
            Direction.Opposite()
        );

        public T Choose()
        {
            throw new NotImplementedException("It is unknown what this value should be.");
        }

        public void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, Count);

        public bool Equals(IDirectedCollectionValue<T> other) => Equals(this, other, _equalityComparer) && Equals(Backwards(), other.Backwards(), _equalityComparer);

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

        public override int GetHashCode()
        {
            throw new NotImplementedException($"{nameof(ExpectedDirectedCollectionValue<T>)} should only be used in tests and compared directly. Hash code is not supported.");
        }

        public bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider) => Showing.Show(this, stringBuilder, ref rest, formatProvider);

        public T[] ToArray()
        {
            var array = new T[Count];
            Array.Copy(_items, array, Count);
            return array;
        }

        public override string ToString() => ToString(null, null);

        public string ToString(string format, IFormatProvider formatProvider) => Showing.ShowString(this, format, formatProvider);

        #endregion

        #region Explicit Implementations

        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Private Members

        private static bool Equals(IDirectedCollectionValue<T> expected, IDirectedCollectionValue<T> actual, SCG.IEqualityComparer<T> equalityComparer)
        {
            // Prepare CopyTo()
            var padding = 2;
            var expectedArray = new T[expected.Count + 2 * padding];
            expected.CopyTo(expectedArray, padding);
            var actualArray = new T[actual.Count + 2 * padding];
            actual.CopyTo(actualArray, padding);

            return
                // Properties
                expected.AllowsNull == actual.AllowsNull
                && expected.Count == actual.Count
                && expected.CountSpeed == actual.CountSpeed // TODO: Is this always constant? We would at least like that, right?
                && expected.Direction == actual.Direction
                && expected.IsEmpty == actual.IsEmpty

                    // Pure methods
                    // TODO: Choose() - this is dependent on the specific implementation. Can we predict it and compare against that value?
                && expectedArray.SequenceEqual(actualArray, equalityComparer)
                && expected.SequenceEqual(actual, equalityComparer)
                    // Show() is tested with ToString()
                && expected.ToArray().SequenceEqual(actual.ToArray(), equalityComparer)
                && expected.ToString().Equals(actual.ToString()) // TODO: Should they always return the same result? Couldn't this differ between collection types?
                ;
        }

        #endregion
    }
}