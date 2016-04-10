// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Reflection.BindingFlags;

using SCG = System.Collections.Generic;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;


namespace C6.Contracts
{
    // TODO: Should this be internal like the contract classes?
    public static class ContractHelperExtensions
    {
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of
        /// a sequence until index <paramref name="startIndex"/>, then bypasses
        /// the next <paramref name="count"/> elements in the sequence and then
        /// returns the remaining elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of
        /// <paramref name="enumerable"/>.</typeparam>
        /// <param name="enumerable">An <see cref="SCG.IEnumerable{T}"/> to return
        /// elements from.</param>
        /// <param name="startIndex">The number of elements to return before
        /// skipping the next <paramref name="count"/> elements.</param>
        /// <param name="count">The number of elements to skip before returning
        /// the remaining elements.</param>
        /// <returns>An <see cref="SCG.IEnumerable{T}"/> containing the elements 
        /// from the specified <see cref="SCG.IEnumerable{T}"/>, but with a range
        /// of items skipped.</returns>
        /// <remarks>This is only intended for code contracts, and is not 
        /// optimal in any sense.</remarks>
        [Pure]
        public static SCG.IEnumerable<T> SkipRange<T>(this SCG.IEnumerable<T> enumerable, int startIndex, int count)
        {
            // Argument is non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            // ReSharper disable PossibleMultipleEnumeration
            return enumerable.Take(startIndex).Concat(enumerable.Skip(startIndex + count));
            // ReSharper enable PossibleMultipleEnumeration
        }

        /// <summary>
        /// Determines whether two enumerables contain the same elements in
        /// regards to multiplicity, but not sequence order, using the default
        /// equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input
        /// sequences.</typeparam>
        /// <param name="first">An <see cref="SCG.IEnumerable{T}"/> to compare
        /// to <paramref name="second"/>.</param>
        /// <param name="second">An <see cref="SCG.IEnumerable{T}"/> to compare
        /// to <paramref name="first"/>.</param>
        /// <returns><c>true</c> if the enumerables contain equal items;
        /// otherwise, <c>false</c>.</returns>
        [Pure]
        public static bool UnsequenceEqual<T>(this SCG.IEnumerable<T> first, SCG.IEnumerable<T> second) => UnsequenceEqual(first, second, SCG.EqualityComparer<T>.Default);

        /// <summary>
        /// Determines whether two enumerables contain the same elements in
        /// regards to multiplicity, but not sequence order, using a specified
        /// <see cref="SCG.IEqualityComparer{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input
        /// sequences.</typeparam>
        /// <param name="first">An <see cref="SCG.IEnumerable{T}"/> to compare
        /// to <paramref name="second"/>.</param>
        /// <param name="second">An <see cref="SCG.IEnumerable{T}"/> to compare
        /// to <paramref name="first"/>.</param>
        /// <param name="comparer">An <see cref="SCG.IEqualityComparer{T}"/> to
        /// use to compare elements.</param>
        /// <returns><c>true</c> if the enumerables contain equal items;
        /// otherwise, <c>false</c>.</returns>
        [Pure]
        public static bool UnsequenceEqual<T>(this SCG.IEnumerable<T> first, SCG.IEnumerable<T> second, SCG.IEqualityComparer<T> comparer)
        {
            if (ReferenceEquals(first, second)) {
                return true;
            }

            if (first == null || second == null) {
                return false;
            }

            var firstArray = first as T[] ?? first.ToArray();
            var secondArray = second as T[] ?? second.ToArray();

            if (firstArray.Length != secondArray.Length) {
                return false;
            }

            // Use default comparer if none is supplied
            comparer = comparer ?? SCG.EqualityComparer<T>.Default;

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
        public static bool ContainsSame<T>(this SCG.IEnumerable<T> enumerable, T item)
        {
            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            return enumerable.Contains(item, GetSameEqualityComparer<T>());
        }

        public static int ContainsSameCount<T>(this SCG.IEnumerable<T> enumerable, T item)
        {
            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            return enumerable.CountDuplicates(item, GetSameEqualityComparer<T>());
        }

        public static bool IsSameAs<T>(this T item, T otherItem) => GetSameEqualityComparer<T>().Equals(item, otherItem);

        public static bool IsSameSequenceAs<T>(this SCG.IEnumerable<T> enumerable, SCG.IEnumerable<T> otherEnumerable)
            => enumerable.SequenceEqual(otherEnumerable, GetSameEqualityComparer<T>());

        public static bool HasSameAs<T>(this SCG.IEnumerable<T> enumerable, SCG.IEnumerable<T> otherEnumerable)
            => enumerable.UnsequenceEqual(otherEnumerable, GetSameEqualityComparer<T>());

        /// <summary>
        /// Returns an <see cref="SCG.IEqualityComparer{T}"/> that checks if
        /// the objects are the same, possibly bypassing the type
        /// <typeparamref name="T"/>'s own <see cref="object.Equals(object)"/>
        /// method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>An <see cref="SCG.IEqualityComparer{T}"/> that checks if
        /// objects are the same.</returns>
        /// <remarks>
        /// Primitive types are compared using the default equality comparer 
        /// for the type. Structs are compared using reflection to compare each
        /// field. Objects are compared using reference equality.
        /// </remarks>
        private static SCG.IEqualityComparer<T> GetSameEqualityComparer<T>()
            => typeof(T).IsValueType
                ? (typeof(T).IsPrimitive
                    ? SCG.EqualityComparer<T>.Default
                    : CreateStructComparer<T>())
                : ComparerFactory.CreateReferenceEqualityComparer<T>();


        public static SCG.IEqualityComparer<T> CreateStructComparer<T>() => ComparerFactory.CreateEqualityComparer<T>(StructEquals, type => type.GetHashCode());

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

            var type = typeof(T);

            // If the structs don't consider themselves equal, just return false
            if (!x.Equals(y)) {
                return false;
            }

            var thisFieldsInfos = type.GetFields(Instance | Public | NonPublic);

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
                    // Call method recursively for structs - we invoke a generic method, as variables are objects and therefore not typed properly
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