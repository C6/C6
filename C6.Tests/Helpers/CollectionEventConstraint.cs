// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using C6.Collections;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using static C6.EventTypes;

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class CollectionEventConstraint<T> : Constraint
    {
        private readonly IListenable<T> _collection;
        private readonly SCG.IEnumerable<CollectionEvent<T>> _expectedEvents;
        private readonly IList<CollectionEvent<T>> _actualEvents;

        public CollectionEventConstraint(IListenable<T> collection, CollectionEvent<T>[] expectedEvents)
        {
            _collection = collection;
            _expectedEvents = expectedEvents;

            _actualEvents = new ArrayList<CollectionEvent<T>>();

            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            if (_collection.ListenableEvents.HasFlag(Changed)) {
                _collection.CollectionChanged += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Changed, EventArgs.Empty, sender));
            }
            if (_collection.ListenableEvents.HasFlag(Cleared)) {
                _collection.CollectionCleared += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Cleared, eventArgs, sender));
            }
            if (_collection.ListenableEvents.HasFlag(Removed)) {
                _collection.ItemsRemoved += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Removed, eventArgs, sender));
            }
            if (_collection.ListenableEvents.HasFlag(Added)) {
                _collection.ItemsAdded += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Added, eventArgs, sender));
            }
            if (_collection.ListenableEvents.HasFlag(Inserted)) {
                _collection.ItemInserted += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(Inserted, eventArgs, sender));
            }
            if (_collection.ListenableEvents.HasFlag(RemovedAt)) {
                _collection.ItemRemovedAt += (sender, eventArgs) => _actualEvents.Add(new CollectionEvent<T>(RemovedAt, eventArgs, sender));
            }
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
            var i = 0;
            foreach (var expectedEvent in _expectedEvents) {
                if (i >= _actualEvents.Count) {
                    Assert.Fail($"Event number {i} did not happen:\n expected {expectedEvent}");
                }

                if (!expectedEvent.Equals(_actualEvents[i])) {
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