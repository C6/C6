// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;

using NUnit.Framework;
using NUnit.Framework.Internal;

using SCG = System.Collections.Generic;

using static C6.Contracts.ContractMessage;
using static C6.Tests.Helpers.TestHelper;


namespace C6.Tests.Collections
{
    [TestFixture]
    public abstract class ICollectionValueTests : IEnumerableTests
    {
        #region Factories

        /// <summary>
        /// Creates an empty collection value.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection value.
        /// </typeparam>
        /// <returns>An empty collection value.</returns>
        protected abstract ICollectionValue<T> GetEmptyCollectionValue<T>();

        /// <summary>
        /// Creates a collection value containing the items in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection value.
        /// </typeparam>
        /// <param name="enumerable">The collection whose items are copied to
        /// the new collection value.</param>
        /// <returns>A collection value containing the items in the enumerable.
        /// </returns>
        protected abstract ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false);

        #region Helpers

        private ICollectionValue<T> GetCollectionValue<T>(params T[] items) => GetCollectionValue((SCG.IEnumerable<T>) items);

        private ICollectionValue<int> GetRandomIntCollectionValue(Random random, bool allowsNull = false)
            => GetCollectionValue(GetRandomIntEnumerable(random, GetRandomCount(random)), allowsNull);

        private ICollectionValue<int> GetRandomIntCollectionValue(Random random, int count, bool allowsNull = false)
            => GetCollectionValue(GetRandomIntEnumerable(random, count), allowsNull);

        private ICollectionValue<string> GetRandomStringCollectionValue(Randomizer random, bool allowsNull = false)
            => GetCollectionValue(GetRandomStringEnumerable(random, GetRandomCount(random)), allowsNull);

        private ICollectionValue<string> GetRandomStringCollectionValue(Randomizer random, int count, bool allowsNull = false)
            => GetCollectionValue(GetRandomStringEnumerable(random, count), allowsNull);

        #endregion

        #region Inherited

        protected override SCG.IEnumerable<T> GetEmptyEnumerable<T>() => GetEmptyCollectionValue<T>();

        protected override SCG.IEnumerable<T> GetEnumerable<T>(SCG.IEnumerable<T> enumerable) => GetCollectionValue(enumerable);

        #endregion

        #endregion

        #region Test Methods

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
            var random = TestContext.CurrentContext.Random;
            var size = GetRandomCount(random);
            var collection = GetRandomIntCollectionValue(random, size);

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
            var random = TestContext.CurrentContext.Random;
            var size = GetRandomCount(random);
            var collection = GetRandomIntCollectionValue(random, size);

            // Act
            var isEmpty = collection.IsEmpty;

            // Assert
            Assert.That(isEmpty, Is.False);
        }

        #endregion

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
            var random = TestContext.CurrentContext.Random;
            var item = random.GetString();
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
            var random = TestContext.CurrentContext.Random;
            var item = random.Next();
            var collection = GetCollectionValue(new[] { item });

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(choose, Is.EqualTo(item));
        }

        [Test]
        public void Choose_RandomCollection_ItemFromCollection()
        {
            // Arrange
            var collection = GetRandomStringCollectionValue(TestContext.CurrentContext.Random);

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
            var collection = GetRandomIntCollectionValue(TestContext.CurrentContext.Random);

            // Act & Assert
            Assert.That(() => collection.CopyTo(null, 0), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CopyTo_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetRandomIntCollectionValue(TestContext.CurrentContext.Random);
            var array = new int[collection.Count];

            // Act & Assert
            Assert.That(() => collection.CopyTo(array, -1), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_IndexOutOfBound_ViolatesPrecondition()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var collection = GetRandomIntCollectionValue(random);
            var array = new int[collection.Count];
            var index = random.Next(1, collection.Count);

            // Act & Assert
            Assert.That(() => collection.CopyTo(array, index), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_EqualSizeArray_Equals()
        {
            // Arrange
            var collection = GetRandomIntCollectionValue(TestContext.CurrentContext.Random);
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
            var random = TestContext.CurrentContext.Random;
            var collection = GetRandomIntCollectionValue(random);
            var array = GetRandomIntEnumerable(random, (int) (collection.Count * 1.7)).ToArray();
            var arrayIndex = random.Next(0, array.Length - collection.Count);

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
            var random = TestContext.CurrentContext.Random;
            var item = random.GetString();
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
            var items = GetRandomStringEnumerable(TestContext.CurrentContext.Random).ToArray();
            var collection = GetCollectionValue(items);

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(items));
        }

        #endregion

        #endregion
    }
}