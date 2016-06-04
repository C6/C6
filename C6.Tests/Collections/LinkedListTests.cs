// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Collections;

using NUnit.Framework;

using static System.Diagnostics.Contracts.Contract;

using SCG = System.Collections.Generic;


namespace C6.Tests.Collections
{
    [TestFixture]
    public class LinkedListTests : ICollectionValueTests
    {
        protected override ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false) => new LinkedList<T>(allowsNull);

        protected override ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new LinkedList<T>(enumerable, allowsNull);

        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            Requires(collection is LinkedList<T>);

            var linkedList = (LinkedList<T>) collection;

            // TODO: Use Last
            yield return linkedList.Last();
        }
    }
}