﻿// This file is part of the C6 Generic Collection Library for C# and CLI
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

        #region SC.IList

        #region IndexOf(T)

        [Test]
        public void SCIListIndexOf_InvalidType_ThrowsInvalidCastException()
        {
            // Arrange
            var collection = GetStringList(Random);
            object item = Random.Next();

            // Act & Assert
            Assert.That(() => collection.IndexOf(item), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SCIListIndexOf_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.IndexOf((object) null), Violates.UncaughtPrecondition);
        }

        [Test]
        public void SCIListIndexOf_AllowsNull_PositiveIndex()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetList(items, allowsNull: true);
            var index = collection.ToArray().IndexOf(null);

            // Act
            var indexOf = collection.IndexOf((object) null);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void SCIListIndexOf_EmptyCollection_TildeZero()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            object item = Random.GetString();

            // Act
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(~0));
        }

        [Test]
        public void SCIListIndexOf_RandomCollectionIndexOfNewItem_NegativeIndex()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetList(items);
            object item = items.DifferentItem(() => Random.GetString());
            var count = collection.Count;

            // Act
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(~indexOf, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(count));
        }

        [Test]
        public void SCIListIndexOf_RandomCollectionIndexOfExistingItem_Index()
        {
            // Arrange
            var collection = GetStringList(Random, ReferenceEqualityComparer);
            var items = collection.ToArray();
            var index = Random.Next(0, items.Length);
            object item = items[index];

            // Act
            var indexOf = collection.IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void SCIListIndexOf_DuplicateItems_Zero()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = item.Repeat(count);
            var collection = GetList(items);

            // Act
            var indexOf = collection.IndexOf((object) item);

            // Assert
            Assert.That(indexOf, Is.Zero);
        }

        [Test]
        public void SCIListIndexOf_CollectionWithDuplicateItems_FirstIndex()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = GetStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetList(items);
            var index = collection.ToArray().IndexOf(item);

            // Act
            var indexOf = collection.IndexOf((object) item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void SCIListIndexOf_RandomCollectionNewItem_GetsTildeIndex()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetList(items);
            var item = GetLowercaseString(Random);

            // Act
            var expectedIndex = ~collection.IndexOf(item);
            collection.Add(item);
            var indexOf = collection.IndexOf((object) item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(expectedIndex));
        }

        #endregion

        #region Insert(int, object)

        [Test]
        public void SCIListInsert_InvalidType_ThrowsInvalidCastException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            object item = Random.Next();

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SCIListInsert_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(int.MinValue, 0);
            object item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Violates.Precondition);
        }

        [Test]
        public void SCIListInsert_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(collection.Count + 1, int.MaxValue);
            object item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Violates.UncaughtPrecondition);
        }

        [Test]
        public void SCIListInsert_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);
            var index = GetIndex(collection, Random, true);

            // Act & Assert
            Assert.That(() => collection.Insert(index, (object) null), Violates.UncaughtPrecondition);
        }

        [Test]
        public void SCIListInsert_RandomCollectionSetDuplicate_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            object item = collection.ToArray().Choose(Random);

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Violates.Precondition);
        }

        [Test]
        public void SCIListInsert_RandomCollectionSetDuplicate_Inserted()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            object item = collection.ToArray().Choose(Random);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection[index], Is.SameAs(item));
        }

        [Test]
        public void SCIListInsert_EmptyCollection_SingleItemCollection()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var index = 0;
            object item = Random.GetString();
            var array = new[] { item };

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListInsert_IndexOfCount_Appended()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = collection.Count;
            object item = Random.GetString();
            var array = collection.Append(item).ToArray();

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListInsert_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var index = GetIndex(collection, Random, true);
            var array = collection.ToArray().InsertItem(index, null);

            // Act
            collection.Insert(index, (object) null);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListInsert_RandomCollectionIndexZero_FirstItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            object item = Random.GetString();
            var index = 0;
            var array = collection.ToArray().InsertItem(index, item);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListInsert_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            object item = Random.GetString();
            var index = collection.Count - 1;
            var array = collection.ToArray().InsertItem(index, item);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListInsert_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            object item = Random.GetString();
            var index = GetIndex(collection, Random, true);
            var array = collection.ToArray().InsertItem(index, item);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListInsert_RandomCollectionRandomIndex_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random, true);
            var expectedEvents = new[] {
                Inserted(item, index, collection),
                Added(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.Insert(index, (object) item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void SCIListInsert_InsertDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            object item = Random.GetString();
            var index = GetIndex(collection, Random, true);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Insert(index, item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListInsert_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListInsert_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #endregion

        #region SCG.IList<T>

        #region IndexOf(T)

        [Test]
        public void SCGIListIndexOf_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => ((SCG.IList<string>) collection).IndexOf(null), Violates.UncaughtPrecondition);
        }

        [Test]
        public void SCGIListIndexOf_AllowsNull_PositiveIndex()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetList(items, allowsNull: true);
            var index = collection.ToArray().IndexOf(null);

            // Act
            var indexOf = ((SCG.IList<string>) collection).IndexOf(null);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void SCGIListIndexOf_EmptyCollection_TildeZero()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var item = Random.GetString();

            // Act
            var indexOf = ((SCG.IList<string>) collection).IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(-1));
        }

        [Test]
        public void SCGIListIndexOf_RandomCollectionIndexOfNewItem_NegativeIndex()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetList(items);
            var item = items.DifferentItem(() => Random.GetString());
            var count = collection.Count;

            // Act
            var indexOf = ((SCG.IList<string>) collection).IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(-1));
        }

        [Test]
        public void SCGIListIndexOf_RandomCollectionIndexOfExistingItem_Index()
        {
            // Arrange
            var collection = GetStringList(Random, ReferenceEqualityComparer);
            var items = collection.ToArray();
            var index = Random.Next(0, items.Length);
            var item = items[index];

            // Act
            var indexOf = ((SCG.IList<string>) collection).IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void SCGIListIndexOf_DuplicateItems_Zero()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = item.Repeat(count);
            var collection = GetList(items);

            // Act
            var indexOf = ((SCG.IList<string>) collection).IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.Zero);
        }

        [Test]
        public void SCGIListIndexOf_CollectionWithDuplicateItems_FirstIndex()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = GetStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetList(items);
            var index = collection.ToArray().IndexOf(item);

            // Act
            var indexOf = ((SCG.IList<string>) collection).IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(index));
        }

        [Test]
        public void SCGIListIndexOf_RandomCollectionNewItem_GetsTildeIndex()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetList(items);
            var item = GetLowercaseString(Random);

            // Act
            var expectedIndex = ~collection.IndexOf(item);
            collection.Add(item);
            var indexOf = ((SCG.IList<string>) collection).IndexOf(item);

            // Assert
            Assert.That(indexOf, Is.EqualTo(expectedIndex));
        }

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
            var index = GetIndex(collection, Random, true);

            // Act & Assert
            Assert.That(() => collection.Insert(index, null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Insert_RandomCollectionSetDuplicate_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
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
            var index = GetIndex(collection, Random, true);
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
            var index = GetIndex(collection, Random, true);
            var array = collection.ToArray().InsertItem(index, null);

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
            var array = collection.ToArray().InsertItem(index, item);

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
            var array = collection.ToArray().InsertItem(index, item);

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
            var index = GetIndex(collection, Random, true);
            var array = collection.ToArray().InsertItem(index, item);

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
            var index = GetIndex(collection, Random, true);
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
            var index = GetIndex(collection, Random, true);

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

        #region InsertFirst(int, T)

        [Test]
        public void InsertFirst_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.InsertFirst(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void InsertFirst_RandomCollectionInsertExistingFirst_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var item = collection.ToArray().Choose(Random);

            // Act & Assert
            Assert.That(() => collection.InsertFirst(item), Violates.PreconditionSaying(CollectionMustAllowDuplicates));
        }

        [Test]
        public void InsertFirst_RandomCollectionInsertExistingFirst_InsertedFirst()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var item = collection.ToArray().Choose(Random);
            var array = collection.ToArray().InsertItem(0, item);

            // Act
            collection.InsertFirst(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertFirst_EmptyCollection_SingleItemCollection()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var item = Random.GetString();
            var array = new[] { item };

            // Act
            collection.InsertFirst(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertFirst_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var array = collection.ToArray().InsertItem(0, null);

            // Act
            collection.InsertFirst(null);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertFirst_RandomCollectionInsertFirst_InsertedFirst()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var array = collection.ToArray().InsertItem(0, item);

            // Act
            collection.InsertFirst(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertFirst_RandomCollectionInsertFirst_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var expectedEvents = new[] {
                Inserted(item, 0, collection),
                Added(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.InsertFirst(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void InsertFirst_InsertFirstDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.InsertFirst(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void InsertFirst_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void InsertFirst_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region InsertLast(int, T)

        [Test]
        public void InsertLast_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.InsertLast(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void InsertLast_RandomCollectionInsertExistingLast_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var item = collection.ToArray().Choose(Random);

            // Act & Assert
            Assert.That(() => collection.InsertLast(item), Violates.PreconditionSaying(CollectionMustAllowDuplicates));
        }

        [Test]
        public void InsertLast_RandomCollectionInsertExistingLast_InsertedLast()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var item = collection.ToArray().Choose(Random);
            var array = collection.Append(item).ToArray();

            // Act
            collection.InsertLast(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertLast_EmptyCollection_SingleItemCollection()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var item = Random.GetString();
            var array = new[] { item };

            // Act
            collection.InsertLast(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertLast_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var array = collection.Append(null).ToArray();

            // Act
            collection.InsertLast(null);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertLast_RandomCollectionInsertLast_InsertedLast()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var array = collection.Append(item).ToArray();

            // Act
            collection.InsertLast(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertLast_RandomCollectionInsertLast_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var expectedEvents = new[] {
                Inserted(item, collection.Count, collection),
                Added(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.InsertLast(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void InsertLast_InsertLastDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.InsertLast(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void InsertLast_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void InsertLast_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region InsertRange(int, IEnumerable<T>)

        [Test]
        public void InsertRange_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(int.MinValue, 0);
            var items = GetStrings(Random);

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, items), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void InsertRange_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(collection.Count + 1, int.MaxValue);
            var items = GetStrings(Random);

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, items), Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void InsertRange_NullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);
            var index = GetIndex(collection, Random, true);

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void InsertRange_DisallowsNullsInEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);
            var index = GetIndex(collection, Random, true);
            var items = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, items), Violates.PreconditionSaying(ItemsMustBeNonNull));
        }

        [Test]
        public void InsertRange_RandomCollectionInsertExistingItems_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var count = GetCount(Random);
            var items = collection.ShuffledCopy(Random).Take(count);

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, items), Violates.PreconditionSaying(CollectionMustAllowDuplicates));
        }

        [Test]
        public void InsertRange_RandomCollectionInsertExistingItem_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var item = collection.ToArray().Choose(Random);
            var items = GetStrings(Random).WithRepeatedItem(() => item, 1, Random);

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, items), Violates.PreconditionSaying(CollectionMustAllowDuplicates));
        }

        [Test]
        public void InsertRange_RandomCollectionInsertExistingItems_InsertedRange()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var count = GetCount(Random);
            var items = collection.ShuffledCopy(Random).Take(count).ToArray();
            var array = collection.ToArray().InsertItems(index, items);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_RandomCollectionInsertExistingItem_InsertedRange()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var item = collection.ToArray().Choose(Random);
            var items = GetStrings(Random).WithRepeatedItem(() => item, 1, Random);
            var array = collection.ToArray().InsertItems(index, items);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_EmptyCollection_Items()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var index = 0;
            var items = GetStrings(Random);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_IndexOfCount_Appended()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = collection.Count;
            var items = GetStrings(Random);
            var array = collection.Concat(items).ToArray();

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_AllowsNull_InsertedRangeWithNull()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var index = GetIndex(collection, Random, true);
            var items = GetStrings(Random).WithNull(Random);
            var array = collection.ToArray().InsertItems(index, items);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_RandomCollectionIndexZero_FirstItems()
        {
            // Arrange
            var collection = GetStringList(Random);
            var items = GetStrings(Random);
            var index = 0;
            var array = collection.ToArray().InsertItems(index, items);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var items = GetStrings(Random);
            var index = collection.Count - 1;
            var array = collection.ToArray().InsertItems(index, items);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            var items = GetStrings(Random);
            var index = GetIndex(collection, Random, true);
            var array = collection.ToArray().InsertItems(index, items);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_RandomCollectionRandomIndex_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var items = GetStrings(Random);
            var index = GetIndex(collection, Random, true);
            var expectedEvents = items.SelectMany((item, i) => new[] { Inserted(item, index + i, collection), Added(item, 1, collection) }).Append(Changed(collection)).ToArray();

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, items), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void InsertRange_InsertEmptyRange_Nothing()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var items = NoStrings;
            var array = collection.ToArray();

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_InsertEmptyRange_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var items = NoStrings;

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, items), RaisesNoEventsFor(collection));
        }

        [Test]
        public void InsertRange_InsertEmptyRangeDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var items = NoStrings;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.InsertRange(index, items);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void InsertRange_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var collection = GetStringList(Random, ReferenceEqualityComparer, allowsNull: true);
            var index = GetIndex(collection, Random, true);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();
            var array = collection.ToArray();

            // Act & Assert
            Assert.That(() => collection.InsertRange(index, badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_InsertRangeDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var items = GetStrings(Random);
            var index = GetIndex(collection, Random, true);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.InsertRange(index, items);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void InsertRange_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void InsertRange_FixedSizeCollection_Fail()
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

        #region RemoveFirst()

        [Test]
        public void RemoveFirst_EmptyCollection_ViolatesPrecondtion()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => collection.RemoveFirst(), Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void RemoveFirst_RemoveFirstDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RemoveFirst();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void RemoveFirst_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = collection.First;
            var expectedEvents = new[] {
                RemovedAt(item, 0, collection),
                Removed(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.RemoveFirst(), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void RemoveFirst_RandomCollectionWithNullRemoveNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            collection[0] = null;

            // Act
            var removeFirst = collection.RemoveFirst();

            // Assert
            Assert.That(removeFirst, Is.Null);
        }

        [Test]
        public void RemoveFirst_SingleItemCollection_Empty()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetList(itemArray);

            // Act
            var removeFirst = collection.RemoveFirst();

            // Assert
            Assert.That(removeFirst, Is.SameAs(item));
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void RemoveFirst_RemoveFirstItem_Removed()
        {
            // Arrange
            var collection = GetStringList(Random);
            var firstItem = collection.First;
            var array = collection.Skip(1).ToArray();

            // Act
            var removeFirst = collection.RemoveFirst();

            // Assert
            Assert.That(removeFirst, Is.SameAs(firstItem));
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveFirst_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveFirst_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region RemoveLast()

        [Test]
        public void RemoveLast_EmptyCollection_ViolatesPrecondtion()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => collection.RemoveLast(), Violates.PreconditionSaying(CollectionMustBeNonEmpty));
        }

        [Test]
        public void RemoveLast_RemoveLastDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RemoveLast();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void RemoveLast_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = collection.Last;
            var expectedEvents = new[] {
                RemovedAt(item, collection.Count - 1, collection),
                Removed(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.RemoveLast(), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void RemoveLast_RandomCollectionWithNullRemoveNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            collection[collection.Count - 1] = null;

            // Act
            var removeLast = collection.RemoveLast();

            // Assert
            Assert.That(removeLast, Is.Null);
        }

        [Test]
        public void RemoveLast_SingleItemCollection_Empty()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetList(itemArray);

            // Act
            var removeLast = collection.RemoveLast();

            // Assert
            Assert.That(removeLast, Is.SameAs(item));
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void RemoveLast_RemoveLastItem_Removed()
        {
            // Arrange
            var collection = GetStringList(Random);
            var lastItem = collection.Last;
            var array = collection.Take(collection.Count - 1).ToArray();

            // Act
            var removeLast = collection.RemoveLast();

            // Assert
            Assert.That(removeLast, Is.SameAs(lastItem));
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveLast_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveLast_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
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