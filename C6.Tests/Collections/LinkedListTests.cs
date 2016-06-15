// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Collections;

using NUnit.Framework;

using static System.Diagnostics.Contracts.Contract;

using static C6.EventTypes;
using static C6.Speed;

using SCG = System.Collections.Generic;


namespace C6.Tests.Collections
{
    [TestFixture]
    public class LinkedListListTests : IListTests
    {
        #region Properties

        protected override bool AllowsDuplicates => true;

        protected override Speed ContainsSpeed => Linear;

        protected override bool DuplicatesByCounting => false;

        protected override Speed IndexingSpeed => Linear;

        protected override bool IsFixedSize => false;

        protected override bool IsReadOnly => false;

        protected override EventTypes ListenableEvents => All;

        #endregion

        #region Methods

        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            Requires(collection is LinkedList<T>);

            var linkedList = (LinkedList<T>) collection;

            // TODO: Use Last
            yield return linkedList.Last();
        }

        protected override IList<T> GetEmptyList<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => new LinkedList<T>(equalityComparer, allowsNull);

        protected override IList<T> GetList<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => new LinkedList<T>(enumerable, equalityComparer, allowsNull);

        #endregion
    }


    [TestFixture]
    public class LinkedListStackTests : IStackTests
    {
        protected override bool IsReadOnly => false;
        protected override EventTypes ListenableEvents => All;

        protected override IStack<T> GetEmptyStack<T>(bool allowsNull = false) => new LinkedList<T>(allowsNull: allowsNull);
        protected override IStack<T> GetStack<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new LinkedList<T>(enumerable, allowsNull: allowsNull);
        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            Requires(collection is LinkedList<T>);

            var linkedList = (LinkedList<T>) collection;

            yield return linkedList.Last;
        }
    }
}