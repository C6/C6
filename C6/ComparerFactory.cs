// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System;
using System.Diagnostics.Contracts;

using SCG = System.Collections.Generic;


namespace C6
{
    /// <summary>
    /// Factory class to create <see cref="SCG.IComparer{T}"/> and
    /// <see cref="SCG.IEqualityComparer{T}"/> using delegate functions.
    /// </summary>
    public static class ComparerFactory
    {
        /// <summary>
        /// Create a new <see cref="SCG.IComparer{T}"/> using the specified
        /// compare function.
        /// </summary>
        /// <param name="compare">The compare function.</param>
        /// <typeparam name="T">The type of the objects to compare.</typeparam>
        /// <returns>An <see cref="SCG.IComparer{T}"/> that uses the specified
        /// compare function.</returns>
        [Pure]
        public static SCG.IComparer<T> CreateComparer<T>(Func<T, T, int> compare)
        {
            // Argument must be non-null
            Contract.Requires(compare != null);

            // Result is non-null
            Contract.Ensures(Contract.Result<SCG.IComparer<T>>() != null);


            return new Comparer<T>(compare);
        }


        /// <summary>
        /// Creates a new equality comparer using the specified functions.
        /// </summary>
        /// <param name="equals">The equals function.</param>
        /// <param name="getHashCode">The hash function.</param>
        /// <returns>An <see cref="SCG.IEqualityComparer{T}"/> that uses the
        /// specified equals and hash functions.</returns>
        [Pure]
        public static SCG.IEqualityComparer<T> CreateEqualityComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            // Argument must be non-null
            Contract.Requires(equals != null);
            // Argument must be non-null
            Contract.Requires(getHashCode != null);

            // Result is non-null
            Contract.Ensures(Contract.Result<SCG.IEqualityComparer<T>>() != null);


            return new EqualityComparer<T>(equals, getHashCode);
        }


        // TODO: Overload with a custom hash function
        /// <summary>
        /// Creates a new reference equality comparer that compares equality
        /// based on reference equality and uses the type's default hash
        /// function.
        /// </summary>
        /// <returns>An <see cref="SCG.IEqualityComparer{T}"/> that uses the
        /// specified equals and hash functions.</returns>
        [Pure]
        public static SCG.IEqualityComparer<T> CreateReferenceEqualityComparer<T>()
        {
            // Result is non-null
            Contract.Ensures(Contract.Result<SCG.IEqualityComparer<T>>() != null);


            return new EqualityComparer<T>((x, y) => ReferenceEquals(x, y), SCG.EqualityComparer<T>.Default.GetHashCode);
        }


        #region Nested Types

        [Serializable]
        private class Comparer<T> : SCG.IComparer<T>
        {
            private readonly Func<T, T, int> _compare;


            [ContractInvariantMethod]
            private void Invariants()
            {
                Contract.Invariant(_compare != null);
            }


            public Comparer(Func<T, T, int> compare)
            {
                // Argument must be non-null
                Contract.Requires(compare != null);

                _compare = compare;
            }


            public int Compare(T x, T y) => _compare(x, y);
        }



        [Serializable]
        private class EqualityComparer<T> : SCG.IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _equals;
            private readonly Func<T, int> _getHashCode;


            [ContractInvariantMethod]
            private void Invariants()
            {
                Contract.Invariant(_equals != null);
                Contract.Invariant(_getHashCode != null);
            }


            public EqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
            {
                // Argument must be non-null
                Contract.Requires(equals != null);

                // Argument must be non-null
                Contract.Requires(getHashCode != null);

                _equals = equals;
                _getHashCode = getHashCode;
            }


            public bool Equals(T x, T y) => _equals(x, y);


            public int GetHashCode(T obj) => _getHashCode(obj);
        }

        #endregion
    }
}
