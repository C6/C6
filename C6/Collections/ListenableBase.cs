// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using SCG = System.Collections.Generic;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;
using static C6.Collections.ExceptionMessages;


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

        #region Properties

        public virtual EventTypes ActiveEvents { get; private set; }

        public abstract EventTypes ListenableEvents { get; }

        #endregion

        #region Methods

        protected void RaiseForAdd(T item)
        {
            Requires(AllowsNull || item != null, ItemMustBeNonNull);

            OnItemsAdded(item, 1);
            OnCollectionChanged();
        }

        protected void RaiseForAddRange(SCG.IEnumerable<T> items)
        {
            Requires(items != null, ArgumentMustBeNonNull);
            Requires(AllowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);

            if (ActiveEvents.HasFlag(Added)) {
                foreach (var item in items) {
                    OnItemsAdded(item, 1);
                }
            }
            OnCollectionChanged();
        }

        protected void RaiseForClear(int count)
        {
            Requires(count >= 1, ArgumentMustBePositive);

            OnCollectionCleared(true, count);
            OnCollectionChanged();
        }

        protected void RaiseForEnqueue(T item)
        {
            OnItemInserted(item, Count - 1);
            OnItemsAdded(item, 1);
            OnCollectionChanged();
        }

        protected void RaiseForIndexSetter(T oldItem, T newItem, int index)
        {
            if (ActiveEvents != None) {
                OnItemRemovedAt(oldItem, index);
                OnItemsRemoved(oldItem, 1);
                OnItemInserted(newItem, index);
                OnItemsAdded(newItem, 1);
                OnCollectionChanged();
            }
        }

        protected void RaiseForInsert(int index, T item)
        {
            OnItemInserted(item, index);
            OnItemsAdded(item, 1);
            OnCollectionChanged();
        }

        protected void RaiseForInsertRange(int index, SCG.IEnumerable<T> items)
        {
            if (ActiveEvents.HasFlag(Inserted | Added)) {
                var offset = 0;
                foreach (var item in items) {
                    OnItemInserted(item, index + offset++);
                    OnItemsAdded(item, 1);
                }
            }
            OnCollectionChanged();
        }

        protected void RaiseForRemove(T removedItem)
        {
            OnItemsRemoved(removedItem, 1);
            OnCollectionChanged();
        }

        protected void RaiseForRemoveAt(T removedItem, int index)
        {
            OnItemRemovedAt(removedItem, index);
            OnItemsRemoved(removedItem, 1);
            OnCollectionChanged();
        }

        protected void RaiseForRemoveIndexRange(int startIndex, int count)
        {
            OnCollectionCleared(false, count, startIndex);
            OnCollectionChanged();
        }

        protected void RaiseForReverse() => OnCollectionChanged();

        protected void RaiseForShuffle() => OnCollectionChanged();

        protected void RaiseForSort() => OnCollectionChanged();

        protected void RaiseForUpdate(T item, T oldItem)
        {
            Requires(AllowsNull || item != null, ItemMustBeNonNull);
            Requires(AllowsNull || oldItem != null, ItemMustBeNonNull);

            OnItemsRemoved(oldItem, 1);
            OnItemsAdded(item, 1);
            OnCollectionChanged();
        }

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

        #region Protected Properties

        protected virtual int Version
        {
            [Pure]
            get;
            private set;
        }

        #endregion

        #region Protected Methods

        [Pure]
        protected bool CheckVersion(int version)
        {
            if (version == Version) {
                return true;
            }

            // See https://msdn.microsoft.com/library/system.collections.ienumerator.movenext.aspx
            throw new InvalidOperationException(CollectionWasModified);
        }

        protected virtual void UpdateVersion() => Version++;

        #endregion
    }
}