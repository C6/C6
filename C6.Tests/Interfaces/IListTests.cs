// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

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


namespace C6.Tests
{
    [TestFixture]
    public abstract class IListTests : IIndexedTests
    {
        #region Factories

        protected abstract IList<T> GetEmptyList<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        protected abstract IList<T> GetList<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        private IList<string> GetStringList(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetList(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        private static NonComparable[] GetNonComparables(Random random) => GetNonComparables(random, GetCount(random));
        private static NonComparable[] GetNonComparables(Random random, int count) => Enumerable.Range(0, count).Select(i => new NonComparable(random.Next())).ToArray();
        private static Comparable[] GetComparables(Random random) => GetComparables(random, GetCount(random));
        private static Comparable[] GetComparables(Random random, int count) => Enumerable.Range(0, count).Select(i => new Comparable(random.Next())).ToArray();

        #region Inherited

        protected override IIndexed<T> GetEmptyIndexed<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetEmptyList(equalityComparer, allowsNull);

        protected override IIndexed<T> GetIndexed<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetList(enumerable, equalityComparer, allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region SC.ICollection

        #region Properties

        #region IsSynchronized

        [Test]
        public void IsSynchronized_RandomCollection_False()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            var isSynchronized = collection.IsSynchronized;

            // Assert
            Assert.That(isSynchronized, Is.False);
        }

        #endregion

        #endregion

        #region Methods

        #endregion

        #endregion

        #region IList<T>

        #region Properties

        #region First

        [Test]
        public void First_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => collection.First, Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void First_SingleItemCollection_Item()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);

            // Act
            var first = collection.First;

            // Assert
            Assert.That(first, Is.SameAs(item));
        }

        [Test]
        public void First_RandomCollection_FirstItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = collection.First();

            // Act
            var first = collection.First;

            // Assert
            Assert.That(first, Is.EqualTo(item));
        }

        [Test]
        public void First_RandomCollectionStartingWithNull_Null()
        {
            // Arrange
            var items = new string[] { null }.Concat(GetStrings(Random));
            var collection = GetList(items, allowsNull: true);

            // Act
            var first = collection.First;

            // Assert
            Assert.That(first, Is.Null);
        }

        #endregion

        #region Last

        [Test]
        public void Last_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => collection.Last, Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void Last_SingleItemCollection_Item()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);

            // Act
            var last = collection.Last;

            // Assert
            Assert.That(last, Is.SameAs(item));
        }

        [Test]
        public void Last_RandomCollection_LastItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = collection.Last();

            // Act
            var last = collection.Last;

            // Assert
            Assert.That(last, Is.EqualTo(item));
        }

        [Test]
        public void Last_RandomCollectionStartingWithNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).Append(null);
            var collection = GetList(items, allowsNull: true);

            // Act
            var last = collection.Last;

            // Assert
            Assert.That(last, Is.Null);
        }

        #endregion

        #region this[int]

        [Test]
        public void ItemSet_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(int.MinValue, 0);
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection[index] = item, Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemSet_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = collection.Count;
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection[index] = item, Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemSet_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(collection.Count + 1, int.MaxValue);
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection[index] = item, Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemSet_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var index = 0;
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection[index] = item, Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemSet_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);
            var index = GetIndex(collection, Random);

            // Act & Assert
            Assert.That(() => collection[index] = null, Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void ItemSet_RandomCollectionSetDuplicate_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random);
            var item = collection.ToArray().Choose(Random);

            // Act & Assert
            Assert.That(() => collection[index] = item, Violates.PreconditionSaying(CollectionMustAllowDuplicates));
        }

        [Test]
        public void ItemSet_RandomCollectionSetDuplicate_Inserted()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random);
            var item = collection.ToArray().Choose(Random);

            // Act
            collection[index] = item;

            // Assert
            Assert.That(collection[index], Is.SameAs(item));
        }

        [Test]
        public void ItemSet_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var index = GetIndex(collection, Random);
            var array = collection.ToArray();
            array[index] = null;

            // Act
            collection[index] = null;

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void ItemSet_RandomCollectionIndexZero_FirstItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = 0;
            var array = collection.ToArray();
            array[index] = item;

            // Act
            collection[index] = item;

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void ItemSet_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = collection.Count - 1;
            var array = collection.ToArray();
            array[index] = item;

            // Act
            collection[index] = item;

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void ItemSet_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random);
            var array = collection.ToArray();
            array[index] = item;

            // Act
            collection[index] = item;

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void ItemSet_RandomCollectionRandomIndex_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random);
            var oldItem = collection[index];
            var expectedEvents = new[] {
                RemovedAt(oldItem, index, collection),
                Removed(oldItem, 1, collection),
                Inserted(item, index, collection),
                Added(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection[index] = item, Raises(expectedEvents).For(collection));
        }

        [Test]
        public void ItemSet_SetDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection[index] = item;

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void ItemSet_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #endregion

        #region Methods

        #region Insert(int, T)

        [Test]
        public void Insert_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(int.MinValue, 0);
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void Insert_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(collection.Count + 1, int.MaxValue);
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void Insert_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);
            var index = GetIndex(collection, Random);

            // Act & Assert
            Assert.That(() => collection.Insert(index, null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Insert_RandomCollectionSetDuplicate_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random);
            var item = collection.ToArray().Choose(Random);

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Violates.PreconditionSaying(CollectionMustAllowDuplicates));
        }

        [Test]
        public void Insert_RandomCollectionSetDuplicate_Inserted()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random);
            var item = collection.ToArray().Choose(Random);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection[index], Is.SameAs(item));
        }

        [Test]
        public void Insert_EmptyCollection_SingleItemCollection()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var index = 0;
            var item = Random.GetString();
            var array = new[] { item };

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Insert_IndexOfCount_Appended()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = collection.Count;
            var item = Random.GetString();
            var array = collection.Append(item).ToArray();

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Insert_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var index = GetIndex(collection, Random);
            var array = collection.ToArray().Insert(index, null);

            // Act
            collection.Insert(index, null);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Insert_RandomCollectionIndexZero_FirstItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = 0;
            var array = collection.ToArray().Insert(index, item);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Insert_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = collection.Count - 1;
            var array = collection.ToArray().Insert(index, item);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Insert_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random);
            var array = collection.ToArray().Insert(index, item);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Insert_RandomCollectionRandomIndex_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random);
            var expectedEvents = new[] {
                Inserted(item, index, collection),
                Added(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Insert_InsertDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Insert(index, item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void Insert_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Insert_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region IsSorted()

        [Test]
        public void IsSorted_EmptyCollection_True()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSorted_SingleItemCollection_True()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSorted_TwoItemsAscending_True()
        {
            // Arrange
            var items = new[] { Random.Next(int.MinValue, 0), Random.Next() };
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSorted_TwoItemsDescending_False()
        {
            // Arrange
            var items = new[] { Random.Next(), Random.Next(int.MinValue, 0) };
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        public void IsSorted_TwoEqualItems_True()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item, string.Copy(item) };
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSorted_EqualItems_True()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = TestHelper.Repeat(() => string.Copy(item), count);
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.True);
        }


        [Test]
        public void IsSorted_NonComparables_ThrowsArgumentException()
        {
            // Arrange
            var items = GetNonComparables(Random);
            var collection = GetList(items);

            // Act & Assert
            // TODO: This is not the exception stated in the documentation!
            Assert.That(() => collection.IsSorted(), Throws.ArgumentException.Because("At least one object must implement IComparable."));
        }

        [Test]
        public void IsSorted_Comparables_ThrowsNothing()
        {
            // Arrange
            var items = GetComparables(Random);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.IsSorted(), Throws.Nothing);
        }

        [Test]
        public void IsSorted_NonDescendingRandomCollection_True()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSorted_Descending_False()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem + 1, previousItem + maxGap)), count).Reverse();
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        public void IsSorted_AllButLastAreSorted_False()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count).Append(new Comparable(previousItem - 1));
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted();

            // Assert
            Assert.That(isSorted, Is.False);
        }

        #endregion

        #region Reverse()

        [Test]
        public void Reverse_EmptyCollection_Nothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act
            collection.Reverse();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Reverse_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => collection.Reverse(), RaisesNoEventsFor(collection));
        }

        [Test]
        public void Reverse_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);

            var expectedEvents = new[] {
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.Reverse(), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Reverse_ReverseDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Reverse();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void Reverse_ReverseDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Reverse();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void Reverse_RandomCollectionWithNull_Reversed()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetList(items, allowsNull: true);
            var reversedCollection = collection.ToArray().Reverse();

            // Act
            collection.Reverse();

            // Assert
            Assert.That(collection, Is.EqualTo(reversedCollection));
        }

        [Test]
        public void Reverse_SingleItemCollection_RaisesNoEvents()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Reverse(), RaisesNoEventsFor(collection));
        }

        [Test]
        public void Reverse_SingleItemCollectionReverseDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Reverse();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void Reverse_RandomCollection_Reversed()
        {
            // Arrange
            var collection = GetStringList(Random);
            var reversedCollection = collection.ToArray().Reverse();

            // Act
            collection.Reverse();

            // Assert
            Assert.That(collection, Is.EqualTo(reversedCollection));
        }

        [Test]
        public void Reverse_ReverseReversedRandomCollection_OriginalCollection()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = collection.ToArray();

            // Act
            collection.Reverse();
            collection.Reverse();

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        [Category("Unfinished")]
        public void Reverse_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Nested Types

        private class NonComparable
        {
            public NonComparable(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }


        private class Comparable : NonComparable, IComparable<NonComparable>
        {
            public Comparable(int value) : base(value) {}
            public int CompareTo(NonComparable other) => Value.CompareTo(other.Value);
        }

        #endregion
    }
}