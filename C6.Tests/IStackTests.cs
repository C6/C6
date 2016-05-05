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
using static C6.Tests.Helpers.TestHelper;


namespace C6.Tests
{
    [TestFixture]
    public abstract class IStackTests : IListenableTests
    {
        #region Factories

        /// <summary>
        ///     Creates an empty <see cref="IStack{T}"/>.
        /// </summary>
        /// <param name="allowsNull">
        ///     A value indicating whether the <see cref="IStack{T}"/> allows <c>null</c> items.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the items in the <see cref="IStack{T}"/>.
        /// </typeparam>
        /// <returns>
        ///     An empty <see cref="IStack{T}"/>.
        /// </returns>
        protected abstract IStack<T> GetEmptyStack<T>(bool allowsNull = false);

        /// <summary>
        ///     Creates a <see cref="IStack{T}"/> containing the items in the enumerable.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the <see cref="IStack{T}"/>.
        /// </typeparam>
        /// <param name="enumerable">
        ///     The collection whose items are copied to the new <see cref="IStack{T}"/>.
        /// </param>
        /// <param name="allowsNull">
        ///     A value indicating whether the <see cref="IStack{T}"/> allows <c>null</c> items.
        /// </param>
        /// <returns>
        ///     A <see cref="IStack{T}"/> containing the items in the enumerable.
        /// </returns>
        protected abstract IStack<T> GetStack<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false);

        #region Helpers
        
        private IStack<string> GetStringStack(Randomizer random, bool allowsNull = false)
            => GetStack(GetStrings(random, GetCount(random)), allowsNull);

        #endregion

        #region Inherited

        protected override IListenable<T> GetEmptyListenable<T>(bool allowsNull = false) => GetEmptyStack<T>(allowsNull);

        protected override IListenable<T> GetListenable<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => GetStack(enumerable, allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region Properties

        #region this[int]

        [Test]
        public void ItemGet_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var index = Random.Next(int.MinValue, 0);

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemGet_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var index = collection.Count;

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemGet_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var count = collection.Count;
            var index = Random.Next(count + 1, int.MaxValue);

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemGet_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyStack<string>();

            // Act & Assert
            Assert.That(() => collection[0], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemGet_RandomCollectionWithNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetStack(items, true);
            var index = collection.ToArray().IndexOf(null);

            // Act
            var item = collection[index];

            // Act & Assert
            Assert.That(item, Is.Null);
        }

        [Test]
        public void ItemGet_RandomCollectionIndexZero_FirstItem()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var first = collection.First();
            var index = 0;

            // Act
            var item = collection[index];

            // Assert
            Assert.That(item, Is.SameAs(first));
        }

        [Test]
        public void ItemGet_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var last = collection.Last();
            var index = collection.Count - 1;

            // Act
            var item = collection[index];

            // Assert
            Assert.That(item, Is.SameAs(last));
        }

        [Test]
        public void ItemGet_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var array = collection.ToArray();
            var index = GetIndex(collection, Random);

            // Act
            var item = collection[index];

            // Assert
            Assert.That(item, Is.SameAs(array[index]));
        }

        #endregion

        #endregion

        #region Methods

        #region Pop()

        #endregion

        #region Push(T)

        #endregion

        #endregion

        #endregion
    }
}