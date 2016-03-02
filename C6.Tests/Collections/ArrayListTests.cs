// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;

using NUnit.Framework;
using NUnit.Framework.Internal;

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

        private static ICollectionValue<T> GetEmptyList<T>()
            => GetList(Enumerable.Empty<T>());

        private static ICollectionValue<T> GetList<T>(params T[] array)
            => GetList((SCG.IEnumerable<T>) array);

        private static ICollectionValue<int> GetRandomIntList(Random random)
            => GetList(GetRandomIntEnumerable(random, GetRandomCount(random)));

        private static ICollectionValue<int> GetRandomIntList(Random random, int count)
            => GetList(GetRandomIntEnumerable(random, count));

        private static ICollectionValue<string> GetRandomStringList(Randomizer random)
            => GetList(GetRandomStringEnumerable(random, GetRandomCount(random)));

        private static ICollectionValue<string> GetRandomStringList(Randomizer random, int count)
            => GetList(GetRandomStringEnumerable(random, count));

        #endregion

        #region Factories

        private static ICollectionValue<T> GetList<T>(SCG.IEnumerable<T> enumerable) => new ArrayList<T>(enumerable);

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
            var collection = GetList(new[] { 1, 2, 3 });

            // Assert
            Assert.That(collection, Is.Not.Empty);
        }

        [Test]
        public void GetEnumerator_NonEmptyCollection_ContainsInitialItems()
        {
            // Arrange
            var enumerable = new[] { 1, 2, 3 };
            var collection = GetList(enumerable);

            // Act
            var result = enumerable.SequenceEqual(collection);

            // Assert
            Assert.That(result, Is.True);
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
            Assert.That(count, Is.EqualTo(0));
            // TODO: Assert.That(count, Is.Zero);
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
            Assert.That(() => collection.Choose(), Violates.Precondition);
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
            Assert.That(list, Contains.Item(choose));
        }

        #endregion

        #endregion

        #region ArrayList<T>

        #region Constructors

        #region ArrayList<T>()

        [Test]
        public void Constructor_DefaultConstructor_Empty()
        {
            // Act
            var collection = new ArrayList<int>();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Constructor_DefaultValueTypeConstructor_DisallowsNull()
        {
            // Act
            var collection = new ArrayList<int>();

            // Assert
            Assert.That(collection.AllowsNull, Is.False);
        }

        [Test]
        public void Constructor_DefaultNonValueTypeConstructor_DisallowsNull()
        {
            // Act
            var collection = new ArrayList<string>();

            // Assert
            Assert.That(collection.AllowsNull, Is.False);
        }

        #endregion

        #region ArrayList<T>(bool)

        [Test]
        public void Constructor_ValueTypeCollectionAllowsNull_ViolatesPrecondition()
        {
            // Arrange
            var allowsNull = true;

            // Act & Assert
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.That(() => new ArrayList<int>(allowsNull), Violates.ConstructorPrecondition); // TODO: Violates.Precondition
        }

        [Test]
        public void Constructor_ValueTypeCollectionDisallowsNull_DisallowsNull()
        {
            // Arrange
            var allowsNull = false;

            // Act
            var collection = new ArrayList<int>(allowsNull);

            // Assert
            Assert.That(collection.AllowsNull, Is.False);
        }

        [Test]
        public void Constructor_NonValueTypeCollection_AllowNull([Values(true, false)] bool allowNull)
        {
            // Act
            var collection = new ArrayList<string>(allowNull);

            // Assert
            Assert.That(collection.AllowsNull, Is.EqualTo(allowNull));
        }

        #endregion

        #region ArrayList(SCG.IEnumerable<T>)

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

        #endregion

        #region ArrayList(SCG.IEnumerable<T>, bool)

        // TODO: Test different combinations

        [Test]
        public void Constructor_EnumerableWithNullDisallowNull_ViolatesPrecondition()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var array = GetRandomStringEnumerable(random).ToArray();
            array[random.Next(0, array.Length)] = null;

            // Act & Assert
            Assert.That(() => new ArrayList<string>(array, false), Violates.ConstructorPrecondition); // TODO: Violates.Precondition
        }

        #endregion

        #endregion

        #region Choose

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
            Assert.That(choose, Is.EqualTo(lastItem));
        }

        #endregion

        #endregion
    }
}