// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.Tests.Helpers.CollectionEvent;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class IQueueTests : IListenableTests
    {
        #region Factories

        protected abstract bool IsReadOnly { get; }

        /// <summary>
        ///     Creates an empty <see cref="IQueue{T}"/>.
        /// </summary>
        /// <param name="allowsNull">
        ///     A value indicating whether the <see cref="IQueue{T}"/> allows <c>null</c> items.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the items in the <see cref="IQueue{T}"/>.
        /// </typeparam>
        /// <returns>
        ///     An empty <see cref="IQueue{T}"/>.
        /// </returns>
        protected abstract IQueue<T> GetEmptyQueue<T>(bool allowsNull = false);

        /// <summary>
        ///     Creates a <see cref="IQueue{T}"/> containing the items in the enumerable.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the <see cref="IQueue{T}"/>.
        /// </typeparam>
        /// <param name="enumerable">
        ///     The collection whose items are copied to the new <see cref="IQueue{T}"/>.
        /// </param>
        /// <param name="allowsNull">
        ///     A value indicating whether the <see cref="IQueue{T}"/> allows <c>null</c> items.
        /// </param>
        /// <returns>
        ///     A <see cref="IQueue{T}"/> containing the items in the enumerable.
        /// </returns>
        protected abstract IQueue<T> GetQueue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false);

        #region Helpers

        private IQueue<string> GetStringQueue(Randomizer random, bool allowsNull = false)
            => GetQueue(GetStrings(random, GetCount(random)), allowsNull);

        #endregion

        #region Inherited

        protected override IListenable<T> GetEmptyListenable<T>(bool allowsNull = false) => GetEmptyQueue<T>(allowsNull);

        protected override IListenable<T> GetListenable<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => GetQueue(enumerable, allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region Properties

        #region this[int]

        [Test]
        public void ItemGet_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var index = GetNegative(Random);

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemGet_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var index = collection.Count;

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemGet_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var count = collection.Count;
            var index = Random.Next(count + 1, int.MaxValue);

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemGet_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyQueue<string>();
            var index = 0;

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemGet_RandomCollectionWithNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetQueue(items, true);
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
            var collection = GetStringQueue(Random);
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
            var collection = GetStringQueue(Random);
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
            var collection = GetStringQueue(Random);
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
            var collection = GetStringQueue(Random);
            var index = GetIndex(collection, Random);
            var expected = collection.ElementAt(index);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            var item = collection[index];

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
            Assert.That(item, Is.SameAs(expected));
        }

        #endregion

        #endregion

        #region Methods

        #region Dequeue()

        [Test]
        public void Dequeue_EmptyCollection_ViolatesPrecondtion()
        {
            // Arrange
            var collection = GetEmptyQueue<string>();

            // Act & Assert
            Assert.That(() => collection.Dequeue(), Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void Dequeue_DequeueDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringQueue(Random);

            // Act & Assert
            Assert.That(() => collection.Dequeue(), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void Dequeue_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var item = collection.First();
            var expectedEvents = new[] {
                RemovedAt(item, 0, collection),
                Removed(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.Dequeue(), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Dequeue_AllowsNull_Null()
        {
            // Arrange
            var items = GetStrings(Random);
            items[0] = null;
            var collection = GetQueue(items, allowsNull: true);

            // Act
            var dequeue = collection.Dequeue();

            // Assert
            Assert.That(dequeue, Is.Null);
        }

        [Test]
        public void Dequeue_SingleItemCollection_Empty()
        {
            // Arrange
            var item = GetString(Random);
            var itemArray = new[] { item };
            var collection = GetQueue(itemArray);

            // Act
            var dequeue = collection.Dequeue();

            // Assert
            Assert.That(dequeue, Is.SameAs(item));
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Dequeue_DequeueItem_Removed()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var firstItem = collection.First();
            var array = collection.Skip(1).ToArray();

            // Act
            var dequeue = collection.Dequeue();

            // Assert
            Assert.That(dequeue, Is.SameAs(firstItem));
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Dequeue_RandomCollectionDequeueUntilEmpty_Empty()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var count = collection.Count;

            // Act
            for (var i = 0; i < count; i++) {
                collection.Dequeue();
            }

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        [Category("Unfinished")]
        public void Dequeue_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Enqueue(T)

        [Test]
        public void Enqueue_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringQueue(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.Enqueue(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Enqueue_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringQueue(Random, allowsNull: true);
            var array = collection.Append(null).ToArray();

            // Act
            collection.Enqueue(null);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Enqueue_EmptyCollection_SingleItemCollection()
        {
            // Arrange
            var collection = GetEmptyQueue<string>();
            var item = GetString(Random);
            var array = new[] { item };

            // Act
            collection.Enqueue(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Enqueue_RandomCollectionInsertExistingLast_InsertedLast()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var item = collection.Choose(Random);
            var array = collection.Append(item).ToArray();

            // Act
            collection.Enqueue(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Enqueue_RandomCollectionEnqueue_InsertedLast()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var item = GetString(Random);
            var array = collection.Append(item).ToArray();

            // Act
            collection.Enqueue(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Enqueue_ManyItems_Equal()
        {
            // Arrange
            var collection = GetEmptyQueue<string>();
            var items = GetStrings(Random, Random.Next(100, 250));

            // Act
            foreach (var item in items) {
                collection.Enqueue(item);
            }

            // Assert
            Assert.That(collection, Is.EqualTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Enqueue_RandomCollectionEnqueue_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var item = GetString(Random);
            var expectedEvents = new[] {
                Inserted(item, collection.Count, collection),
                Added(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.Enqueue(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Enqueue_EnqueueDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringQueue(Random);
            var item = GetString(Random);

            // Act & Assert
            Assert.That(() => collection.Enqueue(item), Breaks.EnumeratorFor(collection));
        }

        [Test]
        [Category("Unfinished")]
        public void Enqueue_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion
    }
}