// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using C6.Collections;

using NUnit.Framework;

using SCG = System.Collections.Generic;


namespace C6.Tests.Collections
{
    [TestFixture]
    public class LinkedListTests : IEnumerableTests
    {
        protected override SCG.IEnumerable<T> GetEmptyEnumerable<T>() => new LinkedList<T>();

        protected override SCG.IEnumerable<T> GetEnumerable<T>(SCG.IEnumerable<T> enumerable) => new LinkedList<T>(enumerable);
    }
}