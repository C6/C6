// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;

using static C6.EventTypes;


namespace C6.Tests.Helpers
{
    public sealed class CollectionEvent<T>
    {
        private readonly EventTypes _eventType;
        private readonly EventArgs _eventArgs;
        private readonly object _sender;

        public CollectionEvent(EventTypes eventType, EventArgs eventArgs, object sender)
        {
            #region Code Contracts

            // Argument must be valid enum constant
            Contract.Requires(Enum.IsDefined(typeof(EventTypes), eventType), EnumMustBeDefined);

            // Argument must be non-null
            Contract.Requires(eventArgs != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Contract.Requires(sender != null, ArgumentMustBeNonNull);

            // Event arguments must match event type
            Contract.Requires(
                (eventType == Changed && eventArgs.GetType() == typeof(EventArgs)) ||
                (eventType == Cleared && eventArgs is ClearedEventArgs) ||
                (eventType == Added && eventArgs is ItemCountEventArgs<T>) ||
                (eventType == Removed && eventArgs is ItemCountEventArgs<T>) ||
                (eventType == Inserted && eventArgs is ItemAtEventArgs<T>) ||
                (eventType == RemovedAt && eventArgs is ItemAtEventArgs<T>),
                "Event arguments must match event type" // TODO: Use string?
                );

            #endregion

            _eventType = eventType;
            _eventArgs = eventArgs;
            _sender = sender;
        }

        public bool Equals(CollectionEvent<T> otherEvent, SCG.IEqualityComparer<T> equalityComparer)
        {
            if (otherEvent == null || _eventType != otherEvent._eventType || !ReferenceEquals(_sender, otherEvent._sender)) {
                return false;
            }

            switch (_eventType) {
                case Changed:
                    return true;

                case Cleared:
                    return ((ClearedEventArgs) _eventArgs).Equals((ClearedEventArgs) otherEvent._eventArgs);

                case Added:
                case Removed:
                    return ((ItemCountEventArgs<T>) _eventArgs).Equals((ItemCountEventArgs<T>) otherEvent._eventArgs, equalityComparer);

                case Inserted:
                case RemovedAt:
                    return ((ItemAtEventArgs<T>) _eventArgs).Equals((ItemAtEventArgs<T>) otherEvent._eventArgs, equalityComparer);

                default:
                    throw new ApplicationException($"Illegal Action: {_eventType}");
            }
        }

        public override string ToString() => $"Event: {_eventType}, Arguments : {_eventArgs}, Sender : {_sender}";
    }


    public static class CollectionEvent
    {
        public static CollectionEvent<T> Added<T>(T item, int count, ICollectionValue<T> sender) => new CollectionEvent<T>(EventTypes.Added, new ItemCountEventArgs<T>(item, count), sender);
        public static CollectionEvent<T> Changed<T>(ICollectionValue<T> sender) => new CollectionEvent<T>(EventTypes.Changed, EventArgs.Empty, sender);
        public static CollectionEvent<T> Inserted<T>(T item, int index, ICollectionValue<T> sender) => new CollectionEvent<T>(EventTypes.Inserted, new ItemAtEventArgs<T>(item, index), sender);
        public static CollectionEvent<T> Removed<T>(T item, int count, ICollectionValue<T> sender) => new CollectionEvent<T>(EventTypes.Removed, new ItemCountEventArgs<T>(item, count), sender);
        public static CollectionEvent<T> RemovedAt<T>(T item, int index, ICollectionValue<T> sender) => new CollectionEvent<T>(EventTypes.RemovedAt, new ItemAtEventArgs<T>(item, index), sender);

        public static CollectionEvent<T> Cleared<T>(bool full, int count, int? start, ICollectionValue<T> sender) => new CollectionEvent<T>(EventTypes.Cleared, new ClearedEventArgs(full, count, start), sender);
    }
}