// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using C6.Tests.Helpers;

using NUnit.Framework;

using SC = System.Collections;
using SCG = System.Collections.Generic;

using static C6.Tests.Helpers.TestHelper;


namespace C6.Tests
{
    [TestFixture]
    public abstract class IEnumerableTests : TestBase
    {
        #region Factories

        /// <summary>
        ///     Creates an empty enumerable.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the enumerable.
        /// </typeparam>
        /// <returns>An empty enumerable.</returns>
        protected abstract SCG.IEnumerable<T> GetEmptyEnumerable<T>();

        /// <summary>
        ///     Creates an enumerable containing the items in another enumerable.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the enumerable.
        /// </typeparam>
        /// <param name="enumerable">
        ///     The collection whose items are copied to the new enumerable.
        /// </param>
        /// <returns>
        ///     An enumerable containing the items in another enumerable.
        /// </returns>
        protected abstract SCG.IEnumerable<T> GetEnumerable<T>(SCG.IEnumerable<T> enumerable);

        #region Helpers

        private SC.IEnumerable GetEmptySCEnumerable<T>() => GetEmptyEnumerable<T>();

        private SC.IEnumerable GetSCEnumerable<T>(SCG.IEnumerable<T> enumerable) => GetEnumerable(enumerable);

        #endregion

        #endregion

        #region SCG.IEnumerable<T>

        #region Methods

        #region GetEnumerator()

        [Test]
        public void GetEnumerator_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyEnumerable<string>();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void GetEnumerator_NonEmptyCollection_ContainsInitialItems()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetEnumerable(items);

            // Act & Assert
            Assert.That(collection, Is.EqualTo(items).ByReference<string>());
        }

        #endregion

        #endregion

        #endregion

        #region SC.IEnumerable

        #region Methods

        #region GetEnumerator()

        [Test]
        public void GetSCEnumerator_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptySCEnumerable<string>();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void GetSCEnumerator_NonEmptyCollection_ContainsInitialItems()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetSCEnumerable(items);

            // Act & Assert
            Assert.That(collection, Is.EqualTo(items).ByReference<string>());
        }

        #endregion

        #endregion

        #endregion
    }
}