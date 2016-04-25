// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;
using System.Text;

using C6.Contracts;
using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.ExceptionMessages;
using static C6.Tests.Helpers.CollectionEvent;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class IListTests : IIndexedTests
    {
        #region Factories
        protected abstract IList<T> GetEmptyList<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        protected abstract IList<T> GetList<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);
        
        private IIndexed<string> GetStringList(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetIndexed(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        #region Inherited

        protected override IIndexed<T> GetEmptyIndexed<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetEmptyList(equalityComparer, allowsNull);

        protected override IIndexed<T> GetIndexed<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetList(enumerable, equalityComparer, allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region Properties

        #endregion

        #region Methods

        #endregion

        #endregion
    }
}