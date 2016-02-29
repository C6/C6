// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using SCG = System.Collections.Generic;

using NUnit.Framework;


namespace C6.Tests
{
    [TestFixture]
    public class ArrayListTests
    {
        #region Helper Methods

        private static ICollectionValue<T> GetEmptyList<T>() => new ArrayList<T>();

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
            var size = random.Next(5, 20);
            var enumerable = Enumerable.Range(0, size);
            var collection = GetList(enumerable);

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

        #endregion
    }
}