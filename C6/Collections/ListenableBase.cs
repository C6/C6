// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using static C6.EventTypes;


namespace C6.Collections
{
    [Serializable]
    public abstract class ListenableBase<T> : CollectionValueBase<T>, IListenable<T>
    {
        #region Fields

        private event EventHandler _collectionChanged;
        private event EventHandler<ClearedEventArgs> _collectionCleared;
        private event EventHandler<ItemAtEventArgs<T>> _itemInserted , _itemRemovedAt;
        private event EventHandler<ItemCountEventArgs<T>> _itemsAdded , _itemsRemoved;

        #endregion

        #region Constructor

        protected ListenableBase(bool allowsNull = false, EventTypes listenableEvents = All) : base(allowsNull)
        {
            ListenableEvents = listenableEvents;
        }

        #endregion

        #region Properties

        public virtual EventTypes ActiveEvents { get; private set; }

        public EventTypes ListenableEvents { get; }

        #endregion

        #region Events

        public virtual event EventHandler CollectionChanged
        {
            add {
                _collectionChanged += value;
                ActiveEvents |= Changed;
            }
            remove {
                _collectionChanged -= value;
                if (_collectionChanged == null) {
                    ActiveEvents &= ~Changed;
                }
            }
        }

        public virtual event EventHandler<ClearedEventArgs> CollectionCleared
        {
            add {
                _collectionCleared += value;
                ActiveEvents |= Cleared;
            }
            remove {
                _collectionCleared -= value;
                if (_collectionCleared == null) {
                    ActiveEvents &= ~Cleared;
                }
            }
        }

        public virtual event EventHandler<ItemAtEventArgs<T>> ItemInserted
        {
            add {
                _itemInserted += value;
                ActiveEvents |= Inserted;
            }
            remove {
                _itemInserted -= value;
                if (_itemInserted == null) {
                    ActiveEvents &= ~Inserted;
                }
            }
        }

        public virtual event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt
        {
            add {
                _itemRemovedAt += value;
                ActiveEvents |= RemovedAt;
            }
            remove {
                _itemRemovedAt -= value;
                if (_itemRemovedAt == null) {
                    ActiveEvents &= ~RemovedAt;
                }
            }
        }

        public virtual event EventHandler<ItemCountEventArgs<T>> ItemsAdded
        {
            add {
                _itemsAdded += value;
                ActiveEvents |= Added;
            }
            remove {
                _itemsAdded -= value;
                if (_itemsAdded == null) {
                    ActiveEvents &= ~Added;
                }
            }
        }

        public virtual event EventHandler<ItemCountEventArgs<T>> ItemsRemoved
        {
            add {
                _itemsRemoved += value;
                ActiveEvents |= Removed;
            }
            remove {
                _itemsRemoved -= value;
                if (_itemsRemoved == null) {
                    ActiveEvents &= ~Removed;
                }
            }
        }

        #endregion

        #region Invoking Methods

        protected void OnCollectionChanged()
            => _collectionChanged?.Invoke(this, EventArgs.Empty);

        protected void OnCollectionCleared(bool full, int count, int? start = null)
            => _collectionCleared?.Invoke(this, new ClearedEventArgs(full, count, start));

        protected void OnItemsAdded(T item, int count)
            => _itemsAdded?.Invoke(this, new ItemCountEventArgs<T>(item, count));

        protected void OnItemsRemoved(T item, int count)
            => _itemsRemoved?.Invoke(this, new ItemCountEventArgs<T>(item, count));

        protected void OnItemInserted(T item, int index)
            => _itemInserted?.Invoke(this, new ItemAtEventArgs<T>(item, index));

        protected void OnItemRemovedAt(T item, int index)
            => _itemRemovedAt?.Invoke(this, new ItemAtEventArgs<T>(item, index));

        #endregion
    }
}