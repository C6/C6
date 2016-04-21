using NUnit.Framework;
using System;
using System.Linq;

using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.EnumerationDirection;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;
namespace C6.Tests.Interfaces
{
    [TestFixture]
    public abstract class IIndexedTests : ISequencedTests
    {
        #region Factories
    
        protected abstract Speed IndexingSpeed { get; }

        protected abstract IIndexed<T> GetEmptyIndexed<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        protected abstract IIndexed<T> GetIndexed<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        private IIndexed<string> GetStringIndexed(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetIndexed(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        #region Inherited

        protected override ISequenced<T> GetEmptySequence<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetEmptyIndexed(equalityComparer, allowsNull);

        protected override ISequenced<T> GetSequence<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetIndexed(enumerable, equalityComparer, allowsNull);

        #endregion

        #endregion


    }
}
