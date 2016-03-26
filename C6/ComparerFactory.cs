// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Reflection;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

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
            #region Code Contracts

            // Argument must be non-null
            Requires(compare != null, ArgumentMustBeNonNull);

            // Result is non-null
            Ensures(Result<SCG.IComparer<T>>() != null);

            #endregion

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
        /// Creates a new reference equality comparer that compares equality
        /// based on reference equality and uses the type's default hash
        /// function.
        /// </summary>
        /// <returns>An <see cref="SCG.IEqualityComparer{T}"/> that uses the
        /// specified equals and hash functions.</returns>
        [Pure]
        public static SCG.IEqualityComparer<T> CreateReferenceEqualityComparer<T>()
        {
            #region Code Contracts

            // Result is non-null
            Ensures(Result<SCG.IEqualityComparer<T>>() != null);

            #endregion

            return new EqualityComparer<T>((x, y) => ReferenceEquals(x, y), SCG.EqualityComparer<T>.Default.GetHashCode);
        }

        public static SCG.IEqualityComparer<T> CreateStructComparer<T>() => CreateEqualityComparer<T>(StructEquals, type => type.GetHashCode());

        private static bool StructEquals<T>(T x, T y)
        {
            var type = typeof(T);

            // Only allow structs
            if (!type.IsValueType || type.IsPrimitive) {
                throw new ArgumentException($"{nameof(T)} must be a struct.");
            }

            // If the structs don't consider themselves equal, just return false
            if (!x.Equals(y)) {
                return false;
            }

            var thisFieldsInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var fieldInfo in thisFieldsInfos) {
                // Get field values
                var thisObject = fieldInfo.GetValue(x);
                var thatObject = fieldInfo.GetValue(y);

                var fieldType = fieldInfo.FieldType;

                if (thisObject == null) {
                    if (thatObject != null) {
                        return false;
                    }
                }
                else if (fieldType.IsClass) {
                    // Compare reference equality for objects
                    if (!ReferenceEquals(thisObject, thatObject)) {
                        return false;
                    }
                }
                else if (fieldType.IsPrimitive) {
                    // Compare values for simple types
                    if (!thisObject.Equals(thatObject)) {
                        return false;
                    }
                }
                else {
                    // Call method recursively for structs - we invoke a generic method, as variables are objects, and therefore not typed
                    var methodInfo = typeof(ComparerFactory).GetMethod(nameof(StructEquals), BindingFlags.Static | BindingFlags.NonPublic);
                    var genericMethod = methodInfo.MakeGenericMethod(fieldType);
                    var structEquals = (bool) genericMethod.Invoke(null, new[] { thisObject, thatObject });
                    if (!structEquals) {
                        return false;
                    }
                }
            }

            return true;
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