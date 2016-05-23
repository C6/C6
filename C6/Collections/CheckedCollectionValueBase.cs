// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics;

using static C6.Collections.ExceptionMessages;


namespace C6.Collections
{
    // TODO: Add contracts
    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay}")]
    [DebuggerTypeProxy(typeof(CollectionValueDebugView<>))]
    public abstract class CheckedCollectionValueBase<T> : CollectionValueBase<T>
    {
        public override bool AllowsNull => CheckVersion() & AllowsNullProtected;

        protected abstract bool AllowsNullProtected { get; }

        public override int Count
        {
            get {
                CheckVersion();
                return CountProtected;
            }
        }

        protected abstract int CountProtected { get; }

        public override Speed CountSpeed
        {
            get {
                CheckVersion();
                return CountSpeedProtected;
            }
        }

        protected abstract Speed CountSpeedProtected { get; }

        protected string DebuggerDisplay => IsValid ? ToString() : "Expired collection value; original collection was modified since range was created.";

        public override bool IsEmpty => CheckVersion() & IsEmptyProtected;

        protected abstract bool IsEmptyProtected { get; }

        protected abstract bool IsValid { get; }

        protected virtual bool CheckVersion()
        {
            if (IsValid) {
                return true;
            }

            // See https://msdn.microsoft.com/library/system.collections.ienumerator.movenext.aspx
            throw new InvalidOperationException(CollectionWasModified);
        }

        public override T Choose()
        {
            CheckVersion();
            return ChooseProtected();
        }

        protected abstract T ChooseProtected();

        public override void CopyTo(T[] array, int arrayIndex)
        {
            CheckVersion();
            CopyToProtected(array, arrayIndex);
        }

        protected abstract void CopyToProtected(T[] array, int arrayIndex);

        public override bool Equals(object obj) => CheckVersion() & base.Equals(obj);

        public override int GetHashCode()
        {
            CheckVersion();
            return base.GetHashCode();
        }

        public override T[] ToArray()
        {
            CheckVersion();
            return ToArrayProtected();
        }

        protected abstract T[] ToArrayProtected();
    }
}