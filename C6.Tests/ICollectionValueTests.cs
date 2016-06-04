// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Globalization;
using System.Linq;
using System.Text;

using C6.Collections;
using C6.Tests.Contracts;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.Speed;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class ICollectionValueTests : IEnumerableTests
    {
        #region Factories

        /// <summary>
        ///     Creates an empty collection value.
        /// </summary>
        /// <param name="allowsNull">
        ///     A value indicating whether the collection allows <c>null</c> items.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the items in the collection value.
        /// </typeparam>
        /// <returns>
        ///     An empty collection value.
        /// </returns>
        protected abstract ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false);

        /// <summary>
        ///     Creates a collection value containing the items in the enumerable.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the collection value.
        /// </typeparam>
        /// <param name="enumerable">
        ///     The collection whose items are copied to the new collection value.
        /// </param>
        /// <param name="allowsNull">
        ///     A value indicating whether the collection allows <c>null</c> items.
        /// </param>
        /// <returns>
        ///     A collection value containing the items in the enumerable.
        /// </returns>
        protected abstract ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false);

        /// <summary>
        ///     Returns an enumerable containing all the possible items that <see cref="ICollectionValue{T}.Choose"/> can return.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection from which an item is chosen.
        /// </param>
        /// <returns>
        ///     the possible items that <paramref name="collection"/>'s <see cref="ICollectionValue{T}.Choose"/> can return.
        /// </returns>
        protected abstract SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection);

        #region Inherited

        protected override SCG.IEnumerable<T> GetEmptyEnumerable<T>() => GetEmptyCollectionValue<T>();

        protected override SCG.IEnumerable<T> GetEnumerable<T>(SCG.IEnumerable<T> enumerable) => GetCollectionValue(enumerable);

        #endregion

        #region Helpers

        private ICollectionValue<T> GetCollectionValue<T>(params T[] items) => GetCollectionValue((SCG.IEnumerable<T>) items);

        private ICollectionValue<int> GetIntCollectionValue(Random random) => GetCollectionValue(GetIntegers(random, GetCount(random)), false);

        private ICollectionValue<string> GetStringCollectionValue(Randomizer random, bool allowsNull = false)
            => GetCollectionValue(GetStrings(random, GetCount(random)), allowsNull);

        private ICollectionValue<string> GetStringCollectionValue(Randomizer random, int count, bool allowsNull = false)
            => GetCollectionValue(GetStrings(random, count), allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region ICollectionValue<T>

        #region Properties

        #region AllowsNull

        [Test]
        public void AllowsNull_EmptyCollectionAllowsNull_True()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>(allowsNull: true);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.True);
        }

        [Test]
        public void AllowsNull_EmptyCollectionDisallowsNull_False()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>(allowsNull: false);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        [Test]
        public void AllowsNull_RandomCollectionAllowsNull_True()
        {
            // Arrange
            var collection = GetCollectionValue(Enumerable.Empty<string>(), allowsNull: true);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.True);
        }

        [Test]
        public void AllowsNull_RandomCollectionDisallowsNull_False()
        {
            // Arrange
            var collection = GetCollectionValue(Enumerable.Empty<string>(), allowsNull: false);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        #endregion

        #region Count

        [Test]
        public void Count_EmptyCollection_Zero()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>();

            // Act
            var count = collection.Count;

            // Assert
            Assert.That(count, Is.Zero);
        }

        [Test]
        public void Count_RandomlySizedCollection_Size()
        {
            // Arrange
            var size = GetCount(Random);
            var collection = GetStringCollectionValue(Random, size);

            // Act
            var count = collection.Count;

            // Assert
            Assert.That(count, Is.EqualTo(size));
        }

        #endregion

        #region CountSpeed

        [Test]
        public void CountSpeed_RandomCollection_Constant()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);

            // Act
            var countSpeed = collection.CountSpeed;

            // Assert
            Assert.That(countSpeed, Is.EqualTo(Constant));
        }

        #endregion

        #region IsEmpty

        [Test]
        public void IsEmpty_EmptyCollection_True()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>();

            // Act
            var isEmpty = collection.IsEmpty;

            // Assert
            Assert.That(isEmpty, Is.True);
        }

        [Test]
        public void IsEmpty_NonEmptyCollection_False()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);

            // Act
            var isEmpty = collection.IsEmpty;

            // Assert
            Assert.That(isEmpty, Is.False);
        }

        #endregion

        #endregion

        #region Methods

        #region Choose()

        [Test]
        public void Choose_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>();

            // Act & Assert
            Assert.That(() => collection.Choose(), Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void Choose_SingleItemCollection_SameItem()
        {
            // Arrange
            var item = GetString(Random);
            var collection = GetCollectionValue(item);

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(choose, Is.SameAs(item));
        }

        [Test]
        public void Choose_SingleValueTypeCollection_SameItem()
        {
            // Arrange
            var item = Random.Next();
            var collection = GetCollectionValue(item);

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(choose, Is.EqualTo(item));
        }

        [Test]
        public void Choose_RandomCollection_SameAsItemFromCollection()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(ChooseItems(collection), Has.Some.SameAs(choose));
        }

        #endregion

        #region CopyTo(T[], int)

        [Test]
        public void CopyTo_NullArray_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);

            // Act & Assert
            Assert.That(() => collection.CopyTo(null, 0), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CopyTo_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);
            var array = new string[collection.Count];
            var arrayIndex = GetNegative(Random);

            // Act & Assert
            Assert.That(() => collection.CopyTo(array, arrayIndex), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }
        
        [Test]
        public void CopyTo_IndexOutOfBound_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);
            var padding = GetCount(Random);
            var array = new string[collection.Count + padding];
            var arrayIndex = Random.Next(1, collection.Count) + padding;

            // Act & Assert
            Assert.That(() => collection.CopyTo(array, arrayIndex), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_EqualSizeArray_EqualsCollection()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);
            var array = new string[collection.Count];
            var arrayIndex = 0;

            // Act
            collection.CopyTo(array, arrayIndex);

            // Assert
            Assert.That(array, Is.EqualTo(collection).ByReference<string>());
        }

        [Test]
        public void CopyTo_RandomIndex_EqualsSection()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);
            var padding = GetCount(Random);
            var array = GetStrings(Random, collection.Count + padding);
            var arrayIndex = Random.Next(0, array.Length - collection.Count);

            // Act
            collection.CopyTo(array, arrayIndex);
            var section = array.Skip(arrayIndex).Take(collection.Count);

            // Assert
            Assert.That(section, Is.EqualTo(collection).ByReference<string>());
        }

        #endregion

        #region ToArray()

        [Test]
        public void ToArray_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>();

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Is.Empty);
        }
        
        [Test]
        public void ToArray_SingleItemCollection_SameAsItem()
        {
            // Arrange
            var item = GetString(Random);
            var collection = GetCollectionValue(item);
            var itemArray = new[] { item };

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(itemArray).ByReference<string>());
        }

        [Test]
        public void ToArray_RandomCollection_SameAsItems()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollectionValue(items);

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(items).ByReference<string>());
        }

        #endregion

        #endregion

        #endregion

        #region IShowable

        #region Methods

        // TODO: Review IShowable tests

        #region Show

        [Test]
        public void Show_NullStringBuilder_ViolatesPrecondition()
        {
            // Arrange
            var collectionValue = GetIntCollectionValue(Random);
            var rest = int.MaxValue;
            var formatProvider = CultureInfo.CurrentCulture.NumberFormat;

            // Act & Assert
            Assert.That(() => collectionValue.Show(null, ref rest, formatProvider), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void Show_EmptyCollection_BEHAVIOR()
        {
            // Arrange
            var collectionValue = GetEmptyCollectionValue<int>();
            var stringBuilder = new StringBuilder();
            var rest = int.MaxValue;
            var formatProvider = CultureInfo.CurrentCulture.NumberFormat;
            var shortestEmptyCollection = "{{  }}";

            // Act
            var show = collectionValue.Show(stringBuilder, ref rest, formatProvider);
            var result = stringBuilder.ToString();
            var length = result.Length;

            // Assert
            Assert.That(show, Is.True);
            Assert.That(result, Does.Not.Contain(Showing.Ellipses));
            Assert.That(length, Is.LessThanOrEqualTo(shortestEmptyCollection.Length));
        }

        [Test]
        public void Show_NegativeRest_EllipsesString()
        {
            // Arrange
            var collectionValue = GetIntCollectionValue(Random);
            var stringBuilder = new StringBuilder();
            var rest = Random.Next(int.MinValue, 0);
            var formatProvider = CultureInfo.CurrentCulture.NumberFormat;
            var shortestEmptyCollection = "{{  }}";

            // Act
            var show = collectionValue.Show(stringBuilder, ref rest, formatProvider);
            var result = stringBuilder.ToString();
            var length = result.Length;

            // Assert
            Assert.That(show, Is.False);
            Assert.That(result, Contains.Substring(Showing.Ellipses));
            Assert.That(length, Is.LessThanOrEqualTo(shortestEmptyCollection.Length + Showing.Ellipses.Length));
        }

        [Test]
        public void Show_RandomCollection_RestIsDecreasedByStringLength()
        {
            // Arrange
            var collectionValue = GetIntCollectionValue(Random);
            var stringBuilder = new StringBuilder();
            var originalLength = GetCount(Random);
            stringBuilder.Append(Random.GetString(originalLength));
            var rest = int.MaxValue;
            var formatProvider = CultureInfo.CurrentCulture.NumberFormat;

            // Act
            var show = collectionValue.Show(stringBuilder, ref rest, formatProvider);
            var result = stringBuilder.ToString();
            var length = result.Length - originalLength;

            // Assert
            Assert.That(show, Is.True);
            Assert.That(rest, Is.EqualTo(int.MaxValue - length).ByReference<string>());
        }

        [Test]
        public void Show_RandomCollectionTooBig_ResultIsTruncated()
        {
            // Arrange
            var collectionValue = GetIntCollectionValue(Random);
            var stringBuilder = new StringBuilder();
            var rest = 10;
            var refRest = rest;
            var formatProvider = CultureInfo.CurrentCulture.NumberFormat;

            // Act
            var show = collectionValue.Show(stringBuilder, ref refRest, formatProvider);
            var result = stringBuilder.ToString();
            var length = result.Length;

            // Assert
            Assert.That(show, Is.False);
            Assert.That(length, Is.EqualTo(rest - refRest).ByReference<string>());
            Assert.That(result, Contains.Substring(Showing.Ellipses));
        }

        #endregion

        #region ToString()

        // TODO: Test properly

        #endregion

        #region ToString(string, IFormatProvider)

        // TODO: Test properly

        #endregion

        #endregion

        #endregion

        #endregion
    }
}