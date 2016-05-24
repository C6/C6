// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using static System.Reflection.BindingFlags;
using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6.Contracts
{
    public static class ContractHelperExtensions
    {
        /// <summary>
        ///     Returns all elements in an enumerable, except the element at the specified index.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements of <paramref name="enumerable"/>.
        /// </typeparam>
        /// <param name="enumerable">
        ///     An <see cref="SCG.IEnumerable{T}"/> to return elements from.
        /// </param>
        /// <param name="index">
        ///     The zero-based index of the element that is skipped.
        /// </param>
        /// <returns>
        ///     An <see cref="SCG.IEnumerable{T}"/> containing the elements from the specified <see cref="SCG.IEnumerable{T}"/>,
        ///     but without the element at the specified index.
        /// </returns>
        /// <remarks>
        ///     This is only intended for code contracts, and is not optimal in any sense.
        /// </remarks>
        [Pure]
        public static SCG.IEnumerable<T> SkipIndex<T>(this SCG.IEnumerable<T> enumerable, int index) => enumerable.SkipRange(index, 1);

        /// <summary>
        ///     Returns a specified number of contiguous elements from the start of a sequence until index
        ///     <paramref name="startIndex"/>, then bypasses the next <paramref name="count"/> elements in the sequence and then
        ///     returns the remaining elements.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements of <paramref name="enumerable"/>.
        /// </typeparam>
        /// <param name="enumerable">
        ///     An <see cref="SCG.IEnumerable{T}"/> to return elements from.
        /// </param>
        /// <param name="startIndex">
        ///     The number of elements to return before skipping the next <paramref name="count"/> elements.
        /// </param>
        /// <param name="count">
        ///     The number of elements to skip before returning the remaining elements.
        /// </param>
        /// <returns>
        ///     An <see cref="SCG.IEnumerable{T}"/> containing the elements from the specified <see cref="SCG.IEnumerable{T}"/>,
        ///     but with a range of items skipped.
        /// </returns>
        /// <remarks>
        ///     This is only intended for code contracts, and is not optimal in any sense.
        /// </remarks>
        [Pure]
        public static SCG.IEnumerable<T> SkipRange<T>(this SCG.IEnumerable<T> enumerable, int startIndex, int count)
        {
            #region Code Contracts

            // Argument is non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            #endregion

            // ReSharper disable PossibleMultipleEnumeration
            return enumerable.Take(startIndex).Concat(enumerable.Skip(startIndex + count));
            // ReSharper enable PossibleMultipleEnumeration
        }

        /// <summary>
        ///     Determines whether two enumerables contain the same elements in regards to multiplicity, but not sequence order,
        ///     using the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements of the input sequences.
        /// </typeparam>
        /// <param name="first">
        ///     An <see cref="SCG.IEnumerable{T}"/> to compare to <paramref name="second"/>.
        /// </param>
        /// <param name="second">
        ///     An <see cref="SCG.IEnumerable{T}"/> to compare to <paramref name="first"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the enumerables contain equal items; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool UnsequenceEqual<T>(this SCG.IEnumerable<T> first, SCG.IEnumerable<T> second) => UnsequenceEqual(first, second, SCG.EqualityComparer<T>.Default);

        /// <summary>
        ///     Determines whether two enumerables contain the same elements in regards to multiplicity, but not sequence order,
        ///     using a specified <see cref="SCG.IEqualityComparer{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements of the input sequences.
        /// </typeparam>
        /// <param name="first">
        ///     An <see cref="SCG.IEnumerable{T}"/> to compare to <paramref name="second"/>.
        /// </param>
        /// <param name="second">
        ///     An <see cref="SCG.IEnumerable{T}"/> to compare to <paramref name="first"/>.
        /// </param>
        /// <param name="comparer">
        ///     An <see cref="SCG.IEqualityComparer{T}"/> to use to compare elements.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the enumerables contain equal items; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool UnsequenceEqual<T>(this SCG.IEnumerable<T> first, SCG.IEnumerable<T> second, SCG.IEqualityComparer<T> comparer)
        {
            #region Code Contracts

            // first remains unchanged
            Ensures(first == null || first.IsSameSequenceAs(OldValue(first.ToList())));

            // second remains unchanged
            Ensures(second == null || second.IsSameSequenceAs(OldValue(second.ToList())));

            #endregion

            if (ReferenceEquals(first, second)) {
                return true;
            }

            if (first == null || second == null) {
                return false;
            }

            var firstArray = first.ToArray();
            var secondArray = second.ToArray();

            if (firstArray.Length != secondArray.Length) {
                return false;
            }

            // Use default comparer if none is supplied
            if (comparer == null) {
                comparer = SCG.EqualityComparer<T>.Default;
            }

            // Sort based on hash code
            Comparison<T> hashCodeComparison = (x, y) => comparer.GetHashCode(x).CompareTo(comparer.GetHashCode(y));
            Array.Sort(firstArray, hashCodeComparison);
            Array.Sort(secondArray, hashCodeComparison);

            for (var i = 0; i < firstArray.Length; i++) {
                var found = false;
                var firstElement = firstArray[i];

                for (var j = i; j < secondArray.Length; j++) {
                    var secondElement = secondArray[j];

                    if (hashCodeComparison(firstElement, secondElement) != 0) {
                        break;
                    }

                    if (comparer.Equals(firstElement, secondElement)) {
                        secondArray.Swap(i, j);

                        // Continue with next element in first array
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    return false;
                }
            }

            return true;
        }

        [Pure]
        public static int CountDuplicates<T>(this SCG.IEnumerable<T> enumerable, T item, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);


            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }
            return enumerable.Count(x => equalityComparer.Equals(x, item));
        }

        // TODO: Check that references do check DuplicatesByCounting first!
        [Pure]
        public static bool ContainsSame<T>(this SCG.IEnumerable<T> enumerable, T item)
        {
            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            return enumerable.Contains(item, GetIdenticalityComparer<T>());
        }

        [Pure]
        public static bool ContainsSameRange<T>(this SCG.IEnumerable<T> enumerable, SCG.IEnumerable<T> otherEnumerable)
            => enumerable.ContainsRange(otherEnumerable, GetIdenticalityComparer<T>());

        // TODO: Should work, but could still use some attention
        [Pure]
        public static bool ContainsRange<T>(this SCG.IEnumerable<T> first, SCG.IEnumerable<T> second, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(first != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(second != null, ArgumentMustBeNonNull);


            // first remains unchanged
            Ensures(first == null || first.IsSameSequenceAs(OldValue(first.ToList())));

            // second remains unchanged
            Ensures(second == null || second.IsSameSequenceAs(OldValue(second.ToList())));

            #endregion

            var firstArray = first.ToArray();
            var secondArray = second.ToArray();

            if (firstArray.Length < secondArray.Length) {
                return false;
            }

            // Use default comparer if none is supplied
            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }

            // Sort based on hash code
            Comparison<T> hashCodeComparison = (x, y) => equalityComparer.GetHashCode(x).CompareTo(equalityComparer.GetHashCode(y));
            Array.Sort(firstArray, hashCodeComparison);
            Array.Sort(secondArray, hashCodeComparison);

            for (var j = 0; j < secondArray.Length; j++) {
                var found = false;
                var secondElement = secondArray[j];

                for (var i = j; i < firstArray.Length; i++) {
                    var firstElement = firstArray[i];

                    var comparison = hashCodeComparison(firstElement, secondElement);

                    // Equal doesn't exist
                    if (comparison > 0) {
                        break;
                    }

                    if (comparison == 0 && equalityComparer.Equals(firstElement, secondElement)) {
                        firstArray.Swap(i, j);
                        // TODO: the hash codes are not necessarily ordered after swapping the items

                        found = true;
                        break;
                    }
                }

                if (!found) {
                    return false;
                }

                // Invariant: all items up to and including j are equal pairwise in the two arrays
                Assume(ForAll(0, j + 1, i => equalityComparer.Equals(firstArray[i], secondArray[i])));
            }

            return true;
        }

        [Pure]
        public static int ContainsSameCount<T>(this SCG.IEnumerable<T> enumerable, T item)
        {
            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            return enumerable.CountDuplicates(item, GetIdenticalityComparer<T>());
        }

        [Pure]
        public static bool Invoke(Func<bool> function) => function.Invoke();

        [Pure]
        public static bool IsSameAs<T>(this T item, T otherItem) => GetIdenticalityComparer<T>().Equals(item, otherItem);

        [Pure]
        public static bool IsSameSequenceAs<T>(this SCG.IEnumerable<T> enumerable, SCG.IEnumerable<T> otherEnumerable)
            => enumerable.SequenceEqual(otherEnumerable, GetIdenticalityComparer<T>());

        [Pure]
        public static bool HasSameAs<T>(this SCG.IEnumerable<T> enumerable, SCG.IEnumerable<T> otherEnumerable)
            => enumerable.UnsequenceEqual(otherEnumerable, GetIdenticalityComparer<T>());

        /// <summary>
        ///     Returns an <see cref="SCG.IEqualityComparer{T}"/> that checks if the objects are the same, possibly bypassing the
        ///     type <typeparamref name="T"/>'s own <see cref="object.Equals(object)"/> method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     An <see cref="SCG.IEqualityComparer{T}"/> that checks if objects are the same.
        /// </returns>
        /// <remarks>
        ///     Primitive types are compared using the default equality comparer for the type. Structs are compared using
        ///     reflection to compare each field. Objects are compared using reference equality.
        /// </remarks>
        [Pure]
        private static SCG.IEqualityComparer<T> GetIdenticalityComparer<T>()
        {
            if (!typeof(T).IsValueType) {
                return ComparerFactory.CreateReferenceEqualityComparer<T>();
            }
            if (typeof(T).IsPrimitive) {
                return SCG.EqualityComparer<T>.Default;
            }
            return CreateStructComparer<T>();
        }


        [Pure]
        public static SCG.IEqualityComparer<T> CreateStructComparer<T>() => ComparerFactory.CreateEqualityComparer<T>(StructEquals, type => type.GetHashCode());

        [Pure]
        private static bool StructEquals<T>(T x, T y)
        {
            #region Code Contracts

            // Type must be struct
            Requires(typeof(T).IsValueType && !typeof(T).IsPrimitive, TypeMustBeStruct);

            // Argument must be non-null
            Requires(x != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(y != null, ArgumentMustBeNonNull);

            #endregion

            foreach (var fieldInfo in typeof(T).GetFields(Instance | Public | NonPublic)) {
                // Get field values
                object thisObject = fieldInfo.GetValue(x), thatObject = fieldInfo.GetValue(y);
                var fieldType = fieldInfo.FieldType;

                if (fieldType.IsClass) {
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
                    // Call method recursively for structs 
                    // We make generic method, because variables are non-typed objects
                    var methodInfo = typeof(ContractHelperExtensions).GetMethod(nameof(StructEquals), Static | NonPublic);
                    var genericMethod = methodInfo.MakeGenericMethod(fieldType);
                    var structEquals = (bool) genericMethod.Invoke(null, new[] { thisObject, thatObject });
                    if (!structEquals) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}