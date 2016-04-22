// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Tests.Contracts;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
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

        #region IndexingSpeed

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

        #region this[int]

        [Test]
        public void Item_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var index = Random.Next(int.MinValue, 0);

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void Item_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var index = collection.Count;

            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void Item_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var count = collection.Count;
            var index = Random.Next(count + 1, int.MaxValue);
            
            // Act & Assert
            Assert.That(() => collection[index], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void Item_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyIndexed<string>();
            
            // Act & Assert
            Assert.That(() => collection[0], Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void Item_RandomCollectionWithNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetIndexed(items, allowsNull: true);
            var index = collection.ToArray().IndexOf(null);

            // Act
            var item = collection[index];

            // Act & Assert
            Assert.That(item, Is.Null);
        }

        [Test]
        public void Item_RandomCollectionIndexZero_FirstItem()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var first = collection.First();

            // Act
            var item = collection[0];

            // Assert
            Assert.That(item, Is.SameAs(first));
        }

        [Test]
        public void Item_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var last = collection.Last();
            var count = collection.Count;

            // Act
            var item = collection[count - 1];

            // Assert
            Assert.That(item, Is.SameAs(last));
        }

        [Test]
        public void Item_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var array = collection.ToArray();
            var index = Random.Next(0, array.Length);

            // Act
            var item = collection[index];

            // Assert
            Assert.That(item, Is.SameAs(array[index]));
        }

        #endregion


        #endregion

        #region Methods

        #region GetIndexRange(int, int)

        [Test]
        public void GetIndexRange_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var startIndex = Random.Next(int.MinValue, 0);
            var count = collection.Count / 2;

            // Act & Assert
            Assert.That(() => collection.GetIndexRange(startIndex, count), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void GetIndexRange_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var startIndex = collection.Count;
            var count = collection.Count / 2;

            // Act & Assert
            Assert.That(() => collection.GetIndexRange(startIndex, count), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void GetIndexRange_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var startIndex = Random.Next(collection.Count + 1, int.MaxValue);
            var count = collection.Count / 2;

            // Act & Assert
            Assert.That(() => collection.GetIndexRange(startIndex, count), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void GetIndexRange_NegativeCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var count = collection.Count / 2;
            var startIndex = Random.Next(0, count);

            // Act & Assert
            Assert.That(() => collection.GetIndexRange(startIndex, -count), Violates.PreconditionSaying(ArgumentMustBeNonNegative));
        }

        [Test]
        public void GetIndexRange_CountIsOneLongerThanCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var startIndex = Random.Next(0, collection.Count);
            var count = collection.Count - startIndex + 1;

            // Act & Assert
            Assert.That(() => collection.GetIndexRange(startIndex, count), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void GetIndexRange_GetFullRange_EqualsCollection()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var count = collection.Count;

            // Act
            var getIndexRange = collection.GetIndexRange(0, count);

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(collection));
        }

        [Test]
        public void GetIndexRange_RandomRange_ContainsCountItems()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var count = Random.Next(0, collection.Count);
            var startIndex = Random.Next(0, collection.Count - count);

            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, count);

            // Assert
            Assert.That(getIndexRange.Count, Is.EqualTo(count));
        }

        [Test]
        public void GetIndexRange_RandomRange_EqualsSubrange()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var array = collection.ToArray();
            var count = Random.Next(0, collection.Count);
            var startIndex = Random.Next(0, collection.Count - count);
            var subrange = array.Skip(startIndex).Take(count);

            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, count);

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(subrange));
        }

        // TODO: Test IDirectedCollectionValue extensively

        [Test]
        public void GetIndexRange_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyIndexed<string>();

            // Act
            var getIndexRange = collection.GetIndexRange(0, 0);

            // Assert
            Assert.That(getIndexRange, Is.Empty);
        }

        [Test]
        public void GetIndexRange_EmptyRange_Empty()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var startIndex = Random.Next(0, collection.Count);
            
            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, 0);

            // Assert
            Assert.That(getIndexRange, Is.Empty);
        }

        #endregion

        #region IndexOf(T)

        [Test]
        public void IndexOf_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.IndexOf(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void IndexOf_AllowsNull_PositiveIndex()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetIndexed(items, allowsNull: true);
            var index = collection.ToArray().IndexOf(null);

            // Act
            var indexOf = collection.IndexOf(null);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void IndexOf_EmptyCollection_TildeZero()
        {
            // Arrange
            var collection = GetEmptyIndexed<string>();
            var item = Random.GetString();

            // Act
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(~0));
        }

        [Test]
        public void IndexOf_RandomCollectionIndexOfNewItem_NegativeIndex()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetIndexed(items);
            var item = GetLowercaseString(Random);
            var count = collection.Count;

            // Act
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(~indexOf, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(count));
        }

        [Test]
        public void IndexOf_RandomCollectionIndexOfExistingItem_Index()
        {
            // Arrange
            var collection = GetStringIndexed(Random, ReferenceEqualityComparer);
            var items = collection.ToArray();
            var index = Random.Next(0, items.Length);
            var item = items[index];

            // Act
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void IndexOf_DuplicateItems_Zero()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = item.Repeat(count);
            var collection = GetIndexed(items);

            // Act
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.Zero);
        }

        [Test]
        public void IndexOf_CollectionWithDuplicateItems_FirstIndex()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = GetStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetIndexed(items);
            var index = collection.ToArray().IndexOf(item);

            // Act
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void IndexOf_RandomCollectionNewItem_GetsTildeIndex()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetIndexed(items);
            var item = GetLowercaseString(Random);

            // Act
            var expectedIndex = ~collection.IndexOf(item);
            collection.Add(item);
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(expectedIndex));
        }

        #endregion

        #endregion

        #endregion
    }
}