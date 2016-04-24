// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;
using System.Text;

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
        public void GetIndexRange_GetFullRange_Expected()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var count = collection.Count;
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection,
                collection.ToArray(),
                ReferenceEqualityComparer
            );

            // Act
            var getIndexRange = collection.GetIndexRange(0, count);

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(expected));
        }

        [Test]
        public void GetIndexRange_RandomRange_Expected()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var count = Random.Next(0, collection.Count);
            var startIndex = Random.Next(0, collection.Count - count);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection,
                collection.Skip(startIndex).Take(count),
                ReferenceEqualityComparer
            );

            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, count);

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(expected));
        }

        [Test]
        public void GetIndexRange_EmptyCollection_Expected()
        {
            // Arrange
            var collection = GetEmptyIndexed<string>();
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection,
                NoStrings,
                ReferenceEqualityComparer
            );

            // Act
            var getIndexRange = collection.GetIndexRange(0, 0);

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(expected));
        }

        [Test]
        public void GetIndexRange_EmptyRange_Expected()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var startIndex = Random.Next(0, collection.Count);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection,
                NoStrings,
                ReferenceEqualityComparer
            );

            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, 0);

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(expected));
        }

        [Test]
        public void GetIndexRange_ChangeCollectionInvalidatesDirectedCollectionValue_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetIndexed(items);
            var count = Random.Next(0, collection.Count);
            var startIndex = Random.Next(0, collection.Count - count);
            var array = new string[collection.Count];
            var stringBuilder = new StringBuilder();
            var rest = 0;

            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, count);
            collection.Add(GetLowercaseString(Random));

            // TODO: Refactor into separate DirectCollectionValueConstraint
            // Assert
            Assert.That(() => getIndexRange.AllowsNull, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.Count, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.CountSpeed, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.Direction, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.IsEmpty, Throws.InvalidOperationException.Because(CollectionWasModified));

            Assert.That(() => getIndexRange.Backwards(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.Choose(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.CopyTo(array, 0), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.GetEnumerator().MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.ToArray(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.Show(stringBuilder, ref rest, null), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.ToString(null, null), Throws.InvalidOperationException.Because(CollectionWasModified));

            Assert.That(() => getIndexRange.Equals(null), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.GetHashCode(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => getIndexRange.ToString(), Throws.InvalidOperationException.Because(CollectionWasModified));
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
            var items = GetStrings(Random);
            var collection = GetIndexed(items);
            var item = items.DifferentItem(() => Random.GetString());
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

        #region LastIndexOf(T)

        [Test]
        public void LastIndexOf_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.LastIndexOf(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void LastIndexOf_AllowsNull_PositiveIndex()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetIndexed(items, allowsNull: true);
            var index = collection.ToArray().LastIndexOf(null);

            // Act
            var lastIndexOf = collection.LastIndexOf(null);

            // Assert
            Assert.That(lastIndexOf, Is.EqualTo(index));
        }

        [Test]
        public void LastIndexOf_EmptyCollection_TildeZero()
        {
            // Arrange
            var collection = GetEmptyIndexed<string>();
            var item = Random.GetString();

            // Act
            var lastIndexOf = collection.LastIndexOf(item);

            // Assert
            Assert.That(lastIndexOf, Is.EqualTo(~0));
        }

        [Test]
        public void LastIndexOf_RandomCollectionLastIndexOfNewItem_NegativeIndex()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetIndexed(items);
            var item = items.DifferentItem(() => Random.GetString());
            var count = collection.Count;

            // Act
            var lastIndexOf = collection.LastIndexOf(item);

            // Assert
            Assert.That(~lastIndexOf, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(count));
        }

        [Test]
        public void LastIndexOf_RandomCollectionLastIndexOfExistingItem_Index()
        {
            // Arrange
            var collection = GetStringIndexed(Random, ReferenceEqualityComparer);
            var items = collection.ToArray();
            var index = Random.Next(0, items.Length);
            var item = items[index];

            // Act
            var lastIndexOf = collection.LastIndexOf(item);

            // Assert
            Assert.That(lastIndexOf, Is.EqualTo(index));
        }

        [Test]
        public void LastIndexOf_DuplicateItems_CountMinusOne()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = item.Repeat(count);
            var collection = GetIndexed(items);

            // Act
            var lastIndexOf = collection.LastIndexOf(item);

            // Assert
            Assert.That(lastIndexOf, Is.EqualTo(count - 1));
        }

        [Test]
        public void LastIndexOf_CollectionWithDuplicateItems_LastIndex()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = GetStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetIndexed(items);
            var index = collection.ToArray().LastIndexOf(item);

            // Act
            var lastIndexOf = collection.LastIndexOf(item);

            // Assert
            Assert.That(lastIndexOf, Is.EqualTo(index));
        }

        [Test]
        public void LastIndexOf_RandomCollectionNewItem_GetsTildeIndex()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetIndexed(items);
            var item = GetLowercaseString(Random);

            // Act
            var expectedIndex = ~collection.LastIndexOf(item);
            collection.Add(item);
            var lastIndexOf = collection.LastIndexOf(item);

            // Assert
            Assert.That(lastIndexOf, Is.EqualTo(expectedIndex));
        }

        #endregion

        #region RemoveAt(int)

        [Test]
        public void RemoveAt_EmptyCollection_ViolatesPrecondtion()
        {
            // Arrange
            var collection = GetEmptyIndexed<string>();

            // Act & Assert
            Assert.That(() => collection.RemoveAt(0), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void RemoveAt_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var index = Random.Next(int.MinValue, 0);

            // Act & Assert
            Assert.That(() => collection.RemoveAt(index), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void RemoveAt_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var index = collection.Count;

            // Act & Assert
            Assert.That(() => collection.RemoveAt(index), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void RemoveAt_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var index = Random.Next(collection.Count + 1, int.MaxValue);

            // Act & Assert
            Assert.That(() => collection.RemoveAt(index), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void RemoveAt_RemoveDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var index = Random.Next(0, collection.Count);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RemoveAt(index);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void RemoveAt_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var index = Random.Next(0, collection.Count);
            var item = collection[index];
            var expectedEvents = new[] {
                RemovedAt(item, index, collection),
                Removed(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.RemoveAt(index), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void RemoveAt_RandomCollection_ItemAtIndex()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var index = Random.Next(0, collection.Count);
            var expectedItem = collection[index];

            // Act
            var item = collection.RemoveAt(index);

            // Assert
            Assert.That(item, Is.SameAs(expectedItem));
        }

        [Test]
        public void RemoveAt_RandomCollectionWithNullRemoveNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetIndexed(items, allowsNull: true);
            var index = collection.IndexOf(null);

            // Act
            var removeAt = collection.RemoveAt(index);

            // Assert
            Assert.That(removeAt, Is.Null);
        }

        [Test]
        public void RemoveAt_SingleItemCollection_Item()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetIndexed(itemArray);
            
            // Act
            var removeAt = collection.RemoveAt(0);

            // Assert
            Assert.That(removeAt, Is.SameAs(item));
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void RemoveAt_RemoveFirstItem_Removed()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var items = collection.ToArray();
            var index = 0;
            var firstItem = collection[index];

            // Act
            var removeAt = collection.RemoveAt(index);

            // Assert
            Assert.That(removeAt, Is.EqualTo(firstItem));
            Assert.That(collection, Is.EqualTo(items.Skip(1)));
        }

        [Test]
        public void RemoveAt_RemoveLastItem_Removed()
        {
            // Arrange
            var collection = GetStringIndexed(Random);
            var count = collection.Count;
            var items = collection.ToArray();
            var index = count - 1;
            var lastItem = collection[index];

            // Act
            var removeAt = collection.RemoveAt(index);

            // Assert
            Assert.That(removeAt, Is.EqualTo(lastItem));
            Assert.That(collection, Is.EqualTo(items.Take(index)));
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveAt_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveAt_DuplicatesByCounting_Fail()
        {
            Run.If(DuplicatesByCounting);

            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion
    }
}