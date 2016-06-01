// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Collections.ExceptionMessages;
using static C6.Contracts.ContractMessage;
using static C6.Tests.Helpers.CollectionEvent;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class IStackTests : IListenableTests
    {
        #region Factories

        protected abstract bool IsReadOnly { get; }

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

        [Test]
        public void ItemGet_GetItemDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var index = GetIndex(collection, Random);
            var expected = collection.ElementAt(index);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            var item = collection[index];

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
            Assert.That(item, Is.EqualTo(expected));
        }

        #endregion

        #endregion

        #region Methods

        #region Pop()

        [Test]
        public void Pop_EmptyCollection_ViolatesPrecondtion()
        {
            // Arrange
            var collection = GetEmptyStack<string>();

            // Act & Assert
            Assert.That(() => collection.Pop(), Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void Pop_PopDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringStack(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Pop();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void Pop_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var item = collection.Last();
            var expectedEvents = new[] {
                RemovedAt(item, collection.Count - 1, collection),
                Removed(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.Pop(), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Pop_RandomCollectionWithNullRemoveNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).Append(null);
            var collection = GetStack(items, allowsNull: true);

            // Act
            var removeLast = collection.Pop();

            // Assert
            Assert.That(removeLast, Is.Null);
        }

        [Test]
        public void Pop_SingleItemCollection_Empty()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetStack(itemArray);

            // Act
            var removeLast = collection.Pop();

            // Assert
            Assert.That(removeLast, Is.SameAs(item));
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Pop_PopItem_Removed()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var lastItem = collection.Last();
            var array = collection.Take(collection.Count - 1).ToArray();

            // Act
            var removeLast = collection.Pop();

            // Assert
            Assert.That(removeLast, Is.SameAs(lastItem));
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Pop_RandomCollectionPopUntilEmpty_Empty()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var count = collection.Count;

            // Act
            for (var i = 0; i < count; i++) {
                collection.Pop();
            }

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        [Category("Unfinished")]
        public void Pop_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Push(T)

        [Test]
        public void Push_DisallowsNullPushNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringStack(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.Push(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Push_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringStack(Random, allowsNull: true);
            var array = collection.Append(null).ToArray();

            // Act
            collection.Push(null);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Push_EmptyCollection_SingleItemCollection()
        {
            // Arrange
            var collection = GetEmptyStack<string>();
            var item = Random.GetString();
            var array = new[] { item };

            // Act
            collection.Push(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Push_RandomCollectionInsertExistingLast_InsertedLast()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var item = collection.ToArray().Choose(Random);
            var array = collection.Append(item).ToArray();

            // Act
            collection.Push(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Push_RandomCollectionPush_InsertedLast()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var item = Random.GetString();
            var array = collection.Append(item).ToArray();

            // Act
            collection.Push(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Push_ManyItems_Equal()
        {
            // Arrange
            var collection = GetEmptyStack<string>();
            var items = GetStrings(Random, Random.Next(100, 250));

            // Act
            foreach (var item in items) {
                collection.Push(item);
            }

            // Assert
            Assert.That(collection, Is.EqualTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Push_RandomCollectionPush_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var item = Random.GetString();
            var expectedEvents = new[] {
                Inserted(item, collection.Count, collection),
                Added(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.Push(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Push_PushItemDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringStack(Random);
            var item = Random.GetString();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Push(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void Push_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion
    }
}