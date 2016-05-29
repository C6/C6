// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Collections;
using C6.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class ExpectedCollectionValue<T> : CollectionValueBase<T>, IEquatable<ICollectionValue<T>>
    {
        #region Fields

        private readonly Func<T> _chooseFunction;
        private readonly T[] _items;
        private readonly bool _sequenced;

        #endregion

        #region Constructors

        public ExpectedCollectionValue(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer, bool allowsNull, Func<T> chooseFunction = null, bool sequenced = true) : base(allowsNull)
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
            _chooseFunction = chooseFunction;
            _sequenced = sequenced;
        }

        #endregion

        #region Properties

        public override int Count => _items.Length;

        public override Speed CountSpeed => Speed.Constant; // TODO: Is this always constant? We would at least like that, right?

        public virtual bool HasChoose => _chooseFunction != null;

        public override T Choose()
        {
            if (HasChoose) {
                return _chooseFunction();
            }
            throw new NotSupportedException($"Use the {nameof(_chooseFunction)} to define the value of {nameof(Choose)}.");
        }

        #endregion

        #region Methods

        public override void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, Count);
        
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
                && CountSpeed == other.CountSpeed
                && IsEmpty == other.IsEmpty

                    // Pure methods
                && (!HasChoose || Choose().IsSameAs(other.Choose()))
                && (_sequenced ? expectedArray.SequenceEqual(actualArray, EqualityComparer) : expectedArray.UnsequenceEqual(actualArray, EqualityComparer))
                && (_sequenced ? this.SequenceEqual(other, EqualityComparer) : this.UnsequenceEqual(other, EqualityComparer))
                    // Show() is tested with ToString()
                && (_sequenced ? ToArray().SequenceEqual(other.ToArray(), EqualityComparer) : ToArray().UnsequenceEqual(other.ToArray(), EqualityComparer))
                && (!_sequenced || ToString().Equals(other.ToString())); // TODO: Should they always return the same result? Couldn't this differ between collection types?
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

        public override SCG.IEnumerator<T> GetEnumerator() => ((SCG.IEnumerable<T>) _items).GetEnumerator();

        public override int GetHashCode()
        {
            throw new NotSupportedException($"{nameof(ExpectedCollectionValue<T>)} should only be used in tests and compared directly. Hash code is not supported.");
        }

        #endregion

        #region Protected Members

        protected SCG.IEqualityComparer<T> EqualityComparer { get; }

        #endregion
    }
}