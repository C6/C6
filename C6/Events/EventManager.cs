// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System;
using System.Diagnostics.Contracts;

using static C6.EventTypes;


namespace C6
{
    // TODO: Merge into CollectionValueBase
    /// <summary>
    /// Manages events for an <see cref="ICollectionValue{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Serializable]
    internal sealed class EventManager<T>
    {
        private event EventHandler _collectionChanged;
        private event EventHandler<ClearedEventArgs> _collectionCleared;
        private event EventHandler<ItemCountEventArgs<T>> _itemsAdded , _itemsRemoved;
        private event EventHandler<ItemAtEventArgs<T>> _itemInserted , _itemRemovedAt;


        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // Active events is equal to the actual active events
            Contract.Invariant(ActiveEvents == (
                (_collectionChanged != null ? Changed : None) |
                (_collectionCleared != null ? Cleared : None) |
                (_itemsAdded != null ? Added : None) |
                (_itemsRemoved != null ? Removed : None) |
                (_itemInserted != null ? Inserted : None) |
                (_itemRemovedAt != null ? RemovedAt : None)
                ));
        }


        [Pure]
        public EventTypes ActiveEvents { get; private set; }


        public event EventHandler CollectionChanged {
            add {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Changed));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Changed));


                _collectionChanged += value;
                ActiveEvents |= Changed;
            }
            remove {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // The event is inactive, if the event handler is null
                Contract.Ensures((_collectionChanged == null) == ActiveEvents.HasFlag(Changed));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) & (_collectionChanged == null ? ~Changed : All)));


                _collectionChanged -= value;
                if (_collectionChanged == null) {
                    ActiveEvents &= ~Changed;
                }
            }
        }


        public event EventHandler<ClearedEventArgs> CollectionCleared {
            add {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Cleared));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Cleared));


                _collectionCleared += value;
                ActiveEvents |= Cleared;
            }
            remove {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // The event is inactive, if the event handler is null
                Contract.Ensures((_collectionCleared == null) == ActiveEvents.HasFlag(Cleared));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) & (_collectionCleared == null ? ~Cleared : All)));


                _collectionCleared -= value;
                if (_collectionCleared == null) {
                    ActiveEvents &= ~Cleared;
                }
            }
        }


        public event EventHandler<ItemCountEventArgs<T>> ItemsAdded {
            add {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Added));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Added));


                _itemsAdded += value;
                ActiveEvents |= Added;
            }
            remove {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // The event is inactive, if the event handler is null
                Contract.Ensures((_itemsAdded == null) == ActiveEvents.HasFlag(Added));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) & (_itemsAdded == null ? ~Added : All)));


                _itemsAdded -= value;
                if (_itemsAdded == null) {
                    ActiveEvents &= ~Added;
                }
            }
        }


        public event EventHandler<ItemCountEventArgs<T>> ItemsRemoved {
            add {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Removed));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Removed));


                _itemsRemoved += value;
                ActiveEvents |= Removed;
            }
            remove {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // The event is inactive, if the event handler is null
                Contract.Ensures((_itemsRemoved == null) == ActiveEvents.HasFlag(Removed));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) & (_itemsRemoved == null ? ~Removed : All)));


                _itemsRemoved -= value;
                if (_itemsRemoved == null) {
                    ActiveEvents &= ~Removed;
                }
            }
        }


        public event EventHandler<ItemAtEventArgs<T>> ItemInserted {
            add {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Inserted));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Inserted));


                _itemInserted += value;
                ActiveEvents |= Inserted;
            }
            remove {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // The event is inactive, if the event handler is null
                Contract.Ensures((_itemInserted == null) == ActiveEvents.HasFlag(Inserted));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) & (_itemInserted == null ? ~Inserted : All)));


                _itemInserted -= value;
                if (_itemInserted == null) {
                    ActiveEvents &= ~Inserted;
                }
            }
        }


        public event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt {
            add {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(RemovedAt));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | RemovedAt));


                _itemRemovedAt += value;
                ActiveEvents |= RemovedAt;
            }
            remove {
                // Value must be non-null
                Contract.Requires(value != null); // TODO: Use <ArgumentNullException>?


                // The event is inactive, if the event handler is null
                Contract.Ensures((_itemRemovedAt == null) == ActiveEvents.HasFlag(RemovedAt));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) & (_itemRemovedAt == null ? ~RemovedAt : All)));


                _itemRemovedAt -= value;
                if (_itemRemovedAt == null) {
                    ActiveEvents &= ~RemovedAt;
                }
            }
        }


        public void OnCollectionChanged(object sender)
        {
            // Argument must be non-null
            Contract.Requires(sender != null); // TODO: Use <ArgumentNullException>?


            _collectionChanged?.Invoke(sender, EventArgs.Empty);
        }


        // TODO: Default arguments are not CLS compliant!
        public void OnCollectionCleared(object sender, bool full, int count, int? start = null)
        {
            // Argument must be non-null
            Contract.Requires(sender != null); // TODO: Use <ArgumentNullException>?

            // Argument must be positive
            Contract.Requires(count > 0);

            // Start is only set, if a list view or index range was cleared
            Contract.Requires(!start.HasValue || !full);


            _collectionCleared?.Invoke(sender, new ClearedEventArgs(full, count, start));
        }


        public void OnItemsAdded(object sender, T item, int count)
        {
            // Argument must be non-null
            Contract.Requires(sender != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-null
            // Contract.Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Argument must be positive
            Contract.Requires(count > 0); // TODO: Use <ArgumentOutOfRangeException>?


            _itemsAdded?.Invoke(sender, new ItemCountEventArgs<T>(item, count));
        }


        public void OnItemsRemoved(object sender, T item, int count)
        {
            // Argument must be non-null
            Contract.Requires(sender != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-null
            // Contract.Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Argument must be positive
            Contract.Requires(count > 0); // TODO: Use <ArgumentOutOfRangeException>?


            _itemsRemoved?.Invoke(sender, new ItemCountEventArgs<T>(item, count));
        }


        public void OnItemInserted(object sender, T item, int index)
        {
            // Argument must be non-null
            Contract.Requires(sender != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-null
            // Contract.Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-negative
            Contract.Requires(index >= 0); // TODO: Use <ArgumentOutOfRangeException>?


            _itemInserted?.Invoke(sender, new ItemAtEventArgs<T>(item, index));
        }


        public void OnItemRemovedAt(object sender, T item, int index)
        {
            // Argument must be non-null
            Contract.Requires(sender != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-null
            // Contract.Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-negative
            Contract.Requires(index >= 0); // TODO: Use <ArgumentOutOfRangeException>?


            _itemRemovedAt?.Invoke(sender, new ItemAtEventArgs<T>(item, index));
        }
    }
}