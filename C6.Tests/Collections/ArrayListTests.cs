// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public class ArrayListTests
    {
        #region Helper Methods

        private static int GetRandomCount(Random random)
            => random.Next(5, 20);

        private static SCG.IEnumerable<int> GetRandomIntEnumerable(Random random)
            => GetRandomIntEnumerable(random, GetRandomCount(random));

        private static SCG.IEnumerable<int> GetRandomIntEnumerable(Random random, int count)
            => Enumerable.Range(0, count).Select(i => random.Next());

        private static SCG.IEnumerable<string> GetRandomStringEnumerable(Randomizer random)
            => GetRandomStringEnumerable(random, GetRandomCount(random));

        private static SCG.IEnumerable<string> GetRandomStringEnumerable(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => random.GetString());

        private static IExtensible<T> GetEmptyList<T>()
            => GetList(Enumerable.Empty<T>());

        private static IExtensible<T> GetList<T>(params T[] array)
            => GetList((SCG.IEnumerable<T>) array);

        private static IExtensible<int> GetRandomIntList(Random random)
            => GetList(GetRandomIntEnumerable(random, GetRandomCount(random)));

        private static IExtensible<int> GetRandomIntList(Random random, int count)
            => GetList(GetRandomIntEnumerable(random, count));

        private static IExtensible<string> GetRandomStringList(Randomizer random)
            => GetList(GetRandomStringEnumerable(random, GetRandomCount(random)));

        private static IExtensible<string> GetRandomStringList(Randomizer random, int count)
            => GetList(GetRandomStringEnumerable(random, count));

        #endregion

        #region Factories

        private static IExtensible<T> GetList<T>(SCG.IEnumerable<T> enumerable) => new ArrayList<T>(enumerable);

        #endregion

        #region IEnumerable<T>

        #region GetEnumerator()

        [Test]
        public void GetEnumerator_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyList<int>();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void GetEnumerator_NonEmptyCollection_NotEmpty()
        {
            // Arrange
            var collection = GetList(1, 2, 3);

            // Assert
            Assert.That(collection, Is.Not.Empty);
        }

        [Test]
        public void GetEnumerator_NonEmptyCollection_ContainsInitialItems()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };
            var collection = GetList(array);

            // Act & Assert
            Assert.That(collection, Is.EqualTo(array));
        }

        #endregion

        #endregion

        #region ICollectionValue<T>

        #region Count

        [Test]
        public void Count_EmptyCollection_Zero()
        {
            // Arrange
            var collection = GetEmptyList<int>();

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
            var collection = GetRandomIntList(random, size);

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
            var collection = GetEmptyList<int>();

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
            var collection = GetEmptyList<int>();

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
            var collection = GetRandomIntList(random, size);

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
            var collection = GetEmptyList<int>();

            // Act & Assert
            Assert.That(() => collection.Choose(), Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void Choose_SingleItemCollection_Item()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var item = random.GetString();
            var list = GetList(item);

            // Act
            var choose = list.Choose();

            // Assert
            Assert.That(choose, Is.SameAs(item));
        }

        [Test]
        public void Choose_SingleValueTypeCollection_Item()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var item = random.Next();
            var list = GetList(item);

            // Act
            var choose = list.Choose();

            // Assert
            Assert.That(choose, Is.EqualTo(item));
        }

        [Test]
        public void Choose_RandomCollection_ItemFromCollection()
        {
            // Arrange
            var list = GetRandomStringList(TestContext.CurrentContext.Random);

            // Act
            var choose = list.Choose();

            // Assert
            Assert.That(list, Has.Some.SameAs(choose));
        }

        #endregion

        #region CopyTo(T[], int)

        [Test]
        public void CopyTo_NullArray_ViolatesPrecondition()
        {
            // Arrange
            var list = GetRandomIntList(TestContext.CurrentContext.Random);

            // Act & Assert
            Assert.That(() => list.CopyTo(null, 0), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CopyTo_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var list = GetRandomIntList(TestContext.CurrentContext.Random);
            var array = new int[list.Count];

            // Act & Assert
            Assert.That(() => list.CopyTo(array, -1), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_IndexOutOfBound_ViolatesPrecondition()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var list = GetRandomIntList(random);
            var array = new int[list.Count];
            var index = random.Next(1, list.Count);

            // Act & Assert
            Assert.That(() => list.CopyTo(array, index), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_EqualSizeArray_Equals()
        {
            // Arrange
            var list = GetRandomIntList(TestContext.CurrentContext.Random);
            var array = new int[list.Count];

            // Act
            list.CopyTo(array, 0);

            // Assert
            Assert.That(array, Is.EqualTo(list));
        }

        [Test]
        public void CopyTo_CopyToRandomIndex_SectionEquals()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var list = GetRandomIntList(random);
            var array = GetRandomIntEnumerable(random, (int) (list.Count * 1.7)).ToArray();
            var arrayIndex = random.Next(0, array.Length - list.Count);

            // Act
            list.CopyTo(array, arrayIndex);
            var section = array.Skip(arrayIndex).Take(list.Count);

            // Assert
            Assert.That(section, Is.EqualTo(list));
        }

        #endregion

        #region ToArray()

        [Test]
        public void ToArray_EmptyCollection_Empty()
        {
            // Arrange
            var list = GetEmptyList<int>();

            // Act
            var array = list.ToArray();

            // Assert
            Assert.That(array, Is.Empty);
        }

        [Test]
        public void ToArray_EmptyCollection_NotNull()
        {
            // Arrange
            var list = GetEmptyList<int>();

            // Act
            var array = list.ToArray();

            // Assert
            Assert.That(array, Is.Not.Null);
        }

        [Test]
        public void ToArray_SingleItemCollection_SingleItemArray()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var item = random.GetString();
            var list = GetList(item);
            var itemArray = new[] { item };

            // Act
            var array = list.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(itemArray));
        }

        [Test]
        public void ToArray_RandomNonValueTypeCollection_Equal()
        {
            // Arrange
            var items = GetRandomStringEnumerable(TestContext.CurrentContext.Random).ToArray();
            var list = GetList(items);

            // Act
            var array = list.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(items));
        }

        #endregion

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

        // TODO: Check is equal to provided

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
    }
}