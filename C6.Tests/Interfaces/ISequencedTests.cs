// This file is part of the C6 Generic Sequenced Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.ExceptionMessages;
using static C6.Tests.Helpers.CollectionEvent;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;
using KVP = C6.KeyValuePair<int, int>;


namespace C6.Tests
{
    [TestFixture]
    public abstract class ISequencedTests : ICollectionTests
    {
        #region Factories
        
        protected abstract ISequenced<T> GetEmptySequence<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        protected abstract ISequenced<T> GetSequence<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        #region Helpers

        private ISequenced<int> GetIntSequence(Random random, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetIntegers(random, GetCount(random)), equalityComparer, allowsNull);

        private ISequenced<int> GetIntSequence(Random random, int count, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetIntegers(random, count), equalityComparer, allowsNull);

        private ISequenced<string> GetStringSequence(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        private ISequenced<string> GetStringSequence(Randomizer random, int count, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetStrings(random, count), equalityComparer, allowsNull);

        #endregion

        #region Inherited

        protected override ICollection<T> GetEmptyCollection<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetEmptySequence(equalityComparer, allowsNull);

        protected override ICollection<T> GetCollection<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetSequence(enumerable, equalityComparer, allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region Methods

        #region GetSequencedHashCode()



        #endregion

        #region SequencedEquals(ISequenced<T>)

        #endregion

        #endregion

        #endregion
    }
}