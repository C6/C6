// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Collections;

using NUnit.Framework;

using SCG = System.Collections.Generic;


namespace C6.Tests.Collections
{
    [TestFixture]
    public class CircularQueueTests : ICollectionValueTests {
        protected override ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false) => new CircularQueue<T>(allowsNull: allowsNull);

        protected override ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new CircularQueue<T>(enumerable, allowsNull: allowsNull);

        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            yield return collection.First();
        }
    }
}