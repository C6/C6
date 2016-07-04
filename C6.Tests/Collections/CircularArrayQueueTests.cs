// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Collections;

using NUnit.Framework;

using static C6.EventTypes;

using SCG = System.Collections.Generic;


namespace C6.Tests.Collections
{
    [TestFixture]
    public class CircularArrayQueueListenableTests : IListenableTests
    {
        protected override IListenable<T> GetEmptyListenable<T>(bool allowsNull = false) => new CircularArrayQueue<T>(allowsNull: allowsNull);

        protected override IListenable<T> GetListenable<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new CircularArrayQueue<T>(enumerable, allowsNull: allowsNull);

        protected override EventTypes ListenableEvents => All;

        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            yield return collection.First();
        }
    }


    [TestFixture]
    public class CircularArrayQueueQueueTests : IQueueTests
    {
        protected override IQueue<T> GetEmptyQueue<T>(bool allowsNull = false) => new CircularArrayQueue<T>(allowsNull: allowsNull);

        protected override IQueue<T> GetQueue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new CircularArrayQueue<T>(enumerable, allowsNull);

        protected override EventTypes ListenableEvents => All;

        protected override bool IsReadOnly => false;

        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            yield return collection.First();
        }
    }
}