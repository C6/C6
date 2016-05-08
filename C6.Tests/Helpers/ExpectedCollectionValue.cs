// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;
using System.Text;

using C6.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class ExpectedCollectionValue<T> : ICollectionValue<T>, IEquatable<ICollectionValue<T>>
    {
        #region Fields

        private readonly Func<T> _chooseFunction;
        private readonly T[] _items;

        #endregion

        #region Constructors

        public ExpectedCollectionValue(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer, bool allowsNull, Func<T> chooseFunction = null)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(items != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(equalityComparer != null, ArgumentMustBeNonNull);

            // All items must be non-null if collection disallows null values
            Requires(allowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);

            #endregion

            // Copy the array instead of referencing it
            _items = items.ToArray();

            EqualityComparer = equalityComparer;
            AllowsNull = allowsNull;
            _chooseFunction = chooseFunction;
        }

        #endregion

        #region Properties

        public virtual bool AllowsNull { get; }

        public virtual int Count => _items.Length;

        public virtual Speed CountSpeed => Speed.Constant;

        public virtual bool HasChoose => _chooseFunction != null;

        public virtual bool IsEmpty => Count == 0;

        public virtual T Choose()
        {
            if (HasChoose) {
                return _chooseFunction();
            }
            throw new NotImplementedException($"Use the {nameof(_chooseFunction)} to define the value of {nameof(Choose)}.");
        }

        #endregion

        #region Methods

        public virtual void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, Count);

        public virtual bool Equals(ICollectionValue<T> other)
        {
            // Prepare CopyTo()
            var padding = 2;
            var expectedArray = new T[Count + 2 * padding];
            CopyTo(expectedArray, padding);
            var actualArray = new T[other.Count + 2 * padding];
            other.CopyTo(actualArray, padding);

            return
                // Properties
                AllowsNull == other.AllowsNull
                && Count == other.Count
                && CountSpeed == other.CountSpeed // TODO: Is this always constant? We would at least like that, right?
                && IsEmpty == other.IsEmpty

                // Pure methods
                && (!HasChoose || Choose().IsSameAs(other.Choose()))
                && expectedArray.SequenceEqual(actualArray, EqualityComparer)
                && this.SequenceEqual(other, EqualityComparer)
                // Show() is tested with ToString()
                && ToArray().SequenceEqual(other.ToArray(), EqualityComparer)
                && ToString().Equals(other.ToString()); // TODO: Should they always return the same result? Couldn't this differ between collection types?
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj == null) {
                return false;
            }
            var that = obj as ICollectionValue<T>;
            return that != null && Equals(that);
        }

        public virtual SCG.IEnumerator<T> GetEnumerator() => ((SCG.IEnumerable<T>) _items).GetEnumerator();

        public override int GetHashCode()
        {
            throw new NotImplementedException($"{nameof(ExpectedCollectionValue<T>)} should only be used in tests and compared directly. Hash code is not supported.");
        }

        public virtual bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider) => Showing.Show(this, stringBuilder, ref rest, formatProvider);

        public virtual T[] ToArray()
        {
            var array = new T[Count];
            Array.Copy(_items, array, Count);
            return array;
        }

        public override string ToString() => ToString(null, null);

        public virtual string ToString(string format, IFormatProvider formatProvider) => Showing.ShowString(this, format, formatProvider);

        #endregion

        #region Explicit Implementations

        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Protected Members

        protected SCG.IEqualityComparer<T> EqualityComparer { get; }

        #endregion

    }
}