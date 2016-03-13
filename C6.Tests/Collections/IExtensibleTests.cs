// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests.Collections
{
    [TestFixture]
    public abstract class IExtensibleTests : ICollectionValueTests
    {
        #region Factories

        protected abstract bool AllowsDuplicates { get; }
        protected abstract bool DuplicatesByCounting { get; }
        protected abstract bool IsFixedSize { get; }
        protected abstract bool IsReadOnly { get; }

        protected abstract IExtensible<T> GetEmptyExtensible<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        protected abstract IExtensible<T> GetExtensible<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        #region Helpers

        private IExtensible<T> GetExtensible<T>(params T[] array) => GetExtensible((SCG.IEnumerable<T>) array);

        private IExtensible<int> GetRandomIntExtensible(Random random, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetExtensible(GetRandomIntEnumerable(random, GetRandomCount(random)), equalityComparer, allowsNull);

        private IExtensible<int> GetRandomIntExtensible(Random random, int count, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetExtensible(GetRandomIntEnumerable(random, count), equalityComparer, allowsNull);

        private IExtensible<string> GetRandomStringExtensible(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetExtensible(GetRandomStringEnumerable(random, GetRandomCount(random)), equalityComparer, allowsNull);

        private IExtensible<string> GetRandomStringExtensible(Randomizer random, int count, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetExtensible(GetRandomStringEnumerable(random, count), equalityComparer, allowsNull);

        #endregion

        #region Inherited

        protected override ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false)
            => GetEmptyExtensible<T>(allowsNull: allowsNull);

        protected override ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false)
            => GetExtensible(enumerable, allowsNull: allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region Properties

        #region AllowsDuplicates

        [Test]
        public void AllowsDuplicates_RandomCollection_AllowsDuplicates()
        {
            // Arrange
            var collection = GetRandomStringExtensible(TestContext.CurrentContext.Random);

            // Act
            var allowsDuplicates = collection.AllowsDuplicates;

            // Assert
            Assert.That(allowsDuplicates, Is.EqualTo(AllowsDuplicates));
        }

        #endregion

        #region DuplicatesByCounting

        [Test]
        public void DuplicatesByCounting_RandomCollection_DuplicatesByCounting()
        {
            // Arrange
            var collection = GetRandomStringExtensible(TestContext.CurrentContext.Random);

            // Act
            var duplicatesByCounting = collection.DuplicatesByCounting;

            // Assert
            Assert.That(duplicatesByCounting, Is.EqualTo(DuplicatesByCounting));
        }

        #endregion

        #region EqualityComparer

        [Test]
        public void EqualityComparer_DefaultComparer_NotNull()
        {
            // Arrange
            var collection = GetRandomStringExtensible(TestContext.CurrentContext.Random);

            // Act
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.Not.Null);
        }

        [Test]
        public void EqualityComparer_CustomEqualityComparer_Equal()
        {
            // Arrange
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);
            var collection = GetEmptyExtensible(customEqualityComparer);

            // Act
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(customEqualityComparer));
        }

        [Test]
        public void EqualityComparer_DefaultEqualityComparer_Equal()
        {
            // Arrange
            var defaultEqualityComparer = SCG.EqualityComparer<int>.Default;
            var collection = GetEmptyExtensible<int>();

            // Act
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(defaultEqualityComparer));
        }

        #endregion

        #region IsFixedSize

        [Test]
        public void IsFixedSize_RandomCollection_IsFixedSize()
        {
            // Arrange
            var collection = GetRandomStringExtensible(TestContext.CurrentContext.Random);

            // Act
            var isFixedSize = collection.IsFixedSize;

            // Assert
            Assert.That(isFixedSize, Is.EqualTo(IsFixedSize));
        }

        #endregion

        #region IsReadOnly

        [Test]
        public void IsReadOnly_RandomCollection_IsReadOnly()
        {
            // Arrange
            var collection = GetRandomStringExtensible(TestContext.CurrentContext.Random);

            // Act
            var isReadOnly = collection.IsReadOnly;

            // Assert
            Assert.That(isReadOnly, Is.EqualTo(IsReadOnly));
        }

        #endregion

        #endregion

        #region Methods

        #region Add(T)

        // TODO: Test read-only collections
        // TODO: Test fixed-size collections

        [Test]
        public void Add_NullDisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var allowNull = false;
            var collection = GetEmptyExtensible<string>(allowsNull: allowNull);

            // Act & Assert
            Assert.That(() => collection.Add(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Add_EmptyCollectionAddItem_ContainsItem()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var collection = GetEmptyExtensible<string>();
            var item = random.GetString();
            var itemArray = new[] { item };

            // Act
            collection.Add(item);

            // Assert
            Assert.That(collection, Is.EqualTo(itemArray));
        }

        [Test]
        public void Add_EmptyCollection_ItemIsAdded()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var collection = GetEmptyExtensible<string>();
            var item = random.GetString();

            // Act
            var result = collection.Add(item);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Add_SingleItemCollectionAddsDuplicate_ItemIsAddedIfAllowsDuplicatesIsTrue()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var item = random.GetString();
            var duplicate = string.Copy(item);
            var collection = GetExtensible(item);
            var allowsDuplicates = collection.AllowsDuplicates;

            // Act
            var result = collection.Add(duplicate);

            // Assert
            Assert.That(result, Is.EqualTo(allowsDuplicates));
        }

        [Test]
        public void Add_ManyItems_Equivalent()
        {
            // Arrange
            var equalityComparer = ComparerFactory.CreateReferenceEqualityComparer<string>();
            var collection = GetEmptyExtensible(equalityComparer);
            var random = TestContext.CurrentContext.Random;
            var count = random.Next(100, 250);
            var items = GetRandomStringEnumerable(random, count).ToArray();

            // Act
            foreach (var item in items) {
                collection.Add(item); // TODO: Verify that items were added?
            }

            // Assert
            Assert.That(collection, Is.EquivalentTo(items));
        }

        #endregion

        #endregion

        #endregion
    }
}