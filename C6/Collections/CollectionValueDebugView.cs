// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Diagnostics;

using static System.Diagnostics.Contracts.Contract;


namespace C6.Collections
{
    internal sealed class CollectionValueDebugView<T>
    {
        #region Fields

        private readonly ICollectionValue<T> _collection;

        #endregion

        #region Constructors

        public CollectionValueDebugView(ICollectionValue<T> collection)
        {
            Requires(collection != null);

            _collection = collection;
        }

        #endregion

        #region Properties

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _collection.ToArray();

        #endregion
    }
}