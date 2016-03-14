// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using SCG = System.Collections.Generic;

using static C6.EventTypes;


namespace C6.Tests.Helpers
{
    public class CollectionEventConstraint<T> : Constraint
    {
        private readonly ICollectionValue<T> _collection;
        private readonly SCG.IEnumerable<CollectionEvent<T>> _expectedEvents;
        private readonly SCG.IList<CollectionEvent<T>> _actualEvents; // TODO: Change to C5.IList<T>

        public CollectionEventConstraint(ICollectionValue<T> collection, CollectionEvent<T>[] expectedEvents)
        {
            _collection = collection;
            _expectedEvents = expectedEvents;

            _actualEvents = new SCG.List<CollectionEvent<T>>(); // TODO: Change to C5.ArrayList<T>

            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            _collection.CollectionChanged += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Changed, EventArgs.Empty, sender));
            _collection.CollectionCleared += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Cleared, eventArgs, sender));
            _collection.ItemsRemoved += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Removed, eventArgs, sender));
            _collection.ItemsAdded += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Added, eventArgs, sender));
            _collection.ItemInserted += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Inserted, eventArgs, sender));
            _collection.ItemRemovedAt += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(RemovedAt, eventArgs, sender));
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            // Run the code that raises events
            (actual as TestDelegate)?.Invoke();

            return new ConstraintResult(this, _actualEvents, IsSuccess());
        }

        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            // Run the code that raises events
            del();

            return new ConstraintResult(this, _actualEvents, IsSuccess());
        }

        private bool IsSuccess()
        {
            // TODO: Find a better solution for this
            var equalityComparer = (_collection as IExtensible<T>)?.EqualityComparer ?? SCG.EqualityComparer<T>.Default;

            var i = 0;
            foreach (var expectedEvent in _expectedEvents) {
                if (i >= _actualEvents.Count) {
                    Assert.Fail($"Event number {i} did not happen:\n expected {expectedEvent}");
                }

                if (!expectedEvent.Equals(_actualEvents[i], equalityComparer)) {
                    Assert.Fail($"Event number {i}:\n expected {expectedEvent}\n but saw {_actualEvents[i]}");
                }

                i++;
            }

            if (i < _actualEvents.Count) {
                Assert.Fail($"Event number {i} seen but no event expected:\n {_actualEvents[i]}");
            }

            _actualEvents.Clear();

            return true;
        }

        public override string Description => _expectedEvents.ToString();
    }
}