// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
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

        #region Test Methods

        #region Properties

        [Test]
        public void IndexingSpeed_RandomCollection_IndexingSpeed()
        {
            // Arrange
            var collection = GetStringIndexed(Random);

            // Act
            var indexingSpeed = collection.IndexingSpeed;

            // Assert
            Assert.That(indexingSpeed, Is.EqualTo(IndexingSpeed));
        }

        #endregion

        #region Methods

        #endregion

        #endregion
    }
}