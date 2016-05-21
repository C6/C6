// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class ExpectedDirectedCollectionValue<T> : ExpectedCollectionValue<T>, IDirectedCollectionValue<T>, IEquatable<IDirectedCollectionValue<T>>
    {
        #region Constructors

        public ExpectedDirectedCollectionValue(SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer, bool allowsNull, Func<T> chooseFunction = null, EnumerationDirection direction = EnumerationDirection.Forwards)
            : base(items, equalityComparer, allowsNull, chooseFunction)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(items != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(equalityComparer != null, ArgumentMustBeNonNull);

            // All items must be non-null if collection disallows null values
            Requires(allowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);

            // Argument must be valid enum constant
            Requires(Enum.IsDefined(typeof(EnumerationDirection), direction), EnumMustBeDefined);

            #endregion

            Direction = direction;
        }

        #endregion

        #region Properties

        public EnumerationDirection Direction { get; }

        #endregion

        #region Methods

        public IDirectedCollectionValue<T> Backwards() => new ExpectedDirectedCollectionValue<T>(
            this.Reverse(),
            EqualityComparer,
            AllowsNull,
            HasChoose ? Choose : (Func<T>) null,
            Direction.Opposite()
            );

        public bool Equals(IDirectedCollectionValue<T> other)
            => Equals(this, other)
               && Equals((ExpectedDirectedCollectionValue<T>) Backwards(), other.Backwards());

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

        public override int GetHashCode() => base.GetHashCode();

        #endregion

        #region Private Members

        private static bool Equals(ExpectedDirectedCollectionValue<T> expected, IDirectedCollectionValue<T> actual)
            => ((ExpectedCollectionValue<T>) expected).Equals(actual)
               && expected.Direction == actual.Direction;

        #endregion
    }
}