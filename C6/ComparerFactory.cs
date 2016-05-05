// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6
{
    /// <summary>
    ///     Factory class to create <see cref="SCG.IComparer{T}"/> and <see cref="SCG.IEqualityComparer{T}"/> using delegate
    ///     functions.
    /// </summary>
    public static class ComparerFactory
    {
        /// <summary>
        ///     Creates a new <see cref="SCG.IComparer{T}"/> using a specified compare function.
        /// </summary>
        /// <param name="compare">The compare function.</param>
        /// <typeparam name="T">
        ///     The type of the objects to compare.
        /// </typeparam>
        /// <returns>
        ///     An <see cref="SCG.IComparer{T}"/> that uses the specified compare function.
        /// </returns>
        [Pure]
        public static SCG.IComparer<T> CreateComparer<T>(Func<T, T, int> compare)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(compare != null, ArgumentMustBeNonNull);

            // Result is non-null
            Ensures(Result<SCG.IComparer<T>>() != null);

            #endregion

            return new Comparer<T>(compare);
        }

        /// <summary>
        ///     Creates a new <see cref="SCG.IComparer{T}"/> using a specified comparison delegate.
        /// </summary>
        /// <param name="comparison">The comparison delegate.</param>
        /// <typeparam name="T">
        ///     The type of the objects to compare.
        /// </typeparam>
        /// <returns>
        ///     An <see cref="SCG.IComparer{T}"/> that uses the specified comparison delegate.
        /// </returns>
        [Pure]
        public static SCG.IComparer<T> AsComparer<T>(this Comparison<T> comparison)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(comparison != null, ArgumentMustBeNonNull);

            // Result is non-null
            Ensures(Result<SCG.IComparer<T>>() != null);

            #endregion

            return new Comparer<T>(comparison);
        }

        /// <summary>
        ///     Creates a new equality comparer using the specified functions.
        /// </summary>
        /// <param name="equals">The equals function.</param>
        /// <param name="getHashCode">The hash function.</param>
        /// <returns>
        ///     An <see cref="SCG.IEqualityComparer{T}"/> that uses the specified equals and hash functions.
        /// </returns>
        [Pure]
        public static SCG.IEqualityComparer<T> CreateEqualityComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(equals != null, ArgumentMustBeNonNull);
            // Argument must be non-null
            Requires(getHashCode != null, ArgumentMustBeNonNull);

            // Result is non-null
            Ensures(Result<SCG.IEqualityComparer<T>>() != null);

            #endregion

            return new EqualityComparer<T>(equals, getHashCode);
        }

        // TODO: Overload with a custom hash function
        /// <summary>
        ///     Creates a new reference equality comparer that compares equality based on reference equality and uses the type's
        ///     default hash function.
        /// </summary>
        /// <returns>
        ///     An <see cref="SCG.IEqualityComparer{T}"/> that uses the specified equals and hash functions.
        /// </returns>
        [Pure]
        public static SCG.IEqualityComparer<T> CreateReferenceEqualityComparer<T>()
        {
            #region Code Contracts

            // Result is non-null
            Ensures(Result<SCG.IEqualityComparer<T>>() != null);

            #endregion

            return new EqualityComparer<T>((x, y) => ReferenceEquals(x, y), SCG.EqualityComparer<T>.Default.GetHashCode);
        }

        #region Nested Types

        [Serializable]
        private class Comparer<T> : SCG.IComparer<T>
        {
            private readonly Func<T, T, int> _compare;

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                // ReSharper disable InvocationIsSkipped

                Invariant(_compare != null);

                // ReSharper enable InvocationIsSkipped
            }

            public Comparer(Func<T, T, int> compare)
            {
                #region Code Contracts

                // Argument must be non-null
                Requires(compare != null, ArgumentMustBeNonNull);

                #endregion

                _compare = compare;
            }

            public Comparer(Comparison<T> comparison)
            {
                #region Code Contracts

                // Argument must be non-null
                Requires(comparison != null, ArgumentMustBeNonNull);

                #endregion

                _compare = comparison.Invoke;
            }

            public int Compare(T x, T y) => _compare(x, y);
        }


        [Serializable]
        private class EqualityComparer<T> : SCG.IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _equals;
            private readonly Func<T, int> _getHashCode;

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                // ReSharper disable InvocationIsSkipped

                Invariant(_equals != null);
                Invariant(_getHashCode != null);

                // ReSharper enable InvocationIsSkipped
            }

            public EqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
            {
                #region Code Contracts

                // Argument must be non-null
                Requires(equals != null, ArgumentMustBeNonNull);

                // Argument must be non-null
                Requires(getHashCode != null, ArgumentMustBeNonNull);

                #endregion

                _equals = equals;
                _getHashCode = getHashCode;
            }

            public bool Equals(T x, T y) => _equals(x, y);

            public int GetHashCode(T obj) => _getHashCode(obj);
        }

        #endregion
    }
}