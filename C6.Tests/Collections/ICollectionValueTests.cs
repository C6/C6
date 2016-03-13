// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;

using NUnit.Framework;
using NUnit.Framework.Internal;

using SCG = System.Collections.Generic;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;
using static C6.Tests.Helpers.TestHelper;


namespace C6.Tests.Collections
{
    [TestFixture]
    public abstract class ICollectionValueTests : IEnumerableTests
    {
        #region Fields

        private readonly EventHandler _changed = (sender, args) => { };
        private readonly EventHandler<ClearedEventArgs> _cleared = (sender, args) => { };
        private readonly EventHandler<ItemCountEventArgs<int>> _added = (sender, args) => { }, _removed = (sender, args) => { };
        private readonly EventHandler<ItemAtEventArgs<int>> _inserted = (sender, args) => { }, _removedAt = (sender, args) => { };

        #endregion

        #region Factories

        /// <summary>
        /// Gets a bit flag indicating the expected value for the collection's
        /// <see cref="ICollectionValue{T}.ListenableEvents"/>.
        /// </summary>
        /// <value>
        /// The bit flag indicating the expected value for the collection's
        /// <see cref="ICollectionValue{T}.ListenableEvents"/>.
        /// </value>
        /// <seealso cref="ICollectionValue{T}.ListenableEvents"/>
        protected abstract EventTypes ListenableEvents { get; }

        /// <summary>
        /// Creates an empty collection value.
        /// </summary>
        /// <param name="allowsNull">A value indicating whether the collection
        /// allows <c>null</c> items.</param>
        /// <typeparam name="T">The type of the items in the collection value.
        /// </typeparam>
        /// <returns>An empty collection value.</returns>
        protected abstract ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false);

        /// <summary>
        /// Creates a collection value containing the items in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection value.
        /// </typeparam>
        /// <param name="enumerable">The collection whose items are copied to
        /// the new collection value.</param>
        /// <param name="allowsNull">A value indicating whether the collection
        /// allows <c>null</c> items.</param>
        /// <returns>A collection value containing the items in the enumerable.
        /// </returns>
        protected abstract ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false);

        #region Helpers

        private ICollectionValue<T> GetCollectionValue<T>(params T[] items) => GetCollectionValue((SCG.IEnumerable<T>) items);

        private ICollectionValue<int> GetRandomIntCollectionValue(Random random, bool allowsNull = false)
            => GetCollectionValue(GetRandomIntEnumerable(random, GetRandomCount(random)), allowsNull);

        private ICollectionValue<int> GetRandomIntCollectionValue(Random random, int count, bool allowsNull = false)
            => GetCollectionValue(GetRandomIntEnumerable(random, count), allowsNull);

        private ICollectionValue<string> GetRandomStringCollectionValue(Randomizer random, bool allowsNull = false)
            => GetCollectionValue(GetRandomStringEnumerable(random, GetRandomCount(random)), allowsNull);

        private ICollectionValue<string> GetRandomStringCollectionValue(Randomizer random, int count, bool allowsNull = false)
            => GetCollectionValue(GetRandomStringEnumerable(random, count), allowsNull);


        private void ListenToEvents(ICollectionValue<int> collection, EventTypes events)
        {
            if (events.HasFlag(Changed)) {
                collection.CollectionChanged += _changed;
            }
            if (events.HasFlag(Cleared)) {
                collection.CollectionCleared += _cleared;
            }
            if (events.HasFlag(Removed)) {
                collection.ItemsRemoved += _removed;
            }
            if (events.HasFlag(Added)) {
                collection.ItemsAdded += _added;
            }
            if (events.HasFlag(Inserted)) {
                collection.ItemInserted += _inserted;
            }
            if (events.HasFlag(RemovedAt)) {
                collection.ItemRemovedAt += _removedAt;
            }
        }

        private void StopListeningToEvents(ICollectionValue<int> collection, EventTypes events)
        {
            if (events.HasFlag(Changed)) {
                collection.CollectionChanged -= _changed;
            }
            if (events.HasFlag(Cleared)) {
                collection.CollectionCleared -= _cleared;
            }
            if (events.HasFlag(Removed)) {
                collection.ItemsRemoved -= _removed;
            }
            if (events.HasFlag(Added)) {
                collection.ItemsAdded -= _added;
            }
            if (events.HasFlag(Inserted)) {
                collection.ItemInserted -= _inserted;
            }
            if (events.HasFlag(RemovedAt)) {
                collection.ItemRemovedAt -= _removedAt;
            }
        }

        #endregion

        #region Inherited

        protected override SCG.IEnumerable<T> GetEmptyEnumerable<T>() => GetEmptyCollectionValue<T>();

        protected override SCG.IEnumerable<T> GetEnumerable<T>(SCG.IEnumerable<T> enumerable) => GetCollectionValue(enumerable);

        #endregion

        #endregion

        #region Test Methods

        #region Properties

        #region Active Events

        [Test]
        public void ActiveEvents_NoActiveEvents_None()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        [Test]
        public void ActiveEvents_ListenToAllListableEvents_EqualsListenableEvents()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();
            var listenableEvents = collection.ListenableEvents;
            ListenToEvents(collection, listenableEvents);

            // Act
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(listenableEvents));
        }

        [Test]
        public void ActiveEvents_ListenToAllListenableEventsThenNone_None()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();
            var listenableEvents = collection.ListenableEvents;
            ListenToEvents(collection, listenableEvents);
            StopListeningToEvents(collection, listenableEvents);

            // Act
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        #endregion

        #region AllowsNull

        // TODO: Are the better tests to perform here?

        [Test]
        public void AllowsNull_EmptyCollectionAllowsNull_True()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>(allowsNull: true);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.True);
        }

        [Test]
        public void AllowsNull_EmptyCollectionAllowsNull_False()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<string>(allowsNull: false);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        [Test]
        public void AllowsNull_AllowsNull_True()
        {
            // Arrange
            var collection = GetCollectionValue(Enumerable.Empty<string>(), allowsNull: true);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.True);
        }

        [Test]
        public void AllowsNull_AllowsNull_False()
        {
            // Arrange
            var collection = GetCollectionValue(Enumerable.Empty<string>(), allowsNull: false);

            // Act
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        #endregion

        #region Count

        [Test]
        public void Count_EmptyCollection_Zero()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

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
            var collection = GetRandomIntCollectionValue(random, size);

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
            var collection = GetEmptyCollectionValue<int>();

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
            var collection = GetEmptyCollectionValue<int>();

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
            var collection = GetRandomIntCollectionValue(random, size);

            // Act
            var isEmpty = collection.IsEmpty;

            // Assert
            Assert.That(isEmpty, Is.False);
        }

        #endregion

        #region Listenable Events

        [Test]
        public void ListenableEvents_EmptyCollection_ListenableEvents()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            var listenableEvents = collection.ListenableEvents;

            // Assert
            Assert.That(listenableEvents, Is.EqualTo(ListenableEvents));
        }

        #endregion

        #endregion

        #region Methods

        #region Choose()

        [Test]
        public void Choose_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.Choose(), Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void Choose_SingleItemCollection_Item()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var item = random.GetString();
            var collection = GetCollectionValue(item);

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(choose, Is.SameAs(item));
        }

        [Test]
        public void Choose_SingleValueTypeCollection_Item()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var item = random.Next();
            var collection = GetCollectionValue(item);

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(choose, Is.EqualTo(item));
        }

        [Test]
        public void Choose_RandomCollection_ItemFromCollection()
        {
            // Arrange
            var collection = GetRandomStringCollectionValue(TestContext.CurrentContext.Random);

            // Act
            var choose = collection.Choose();

            // Assert
            Assert.That(collection, Has.Some.SameAs(choose));
        }

        #endregion

        #region CopyTo(T[], int)

        [Test]
        public void CopyTo_NullArray_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetRandomIntCollectionValue(TestContext.CurrentContext.Random);

            // Act & Assert
            Assert.That(() => collection.CopyTo(null, 0), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CopyTo_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetRandomIntCollectionValue(TestContext.CurrentContext.Random);
            var array = new int[collection.Count];

            // Act & Assert
            Assert.That(() => collection.CopyTo(array, -1), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_IndexOutOfBound_ViolatesPrecondition()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var collection = GetRandomIntCollectionValue(random);
            var array = new int[collection.Count];
            var index = random.Next(1, collection.Count);

            // Act & Assert
            Assert.That(() => collection.CopyTo(array, index), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void CopyTo_EqualSizeArray_Equals()
        {
            // Arrange
            var collection = GetRandomIntCollectionValue(TestContext.CurrentContext.Random);
            var array = new int[collection.Count];

            // Act
            collection.CopyTo(array, 0);

            // Assert
            Assert.That(array, Is.EqualTo(collection));
        }

        [Test]
        public void CopyTo_CopyToRandomIndex_SectionEquals()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var collection = GetRandomIntCollectionValue(random);
            var array = GetRandomIntEnumerable(random, (int) (collection.Count * 1.7)).ToArray();
            var arrayIndex = random.Next(0, array.Length - collection.Count);

            // Act
            collection.CopyTo(array, arrayIndex);
            var section = array.Skip(arrayIndex).Take(collection.Count);

            // Assert
            Assert.That(section, Is.EqualTo(collection));
        }

        #endregion

        #region ToArray()

        [Test]
        public void ToArray_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.Empty);
        }

        [Test]
        public void ToArray_EmptyCollection_NotNull()
        {
            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.Not.Null);
        }

        [Test]
        public void ToArray_SingleItemCollection_SingleItemArray()
        {
            // Arrange
            var random = TestContext.CurrentContext.Random;
            var item = random.GetString();
            var collection = GetCollectionValue(item);
            var itemArray = new[] { item };

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(itemArray));
        }

        [Test]
        public void ToArray_RandomNonValueTypeCollection_Equal()
        {
            // Arrange
            var items = GetRandomStringEnumerable(TestContext.CurrentContext.Random).ToArray();
            var collection = GetCollectionValue(items);

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.That(array, Is.EqualTo(items));
        }

        #endregion

        #endregion

        #region Events

        #region CollectionChanged

        [Test]
        public void CollectionChanged_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            if (ListenableEvents.HasFlag(Changed)) {
                Assert.Pass("Collection allows listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionChanged += _changed, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void CollectionChanged_ListenWithNull_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Changed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionChanged += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CollectionChanged_Listen_BecomesActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Changed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.CollectionChanged += _changed;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Changed));
        }

        [Test]
        public void CollectionChanged_ListenAndUnlisten_BecomesInactiveEvent()
        {
            if (!ListenableEvents.HasFlag(Changed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.CollectionChanged += _changed;
            collection.CollectionChanged -= _changed;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        [Test]
        public void CollectionChanged_ListenTwiceAndUnlistenOnce_RemainsActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Changed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.CollectionChanged += _changed;
            collection.CollectionChanged += _changed;
            collection.CollectionChanged -= _changed;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Changed));
        }

        [Test]
        public void CollectionChanged_RemovesInactiveEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Changed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionChanged -= _changed, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void CollectionChanged_RemovesNullEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Changed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();
            collection.CollectionChanged += _changed;

            // Act & Assert
            Assert.That(() => collection.CollectionChanged -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region CollectionCleared

        [Test]
        public void CollectionCleared_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            if (ListenableEvents.HasFlag(Cleared)) {
                Assert.Pass("Collection allows listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionCleared += _cleared, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void CollectionCleared_ListenWithNull_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Cleared)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionCleared += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CollectionCleared_Listen_BecomesActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Cleared)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.CollectionCleared += _cleared;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Cleared));
        }

        [Test]
        public void CollectionCleared_ListenAndUnlisten_BecomesInactiveEvent()
        {
            if (!ListenableEvents.HasFlag(Cleared)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.CollectionCleared += _cleared;
            collection.CollectionCleared -= _cleared;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        [Test]
        public void CollectionCleared_ListenTwiceAndUnlistenOnce_RemainsActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Cleared)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.CollectionCleared += _cleared;
            collection.CollectionCleared += _cleared;
            collection.CollectionCleared -= _cleared;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Cleared));
        }

        [Test]
        public void CollectionCleared_RemovesInactiveEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Cleared)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionCleared -= _cleared, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void CollectionCleared_RemovesNullEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Cleared)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();
            collection.CollectionCleared += _cleared;

            // Act & Assert
            Assert.That(() => collection.CollectionCleared -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region ItemInserted

        [Test]
        public void ItemInserted_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            if (ListenableEvents.HasFlag(Inserted)) {
                Assert.Pass("Collection allows listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemInserted += _inserted, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void ItemInserted_ListenWithNull_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Inserted)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemInserted += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ItemInserted_Listen_BecomesActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Inserted)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemInserted += _inserted;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Inserted));
        }

        [Test]
        public void ItemInserted_ListenAndUnlisten_BecomesInactiveEvent()
        {
            if (!ListenableEvents.HasFlag(Inserted)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemInserted += _inserted;
            collection.ItemInserted -= _inserted;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        [Test]
        public void ItemInserted_ListenTwiceAndUnlistenOnce_RemainsActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Inserted)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemInserted += _inserted;
            collection.ItemInserted += _inserted;
            collection.ItemInserted -= _inserted;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Inserted));
        }

        [Test]
        public void ItemInserted_RemovesInactiveEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Inserted)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemInserted -= _inserted, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void ItemInserted_RemovesNullEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Inserted)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();
            collection.ItemInserted += _inserted;

            // Act & Assert
            Assert.That(() => collection.ItemInserted -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region ItemRemovedAt

        [Test]
        public void ItemRemovedAt_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            if (ListenableEvents.HasFlag(RemovedAt)) {
                Assert.Pass("Collection allows listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemRemovedAt += _removedAt, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void ItemRemovedAt_ListenWithNull_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(RemovedAt)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemRemovedAt += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ItemRemovedAt_Listen_BecomesActiveEvent()
        {
            if (!ListenableEvents.HasFlag(RemovedAt)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemRemovedAt += _removedAt;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(RemovedAt));
        }

        [Test]
        public void ItemRemovedAt_ListenAndUnlisten_BecomesInactiveEvent()
        {
            if (!ListenableEvents.HasFlag(RemovedAt)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemRemovedAt += _removedAt;
            collection.ItemRemovedAt -= _removedAt;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        [Test]
        public void ItemRemovedAt_ListenTwiceAndUnlistenOnce_RemainsActiveEvent()
        {
            if (!ListenableEvents.HasFlag(RemovedAt)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemRemovedAt += _removedAt;
            collection.ItemRemovedAt += _removedAt;
            collection.ItemRemovedAt -= _removedAt;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(RemovedAt));
        }

        [Test]
        public void ItemRemovedAt_RemovesInactiveEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(RemovedAt)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemRemovedAt -= _removedAt, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void ItemRemovedAt_RemovesNullEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(RemovedAt)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();
            collection.ItemRemovedAt += _removedAt;

            // Act & Assert
            Assert.That(() => collection.ItemRemovedAt -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region ItemsAdded

        [Test]
        public void ItemsAdded_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            if (ListenableEvents.HasFlag(Added)) {
                Assert.Pass("Collection allows listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsAdded += _added, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void ItemsAdded_ListenWithNull_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Added)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsAdded += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ItemsAdded_Listen_BecomesActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Added)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemsAdded += _added;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Added));
        }

        [Test]
        public void ItemsAdded_ListenAndUnlisten_BecomesInactiveEvent()
        {
            if (!ListenableEvents.HasFlag(Added)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemsAdded += _added;
            collection.ItemsAdded -= _added;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        [Test]
        public void ItemsAdded_ListenTwiceAndUnlistenOnce_RemainsActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Added)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemsAdded += _added;
            collection.ItemsAdded += _added;
            collection.ItemsAdded -= _added;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Added));
        }

        [Test]
        public void ItemsAdded_RemovesInactiveEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Added)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsAdded -= _added, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void ItemsAdded_RemovesNullEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Added)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();
            collection.ItemsAdded += _added;

            // Act & Assert
            Assert.That(() => collection.ItemsAdded -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region ItemsRemoved

        [Test]
        public void ItemsRemoved_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            if (ListenableEvents.HasFlag(Removed)) {
                Assert.Pass("Collection allows listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsRemoved += _removed, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void ItemsRemoved_ListenWithNull_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Removed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsRemoved += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ItemsRemoved_Listen_BecomesActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Removed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemsRemoved += _removed;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Removed));
        }

        [Test]
        public void ItemsRemoved_ListenAndUnlisten_BecomesInactiveEvent()
        {
            if (!ListenableEvents.HasFlag(Removed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemsRemoved += _removed;
            collection.ItemsRemoved -= _removed;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        [Test]
        public void ItemsRemoved_ListenTwiceAndUnlistenOnce_RemainsActiveEvent()
        {
            if (!ListenableEvents.HasFlag(Removed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act
            collection.ItemsRemoved += _removed;
            collection.ItemsRemoved += _removed;
            collection.ItemsRemoved -= _removed;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Removed));
        }

        [Test]
        public void ItemsRemoved_RemovesInactiveEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Removed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsRemoved -= _removed, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void ItemsRemoved_RemovesNullEvent_ViolatesPrecondition()
        {
            if (!ListenableEvents.HasFlag(Removed)) {
                Assert.Pass("Collection does not allow listening to this event."); // TODO: Ignore instead?
            }

            // Arrange
            var collection = GetEmptyCollectionValue<int>();
            collection.ItemsRemoved += _removed;

            // Act & Assert
            Assert.That(() => collection.ItemsRemoved -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #endregion

        #endregion
    }
}