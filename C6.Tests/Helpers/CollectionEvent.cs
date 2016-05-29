// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using C6.Contracts;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;


namespace C6.Tests.Helpers
{
    public sealed class CollectionEvent<T> : IEquatable<CollectionEvent<T>>
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
                EventArgumentMustMatchEventType
                );

            #endregion

            _eventType = eventType;
            _eventArgs = eventArgs;
            _sender = sender;
        }

        public bool Equals(CollectionEvent<T> otherEvent)
        {
            if (otherEvent == null || _eventType != otherEvent._eventType || !ReferenceEquals(_sender, otherEvent._sender)) {
                return false;
            }

            switch (_eventType) {
                case Changed:
                    return true;

                case Cleared: {
                    var x = _eventArgs as ClearedEventArgs;
                    var y = otherEvent._eventArgs as ClearedEventArgs;
                    return x != null && y != null && x.Count == y.Count && x.Full == y.Full && x.Start == y.Start;
                }

                case Added:
                case Removed: {
                    var x = _eventArgs as ItemCountEventArgs<T>;
                    var y = otherEvent._eventArgs as ItemCountEventArgs<T>;
                    return y != null && x != null && x.Count == y.Count && x.Item.IsSameAs(y.Item);
                }

                case Inserted:
                case RemovedAt: {
                    var x = _eventArgs as ItemAtEventArgs<T>;
                    var y = otherEvent._eventArgs as ItemAtEventArgs<T>;
                    return y != null && x != null && x.Index == y.Index && x.Item.IsSameAs(y.Item);
                }

                default:
                    throw new ApplicationException($"Illegal Action: {_eventType}");
            }
        }

        public override string ToString() => $"{_eventType} {_eventArgs} from {_sender}";
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