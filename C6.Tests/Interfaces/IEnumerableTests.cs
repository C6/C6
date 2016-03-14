// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Collections.Generic;

using NUnit.Framework;


namespace C6.Tests.Collections
{
    [TestFixture]
    public abstract class IEnumerableTests : TestBase
    {
        #region Factories

        /// <summary>
        /// Creates an empty enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
        /// <returns>An empty enumerable.</returns>
        protected abstract IEnumerable<T> GetEmptyEnumerable<T>();

        /// <summary>
        /// Creates a enumerable containing the items in another enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerable.
        /// </typeparam>
        /// <param name="enumerable">The collection whose items are copied to
        /// the new enumerable.</param>
        /// <returns>A enumerable containing the items in another enumerable.
        /// </returns>
        protected abstract IEnumerable<T> GetEnumerable<T>(IEnumerable<T> enumerable);

        #endregion

        #region GetEnumerator()

        [Test]
        public void GetEnumerator_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyEnumerable<int>();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void GetEnumerator_NonEmptyCollection_NotEmpty()
        {
            // Arrange
            var collection = GetEnumerable(new[] { 1, 2, 3 });

            // Assert
            Assert.That(collection, Is.Not.Empty);
        }

        [Test]
        public void GetEnumerator_NonEmptyCollection_ContainsInitialItems()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };
            var collection = GetEnumerable(array);

            // Act & Assert
            Assert.That(collection, Is.EqualTo(array));
        }

        #endregion
    }
}