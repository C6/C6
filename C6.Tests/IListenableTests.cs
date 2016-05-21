// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class IListenableTests : ICollectionValueTests
    {
        #region Fields

        private readonly EventHandler _changed = (sender, args) => { };
        private readonly EventHandler<ClearedEventArgs> _cleared = (sender, args) => { };
        private readonly EventHandler<ItemCountEventArgs<int>> _added = (sender, args) => { }, _removed = (sender, args) => { };
        private readonly EventHandler<ItemAtEventArgs<int>> _inserted = (sender, args) => { }, _removedAt = (sender, args) => { };

        #endregion

        #region Factories

        /// <summary>
        ///     Gets a bit flag indicating the expected value for the collection's
        ///     <see cref="IListenable{T}.ListenableEvents"/>.
        /// </summary>
        /// <value>
        ///     The bit flag indicating the expected value for the collection's
        ///     <see cref="IListenable{T}.ListenableEvents"/>.
        /// </value>
        /// <seealso cref="IListenable{T}.ListenableEvents"/>
        protected abstract EventTypes ListenableEvents { get; }

        /// <summary>
        ///     Creates an empty <see cref="IQueue{T}"/>.
        /// </summary>
        /// <param name="allowsNull">
        ///     A value indicating whether the collection allows <c>null</c> items.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the items in the <see cref="IQueue{T}"/>.
        /// </typeparam>
        /// <returns>
        ///     An empty <see cref="IQueue{T}"/>.
        /// </returns>
        protected abstract IListenable<T> GetEmptyListenable<T>(bool allowsNull = false);

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
        ///     A value indicating whether the collection allows <c>null</c> items.
        /// </param>
        /// <returns>
        ///     A <see cref="IQueue{T}"/> containing the items in the enumerable.
        /// </returns>
        protected abstract IListenable<T> GetListenable<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false);

        #region Helpers

        private void ListenToEvents(IListenable<int> collection, EventTypes events)
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

        private void StopListeningToEvents(IListenable<int> collection, EventTypes events)
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

        protected override ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false) => GetEmptyListenable<T>(allowsNull);

        protected override ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => GetListenable(enumerable, allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region Properties

        #region Active Events

        [Test]
        public void ActiveEvents_NoActiveEvents_None()
        {
            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        [Test]
        public void ActiveEvents_ListenToAllListableEvents_EqualsListenableEvents()
        {
            // Arrange
            var collection = GetEmptyListenable<int>();
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
            var collection = GetEmptyListenable<int>();
            var listenableEvents = collection.ListenableEvents;

            // Act
            ListenToEvents(collection, listenableEvents);
            StopListeningToEvents(collection, listenableEvents);
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(None));
        }

        #endregion

        #region Listenable Events

        [Test]
        public void ListenableEvents_EmptyCollection_ListenableEvents()
        {
            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act
            var listenableEvents = collection.ListenableEvents;

            // Assert
            Assert.That(listenableEvents, Is.EqualTo(ListenableEvents));
        }

        #endregion

        #endregion

        #region Events

        #region CollectionChanged

        [Test]
        public void CollectionChanged_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            Run.If(!ListenableEvents.HasFlag(Changed));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionChanged += _changed, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void CollectionChanged_ListenWithNull_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Changed));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionChanged += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CollectionChanged_Listen_BecomesActiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Changed));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act
            collection.CollectionChanged += _changed;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Changed));
        }

        [Test]
        public void CollectionChanged_ListenAndUnlisten_BecomesInactiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Changed));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Changed));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Changed));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionChanged -= _changed, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void CollectionChanged_RemovesNullEvent_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Changed));

            // Arrange
            var collection = GetEmptyListenable<int>();
            collection.CollectionChanged += _changed;

            // Act & Assert
            Assert.That(() => collection.CollectionChanged -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region CollectionCleared

        [Test]
        public void CollectionCleared_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            Run.If(!ListenableEvents.HasFlag(Cleared));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionCleared += _cleared, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void CollectionCleared_ListenWithNull_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Cleared));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionCleared += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void CollectionCleared_Listen_BecomesActiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Cleared));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act
            collection.CollectionCleared += _cleared;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Cleared));
        }

        [Test]
        public void CollectionCleared_ListenAndUnlisten_BecomesInactiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Cleared));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Cleared));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Cleared));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.CollectionCleared -= _cleared, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void CollectionCleared_RemovesNullEvent_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Cleared));

            // Arrange
            var collection = GetEmptyListenable<int>();
            collection.CollectionCleared += _cleared;

            // Act & Assert
            Assert.That(() => collection.CollectionCleared -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region ItemInserted

        [Test]
        public void ItemInserted_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            Run.If(!ListenableEvents.HasFlag(Inserted));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemInserted += _inserted, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void ItemInserted_ListenWithNull_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Inserted));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemInserted += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ItemInserted_Listen_BecomesActiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Inserted));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act
            collection.ItemInserted += _inserted;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Inserted));
        }

        [Test]
        public void ItemInserted_ListenAndUnlisten_BecomesInactiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Inserted));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Inserted));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Inserted));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemInserted -= _inserted, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void ItemInserted_RemovesNullEvent_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Inserted));

            // Arrange
            var collection = GetEmptyListenable<int>();
            collection.ItemInserted += _inserted;

            // Act & Assert
            Assert.That(() => collection.ItemInserted -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region ItemRemovedAt

        [Test]
        public void ItemRemovedAt_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            Run.If(!ListenableEvents.HasFlag(RemovedAt));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemRemovedAt += _removedAt, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void ItemRemovedAt_ListenWithNull_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(RemovedAt));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemRemovedAt += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ItemRemovedAt_Listen_BecomesActiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(RemovedAt));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act
            collection.ItemRemovedAt += _removedAt;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(RemovedAt));
        }

        [Test]
        public void ItemRemovedAt_ListenAndUnlisten_BecomesInactiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(RemovedAt));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(RemovedAt));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(RemovedAt));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemRemovedAt -= _removedAt, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void ItemRemovedAt_RemovesNullEvent_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(RemovedAt));

            // Arrange
            var collection = GetEmptyListenable<int>();
            collection.ItemRemovedAt += _removedAt;

            // Act & Assert
            Assert.That(() => collection.ItemRemovedAt -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region ItemsAdded

        [Test]
        public void ItemsAdded_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            Run.If(!ListenableEvents.HasFlag(Added));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsAdded += _added, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void ItemsAdded_ListenWithNull_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Added));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsAdded += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ItemsAdded_Listen_BecomesActiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Added));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act
            collection.ItemsAdded += _added;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Added));
        }

        [Test]
        public void ItemsAdded_ListenAndUnlisten_BecomesInactiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Added));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Added));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Added));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsAdded -= _added, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void ItemsAdded_RemovesNullEvent_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Added));

            // Arrange
            var collection = GetEmptyListenable<int>();
            collection.ItemsAdded += _added;

            // Act & Assert
            Assert.That(() => collection.ItemsAdded -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #region ItemsRemoved

        [Test]
        public void ItemsRemoved_ListenToUnlistenableEvent_ViolatesPrecondition()
        {
            Run.If(!ListenableEvents.HasFlag(Removed));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsRemoved += _removed, Violates.PreconditionSaying(EventMustBeListenable));
        }

        [Test]
        public void ItemsRemoved_ListenWithNull_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Removed));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsRemoved += null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ItemsRemoved_Listen_BecomesActiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Removed));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act
            collection.ItemsRemoved += _removed;
            var activeEvents = collection.ActiveEvents;

            // Assert
            Assert.That(activeEvents, Is.EqualTo(Removed));
        }

        [Test]
        public void ItemsRemoved_ListenAndUnlisten_BecomesInactiveEvent()
        {
            Run.If(ListenableEvents.HasFlag(Removed));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Removed));

            // Arrange
            var collection = GetEmptyListenable<int>();

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
            Run.If(ListenableEvents.HasFlag(Removed));

            // Arrange
            var collection = GetEmptyListenable<int>();

            // Act & Assert
            Assert.That(() => collection.ItemsRemoved -= _removed, Violates.PreconditionSaying(EventMustBeActive));
        }

        [Test]
        public void ItemsRemoved_RemovesNullEvent_ViolatesPrecondition()
        {
            Run.If(ListenableEvents.HasFlag(Removed));

            // Arrange
            var collection = GetEmptyListenable<int>();
            collection.ItemsRemoved += _removed;

            // Act & Assert
            Assert.That(() => collection.ItemsRemoved -= null, Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        #endregion

        #endregion

        #endregion
    }
}