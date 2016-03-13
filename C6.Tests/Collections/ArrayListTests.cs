// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Collections;
using C6.Tests.Contracts;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public class ArrayListTests : ICollectionValueTests
    {
        #region Helper Methods

        private static IExtensible<T> GetEmptyList<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
            => GetList(Enumerable.Empty<T>(), equalityComparer, allowsNull);

        private static IExtensible<T> GetList<T>(params T[] array)
            => GetList((SCG.IEnumerable<T>) array);

        private static IExtensible<int> GetRandomIntList(Random random, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetList(GetRandomIntEnumerable(random, GetRandomCount(random)), equalityComparer, allowsNull);

        private static IExtensible<int> GetRandomIntList(Random random, int count, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetList(GetRandomIntEnumerable(random, count), equalityComparer, allowsNull);

        private static IExtensible<string> GetRandomStringList(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetList(GetRandomStringEnumerable(random, GetRandomCount(random)), equalityComparer, allowsNull);

        private static IExtensible<string> GetRandomStringList(Randomizer random, int count, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetList(GetRandomStringEnumerable(random, count), equalityComparer, allowsNull);

        #endregion

        #region Factories

        private static IExtensible<T> GetList<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => new ArrayList<T>(enumerable, equalityComparer, allowsNull);

        #endregion

        #region IExtensible<T>

        #region EqualityComparer

        [Test]
        public void EqualityComparer_DefaultComparer_NotNull()
        {
            // Arrange
            var list = GetRandomStringList(TestContext.CurrentContext.Random);

            // Act
            var equalityComparer = list.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.Not.Null);
        }

        [Test]
        public void EqualityComparer_CustomEqualityComparer_Equal()
        {
            // Arrange
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);
            var list = GetEmptyList(customEqualityComparer);

            // Act
            var equalityComparer = list.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(customEqualityComparer));
        }

        [Test]
        public void EqualityComparer_DefaultEqualityComparer_Equal()
        {
            // Arrange
            var defaultEqualityComparer = SCG.EqualityComparer<int>.Default;
            var list = GetEmptyList<int>();

            // Act
            var equalityComparer = list.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(defaultEqualityComparer));
        }

        #endregion

        #region Add(T)

        // TODO: Test read-only collections
        // TODO: Test fixed-size collections

        [Test]
        public void Add_NullDisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var allowNull = false;
            var list = GetEmptyList<string>(allowsNull: allowNull);

            // Act & Assert
            Assert.That(() => list.Add(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Add_EmptyCollectionAddItem_ContainsItem()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var list = GetEmptyList<string>();
            var item = random.GetString();
            var itemArray = new[] { item };

            // Act
            list.Add(item);

            // Assert
            Assert.That(list, Is.EqualTo(itemArray));
        }

        [Test]
        public void Add_EmptyCollection_ItemIsAdded()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var list = GetEmptyList<string>();
            var item = random.GetString();

            // Act
            var result = list.Add(item);

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
            var list = GetList(item);
            var allowsDuplicates = list.AllowsDuplicates;

            // Act
            var result = list.Add(duplicate);

            // Assert
            Assert.That(result, Is.EqualTo(allowsDuplicates));
        }

        [Test]
        public void Add_ManyItems_Equivalent()
        {
            // Arrange
            var equalityComparer = ComparerFactory.CreateReferenceEqualityComparer<string>();
            var list = GetEmptyList(equalityComparer);
            var random = TestContext.CurrentContext.Random;
            var count = random.Next(100, 250);
            var items = GetRandomStringEnumerable(random, count).ToArray();

            // Act
            foreach (var item in items) {
                list.Add(item); // TODO: Verify that items were added?
            }

            // Assert
            Assert.That(list, Is.EquivalentTo(items));
        }

        #endregion

        #endregion

        #region ArrayList<T>

        #region Constructors

        [Test]
        public void Constructor_Default_Empty()
        {
            // Act
            var collection = new ArrayList<int>();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Constructor_Default_DefaultEqualityComparer()
        {
            // Arrange
            var defaultEqualityComparer = SCG.EqualityComparer<string>.Default;

            // Act
            var collection = new ArrayList<string>();
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(defaultEqualityComparer));
        }

        [Test]
        public void Constructor_DefaultForValueType_DisallowsNull()
        {
            // Act
            var collection = new ArrayList<int>();
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        [Test]
        public void Constructor_DefaultForNonValue_DisallowsNull()
        {
            // Act
            var collection = new ArrayList<string>();
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        [Test]
        public void Constructor_ValueTypeCollectionAllowsNull_ViolatesPrecondition()
        {
            // Arrange
            var allowsNull = true;

            // Act & Assert
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.That(() => new ArrayList<int>(allowsNull: allowsNull), Violates.ConstructorPrecondition); // TODO: Violates.Precondition
        }

        [Test]
        public void Constructor_ValueTypeCollectionDisallowsNull_DisallowsNull()
        {
            // Arrange
            var allowsNull = false;

            // Act
            var collection = new ArrayList<int>(allowsNull: allowsNull);

            // Assert
            Assert.That(collection.AllowsNull, Is.False);
        }

        [Test]
        public void Constructor_NonValueTypeCollection_AllowNull([Values(true, false)] bool allowNull)
        {
            // Act
            var collection = new ArrayList<string>(allowsNull: allowNull);
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.EqualTo(allowNull));
        }

        [Test]
        public void Constructor_NullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            SCG.IEnumerable<string> enumerable = null;

            // Act & Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.That(() => new ArrayList<string>(enumerable), Violates.ConstructorPrecondition); // TODO: Violates.Precondition
        }

        [Test]
        public void Constructor_EmptyEnumerable_Empty()
        {
            // Arrange
            var enumerable = Enumerable.Empty<int>();

            // Act
            var list = new ArrayList<int>(enumerable);

            // Assert
            Assert.That(list, Is.Empty);
        }

        [Test]
        public void Constructor_RandomNonValueTypeEnumerable_Equal()
        {
            // Arrange
            var array = GetRandomStringEnumerable(TestContext.CurrentContext.Random).ToArray();

            // Act
            var list = new ArrayList<string>(array);

            // Assert
            Assert.That(list, Is.EqualTo(array));
        }

        [Test]
        public void Constructor_EnumerableWithNull_ViolatesPrecondition()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var array = GetRandomStringEnumerable(random).ToArray();
            array[random.Next(0, array.Length)] = null;

            // Act & Assert
            Assert.That(() => new ArrayList<string>(array), Violates.ConstructorPrecondition); // TODO: Violates.Precondition
        }

        [Test]
        public void Constructor_EnumerableBeingChanged_Unequal()
        {
            // Arrange
            var array = GetRandomIntEnumerable(TestContext.CurrentContext.Random).ToArray();

            // Act
            var collection = new ArrayList<int>(array);
            for (var i = 0; i < array.Length; i++) {
                array[i] *= -1;
            }

            // Assert
            Assert.That(collection, Is.Not.EqualTo(array));
        }

        [Test]
        public void Constructor_EnumerableWithNullDisallowNull_ViolatesPrecondition()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var array = GetRandomStringEnumerable(random).ToArray();
            array[random.Next(0, array.Length)] = null;

            // Act & Assert
            Assert.That(() => new ArrayList<string>(array, allowsNull: false), Violates.ConstructorPrecondition); // TODO: Violates.Precondition
        }

        [Test]
        public void Constructor_EqualityComparer_EqualsGivenEqualityComparer()
        {
            // Arrange
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);

            // Act
            var list = new ArrayList<int>(equalityComparer: customEqualityComparer);
            var equalityComparer = list.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(customEqualityComparer));
        }

        [Test]
        public void Constructor_EnumerableConstructorEqualityComparer_EqualsGivenEqualityComparer()
        {
            // Arrange
            var enumerable = Enumerable.Empty<int>();
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);

            // Act
            var list = new ArrayList<int>(enumerable, customEqualityComparer);
            var equalityComparer = list.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(customEqualityComparer));
        }

        #endregion

        #region Properties

        #region AllowsDuplicates

        [Test]
        public void AllowsDuplicates_RandomCollection_False()
        {
            // Arrange
            var list = GetRandomStringList(TestContext.CurrentContext.Random);

            // Act
            var allowsDuplicates = list.AllowsDuplicates;

            // Assert
            Assert.That(allowsDuplicates, Is.True);
        }

        #endregion

        #region DuplicatesByCounting

        [Test]
        public void DuplicatesByCounting_RandomCollection_False()
        {
            // Arrange
            var list = GetRandomStringList(TestContext.CurrentContext.Random);

            // Act
            var duplicatesByCounting = list.DuplicatesByCounting;

            // Assert
            Assert.That(duplicatesByCounting, Is.False);
        }

        #endregion

        #region IsFixedSize

        [Test]
        public void IsFixedSize_RandomCollection_False()
        {
            // Arrange
            var list = GetRandomStringList(TestContext.CurrentContext.Random);

            // Act
            var isFixedSize = list.IsFixedSize;

            // Assert
            Assert.That(isFixedSize, Is.False);
        }

        #endregion

        #region IsReadOnly

        [Test]
        public void IsReadOnly_RandomCollection_False()
        {
            // Arrange
            var list = GetRandomStringList(TestContext.CurrentContext.Random);

            // Act
            var isReadOnly = list.IsReadOnly;

            // Assert
            Assert.That(isReadOnly, Is.False);
        }

        #endregion

        #endregion

        #region Methods

        #region Add(T)

        [Test]
        public void Add_InsertAddedToTheEnd_LastItemSame()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var list = GetRandomStringList(random);
            var item = random.GetString();

            // Act
            list.Add(item);

            // Assert
            Assert.That(list.Last(), Is.SameAs(item)); // TODO: Update to Last
        }

        #endregion

        #region Choose()

        [Test]
        public void Choose_RandomCollection_LastItem()
        {
            // Arrange
            var enumerable = GetRandomStringEnumerable(TestContext.CurrentContext.Random).ToArray();
            var list = GetList(enumerable);
            var lastItem = enumerable.Last();

            // Act
            var choose = list.Choose();

            // Assert
            Assert.That(choose, Is.SameAs(lastItem));
        }

        #endregion

        #endregion

        #endregion

        protected override EventTypes ListenableEvents => All;

        protected override ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false) => new ArrayList<T>(allowsNull: allowsNull);

        protected override ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new ArrayList<T>(enumerable, allowsNull: allowsNull);
    }
}