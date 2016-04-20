// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using SCG = System.Collections.Generic;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;
using static C6.Tests.Helpers.TestHelper;


namespace C6.Tests
{
    [TestFixture]
    public abstract class ICollectionValueTests : IEnumerableTests
    {
        #region Factories
        
        /// <summary>
        /// Creates an empty collection value.
        /// </summary>
        /// <param name="allowsNull">A value indicating whether the collection
        /// allows <c>null</c> items.</param>
        /// <typeparam name="T">The type of the items in the collection value.
        /// </typeparam>
        /// <returns>An empty collection value.</returns>
        protected abstract ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false);

        /// <summary>
        /// Creates a collection value containing the items in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection value.
        /// </typeparam>
        /// <param name="enumerable">The collection whose items are copied to
        /// the new collection value.</param>
        /// <param name="allowsNull">A value indicating whether the collection
        /// allows <c>null</c> items.</param>
        /// <returns>A collection value containing the items in the enumerable.
        /// </returns>
        protected abstract ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false);

        #region Helpers

        private ICollectionValue<T> GetCollectionValue<T>(params T[] items) => GetCollectionValue((SCG.IEnumerable<T>) items);

        private ICollectionValue<int> GetIntCollectionValue(Random random, bool allowsNull = false)
            => GetCollectionValue(GetIntegers(random, GetCount(random)), allowsNull);

        private ICollectionValue<int> GetIntCollectionValue(Random random, int count, bool allowsNull = false)
            => GetCollectionValue(GetIntegers(random, count), allowsNull);

        private ICollectionValue<string> GetStringCollectionValue(Randomizer random, bool allowsNull = false)
            => GetCollectionValue(GetStrings(random, GetCount(random)), allowsNull);

        private ICollectionValue<string> GetStringCollectionValue(Randomizer random, int count, bool allowsNull = false)
            => GetCollectionValue(GetStrings(random, count), allowsNull);
        
        #endregion

        #region Inherited

        protected override SCG.IEnumerable<T> GetEmptyEnumerable<T>() => GetEmptyCollectionValue<T>();

        protected override SCG.IEnumerable<T> GetEnumerable<T>(SCG.IEnumerable<T> enumerable) => GetCollectionValue(enumerable);

        #endregion

        #endregion

        #region Test Methods

        #region Properties
        
        #region AllowsNull

        // TODO: Are there better tests to perform here?

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
        public void AllowsNull_EmptyCollectionAllowsNull_False()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>(allowsNull: false);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        [Test]
        public void AllowsNull_AllowsNull_True()
        {
            // Arrange
            var collection = GetCollectionValue(Enumerable.Empty<string>(), allowsNull: true);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.True);
        }

        [Test]
        public void AllowsNull_AllowsNull_False()
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
            var collection = GetEmptyCollectionValue<int>();

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
            var collection = GetIntCollectionValue(Random, size);

            // Act
            var count = collection.Count;

            // Assert
            Assert.That(count, Is.EqualTo(size));
        }

        #endregion

        #region CountSpeed

        [Test]
        public void CountSpeed_EmptyCollection_Constant()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            var speed = collection.CountSpeed;

            // Assert
            Assert.That(speed, Is.EqualTo(Speed.Constant));
        }

        #endregion

        #region IsEmpty

        [Test]
        public void IsEmpty_EmptyCollection_True()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            var isEmpty = collection.IsEmpty;

            // Assert
            Assert.That(isEmpty, Is.True);
        }

        [Test]
        public void IsEmpty_RandomCollection_False()
        {
            // Arrange
            var size = GetCount(Random);
            var collection = GetIntCollectionValue(Random, size);

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
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.Choose(), Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void Choose_SingleItemCollection_Item()
        {
            // Arrange
            var item = Random.GetString();
            var collection = GetCollectionValue(item);

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(choose, Is.SameAs(item));
        }

        [Test]
        public void Choose_SingleValueTypeCollection_Item()
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
        public void Choose_RandomCollection_ItemFromCollection()
        {
            // Arrange
            var collection = GetStringCollectionValue(Random);

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(collection, Has.Some.SameAs(choose));
        }

        #endregion

        #region CopyTo(T[], int)

        [Test]
        public void CopyTo_NullArray_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetIntCollectionValue(Random);

            // Act & Assert
            Assert.That(() => collection.CopyTo(null, 0), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CopyTo_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetIntCollectionValue(Random);
            var array = new int[collection.Count];

            // Act & Assert
            Assert.That(() => collection.CopyTo(array, -1), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_IndexOutOfBound_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetIntCollectionValue(Random);
            var array = new int[collection.Count];
            var index = Random.Next(1, collection.Count);

            // Act & Assert
            Assert.That(() => collection.CopyTo(array, index), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_EqualSizeArray_Equals()
        {
            // Arrange
            var collection = GetIntCollectionValue(Random);
            var array = new int[collection.Count];

            // Act
            collection.CopyTo(array, 0);

            // Assert
            Assert.That(array, Is.EqualTo(collection));
        }

        [Test]
        public void CopyTo_CopyToRandomIndex_SectionEquals()
        {
            // Arrange
            var collection = GetIntCollectionValue(Random);
            var array = GetIntegers(Random, (int) (collection.Count*1.7));
            var arrayIndex = Random.Next(0, array.Length - collection.Count);

            // Act
            collection.CopyTo(array, arrayIndex);
            var section = array.Skip(arrayIndex).Take(collection.Count);

            // Assert
            Assert.That(section, Is.EqualTo(collection));
        }

        #endregion

        #region ToArray()

        [Test]
        public void ToArray_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.Empty);
        }

        [Test]
        public void ToArray_EmptyCollection_NotNull()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.Not.Null);
        }

        [Test]
        public void ToArray_SingleItemCollection_SingleItemArray()
        {
            // Arrange
            var item = Random.GetString();
            var collection = GetCollectionValue(item);
            var itemArray = new[] { item };

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(itemArray));
        }

        [Test]
        public void ToArray_RandomNonValueTypeCollection_Equal()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollectionValue(items);

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(items));
        }

        #endregion

        #endregion
        
        #endregion
    }
}