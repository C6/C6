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

        private static SCG.IEnumerable<int> GetEmptyList() => new ArrayList<int>();

        private static SCG.IEnumerable<T> GetList<T>(SCG.IEnumerable<T> enumerable) => new ArrayList<T>(enumerable);

        #endregion

        #region IEnumerable<T>

        [Test]
        public void GetEnumerator_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyList();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void GetEnumerator_NonEmptyCollection_NotEmpty()
        {
            // Arrange
            var collection = GetList(new [] {1, 2, 3});

            // Assert
            Assert.That(collection, Is.Not.Empty);
        }

        [Test]
        public void GetEnumerator_NonEmptyCollection_ContainsInitialItems()
        {
            // Arrange
            var enumerable = new [] {1, 2, 3};
            var collection = GetList(enumerable);

            // Act
            var result = enumerable.SequenceEqual(collection);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

    }
}