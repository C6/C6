﻿// This file is part of the C6 Generic Collection Library for C# and CLI
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
    public abstract class ICollectionTests : IExtensibleTests
    {
        #region Factories

        protected abstract Speed ContainsSpeed { get; }

        protected abstract ICollection<T> GetEmptyCollection<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        protected abstract ICollection<T> GetCollection<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        #region Helpers

        private ICollection<int> GetIntCollection(Random random, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetCollection(GetIntegers(random, GetCount(random)), equalityComparer, allowsNull);

        private ICollection<int> GetIntCollection(Random random, int count, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetCollection(GetIntegers(random, count), equalityComparer, allowsNull);

        private ICollection<string> GetStringCollection(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetCollection(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        private ICollection<string> GetStringCollection(Randomizer random, int count, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetCollection(GetStrings(random, count), equalityComparer, allowsNull);

        #endregion

        #region Inherited

        protected override IExtensible<T> GetEmptyExtensible<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetEmptyCollection(equalityComparer, allowsNull);

        protected override IExtensible<T> GetExtensible<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetCollection(enumerable, equalityComparer, allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region Properties

        #region ContainsSpeed

        [Test]
        public void ContainsSpeed_RandomCollection_ContainsSpeed()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act
            var containsSpeed = collection.ContainsSpeed;

            // Assert
            Assert.That(containsSpeed, Is.EqualTo(ContainsSpeed));
        }

        #endregion

        #endregion

        #region Methods

        #region Clear

        [Test]
        public void Clear_EmptyCollection_IsEmpty()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();

            // Act
            collection.Clear();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Clear_SingleItem_IsEmpty()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetCollection(itemArray);

            // Act
            collection.Clear();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Clear_RandomCollection_IsEmpty()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act
            collection.Clear();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Clear_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();

            // Act & Assert
            Assert.That(() => collection.Clear(), RaisesNoEventsFor(collection));
        }

        [Test]
        public void Clear_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var count = GetCount(Random);
            var items = GetStrings(Random, count);
            var collection = GetCollection(items);
            var expectedEvents = new[] {
                Cleared(true, count, null, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.Clear(), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Clear_ClearDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Clear();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void Clear_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Clear_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region Contains(T)

        [Test]
        public void Contains_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.Contains(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Contains_AllowNullContainsNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);

            // Act
            var contains = collection.Contains(null);

            // Assert
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Contains_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();

            // Act
            var contains = collection.Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Contains_SingleItemCollectionNonDuplicateItem_False()
        {
            // Arrange
            var item = GetUppercaseString(Random);
            var itemArray = new[] { item };
            var collection = GetCollection(itemArray);
            var nonDuplicateItem = item.ToLower();

            // Act
            var contains = collection.Contains(nonDuplicateItem);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Contains_SingleItemCollectionDuplicateItem_True()
        {
            // Arrange
            var item = GetUppercaseString(Random);
            var itemArray = new[] { item };
            var collection = GetCollection(itemArray, CaseInsensitiveStringComparer.Default);
            var duplicateItem = item.ToLower();

            // Act
            var contains = collection.Contains(duplicateItem);

            // Assert
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Contains_SingleItemCollectionReferenceInequalItem_False()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetCollection(itemArray, ReferenceEqualityComparer);
            var nonDuplicateItem = string.Copy(item);

            // Act
            var contains = collection.Contains(nonDuplicateItem);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Contains_RandomCollectionNonContainedItem_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, ReferenceEqualityComparer);
            var item = Random.GetString();

            // Act
            var contains = collection.Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Contains_RandomCollectionContainedItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var item = items.Choose(Random).ToLower();

            // Act
            var contains = collection.Contains(item);

            // Assert
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Contains_RandomCollectionNonContainedItem_True()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var item = string.Copy(items.Choose(Random));

            // Act
            var contains = collection.Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        [Category("Unfinished")]
        public void Contains_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region ContainsAll(IEnumerable<T>)

        [Test]
        public void ContainsAll_NullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.ContainsAll(null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ContainsAll_DisallowsNullsInEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            var items = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => collection.ContainsAll(items), Violates.PreconditionSaying(ItemsMustBeNonNull));
        }

        [Test]
        public void ContainsAll_AllowNullEnumerableContainingNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            var arrayWithNull = new string[] { null };

            // Act
            var containsAll = collection.ContainsAll(arrayWithNull);

            // Assert
            Assert.That(containsAll, Is.True);
        }

        [Test]
        public void ContainsAll_EmptyCollectionEmptyEnumerable_True()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = Enumerable.Empty<string>();

            // Act
            var containsAll = collection.ContainsAll(items);

            // Assert
            Assert.That(containsAll, Is.True);
        }

        [Test]
        public void ContainsAll_EmptyCollectionNonEmptyEnumerable_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act
            var containsAll = collection.ContainsAll(items);

            // Assert
            Assert.That(containsAll, Is.False);
        }

        [Test]
        public void ContainsAll_RandomCollectionEmptyEnumerable_True()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var items = Enumerable.Empty<string>();

            // Act
            var containsAll = collection.ContainsAll(items);

            // Assert
            Assert.That(containsAll, Is.True);
        }

        [Test]
        public void ContainsAll_Subset_True()
        {
            // Arrange
            var count = GetCount(Random) / 2;
            var items = GetStrings(Random);
            var containedItems = items.ShuffledCopy(Random).Take(count);
            var collection = GetCollection(items, ReferenceEqualityComparer);

            // Act
            var containsAll = collection.ContainsAll(containedItems);

            // Assert
            Assert.That(containsAll, Is.True);
        }

        [Test]
        public void ContainsAll_SubsetWithDuplicates_False()
        {
            // Arrange
            var count = GetCount(Random) / 2;
            var items = GetStrings(Random);
            var nonContainedItems = items.Take(count).Append(items.First()).ShuffledCopy(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);

            // Act
            var containsAll = collection.ContainsAll(nonContainedItems);

            // Assert
            Assert.That(containsAll, Is.False);
        }

        [Test]
        public void ContainsAll_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer, allowsNull: true);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act & Assert
            Assert.That(() => collection.ContainsAll(badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(items).Using(ReferenceEqualityComparer));
        }

        #endregion

        #region CountDuplicates(T)

        [Test]
        public void CountDuplicates_DisallowsNullCountDuplicatesNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.CountDuplicates(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void CountDuplicates_EmptyCollection_Zero()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();

            // Act
            var countDuplicates = collection.CountDuplicates(item);

            // Assert
            Assert.That(countDuplicates, Is.Zero);
        }

        [Test]
        public void CountDuplicates_RandomCollectionWithCountEqualItems_Count()
        {
            // Arrange
            var item = GetLowercaseString(Random);
            var count = GetCount(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);

            // Act
            var countDuplicates = collection.CountDuplicates(item);

            // Assert
            Assert.That(countDuplicates, Is.EqualTo(count));
        }

        [Test]
        public void CountDuplicates_ValueTypeCollectionWithCountEqualItems_Count()
        {
            // Arrange
            var count = GetCount(Random);
            var equalityComparer = KeyEqualityComparer<int, int>();
            var items = GetKeyValuePairs(Random);
            var item = items.DifferentItem(() => new KVP(Random.Next()), equalityComparer);
            items = items.WithRepeatedItem(() => new KVP(item.Key, Random.Next()), count, Random);
            var collection = GetCollection(items, equalityComparer);

            // Act
            var countDuplicates = collection.CountDuplicates(item);

            // Assert
            Assert.That(countDuplicates, Is.EqualTo(count));
        }

        [Test]
        public void CountDuplicates_AllowsNull_Two()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            collection.Add(null);

            // Act
            var countDuplicates = collection.CountDuplicates(null);

            // Assert
            Assert.That(countDuplicates, Is.EqualTo(2));
        }

        [Test]
        [Category("Unfinished")]
        public void CountDuplicates_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region Find(ref T)

        [Test]
        public void Find_DisallowsNullFindNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            string item = null;

            // Act & Assert
            Assert.That(() => collection.Find(ref item), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Find_AllowsNullContainsNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            string item = null;

            // Act
            var find = collection.Find(ref item);

            // Assert
            Assert.That(find, Is.True);
            Assert.That(item, Is.Null);
        }

        [Test]
        public void Find_AllowsNullContainsNoNull_False()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, allowsNull: true);
            string item = null;

            // Act
            var find = collection.Find(ref item);

            // Assert
            Assert.That(find, Is.False);
            Assert.That(item, Is.Null);
        }

        [Test]
        public void Find_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();
            var refItem = item;

            // Act
            var find = collection.Find(ref refItem);

            // Assert
            Assert.That(find, Is.False);
            Assert.That(refItem, Is.SameAs(item));
        }

        [Test]
        public void Find_RandomCollectionDuplicateItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var item = items.Choose(Random);
            var refItem = item.ToLower();

            // Act
            var find = collection.Find(ref refItem);

            // Assert
            Assert.That(find, Is.True);
            Assert.That(refItem, Is.SameAs(item));
        }

        [Test]
        public void Find_RandomCollectionNonDuplicateItem_False()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var item = string.Copy(items.Choose(Random));
            var refItem = item;

            // Act
            var find = collection.Find(ref refItem);

            // Assert
            Assert.That(find, Is.False);
            Assert.That(refItem, Is.SameAs(item));
        }

        [Test]
        public void Find_ValueTypeCollectionNonContainedItem_False()
        {
            // Arrange
            var items = GetKeyValuePairs(Random);
            var collection = GetCollection(items, KeyEqualityComparer<int, int>());
            var item = items.DifferentItem(() => new KeyValuePair<int, int>(Random.Next(), Random.Next()));
            var refItem = item;

            // Act
            var find = collection.Find(ref refItem);

            // Assert
            Assert.That(find, Is.False);
            Assert.That(refItem, Is.EqualTo(item));
        }

        [Test]
        public void Find_ValueTypeCollectionContainedItem_True()
        {
            // Arrange
            var items = GetKeyValuePairs(Random);
            var collection = GetCollection(items, KeyEqualityComparer<int, int>());
            var item = items.Choose(Random);
            var refItem = new KeyValuePair<int, int>(item.Key, ~item.Value);

            // Act
            var find = collection.Find(ref refItem);

            // Assert
            Assert.That(find, Is.True);
            Assert.That(refItem, Is.EqualTo(item));
        }

        [Test]
        [Category("Unfinished")]
        public void Find_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        // TODO

        #endregion

        #region FindOrAdd(ref T)

        [Test]
        public void FindOrAdd_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            string item = null;

            // Act & Assert
            Assert.That(() => collection.FindOrAdd(ref item), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void FindOrAdd_AllowsNullFind_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            string item = null;

            // Act
            var findOrAdd = collection.FindOrAdd(ref item);

            // Assert
            Assert.That(findOrAdd, Is.True);
            Assert.That(item, Is.Null);
        }

        [Test]
        public void FindOrAdd_AllowsNullAdd_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);
            string item = null;

            // Act
            var findOrAdd = collection.FindOrAdd(ref item);

            // Assert
            Assert.That(findOrAdd, Is.False);
            Assert.That(item, Is.Null);
        }

        [Test]
        public void FindOrAdd_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();
            var refItem = item;

            // Act
            var findOrAdd = collection.FindOrAdd(ref refItem);

            // Assert
            Assert.That(findOrAdd, Is.False);
            Assert.That(refItem, Is.SameAs(item));
        }

        [Test]
        public void FindOrAdd_Add_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);
            var expectedEvents = new[] {
                Added(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.FindOrAdd(ref item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void FindOrAdd_RandomCollectionDuplicateItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var item = items.Choose(Random);
            var refItem = item.ToLower();

            // Act
            var findOrAdd = collection.FindOrAdd(ref refItem);

            // Assert
            Assert.That(findOrAdd, Is.True);
            Assert.That(refItem, Is.SameAs(item));
        }

        [Test]
        public void FindOrAdd_RandomCollectionNonDuplicateItem_False()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var item = string.Copy(items.Choose(Random));
            var refItem = item;

            // Act
            var findOrAdd = collection.FindOrAdd(ref refItem);

            // Assert
            Assert.That(findOrAdd, Is.False);
            Assert.That(refItem, Is.SameAs(item));
        }

        [Test]
        public void FindOrAdd_Find_RaisesNoEvents()
        {
            // Arrange
            var items = GetStrings(Random);
            var item = items.Choose(Random);
            var collection = GetCollection(items);

            // Act & Assert
            Assert.That(() => collection.FindOrAdd(ref item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void FindOrAdd_AddItemDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.FindOrAdd(ref item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void FindOrAdd_FindItemDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = items.Choose(Random).ToLower();
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.FindOrAdd(ref item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        [Category("Unfinished")]
        public void FindOrAdd_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void FindOrAdd_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region GetUnsequencedHashCode()

        [Test]
        public void GetUnsequencedHashCode_EmptyCollection_GeneratedHashCode()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var expected = UnsequencedEqualityComparer.GetUnsequencedHashCode(collection);

            // Act
            var unsequencedHashCode = collection.GetUnsequencedHashCode();

            // Assert
            Assert.That(unsequencedHashCode, Is.EqualTo(expected));
        }

        [Test]
        public void GetUnsequencedHashCode_RandomCollection_GeneratedHashCode()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var expected = UnsequencedEqualityComparer.GetUnsequencedHashCode(collection);

            // Act
            var unsequencedHashCode = collection.GetUnsequencedHashCode();

            // Assert
            Assert.That(unsequencedHashCode, Is.EqualTo(expected));
        }

        [Test]
        public void GetUnsequencedHashCode_RandomCollectionWithNull_GeneratedHashCode()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            var expected = UnsequencedEqualityComparer.GetUnsequencedHashCode(collection);

            // Act
            var unsequencedHashCode = collection.GetUnsequencedHashCode();

            // Assert
            Assert.That(unsequencedHashCode, Is.EqualTo(expected));
        }

        [Test]
        public void GetUnsequencedHashCode_EqualCollectionDifferentOrder_SameHashCode()
        {
            // Arrange
            var items = GetStrings(Random);
            var firstCollection = GetCollection(items);
            var shuffledItems = items.ShuffledCopy(Random);
            var secondCollection = GetCollection(shuffledItems);

            // Act
            var firstUnsequencedHashCode = firstCollection.GetUnsequencedHashCode();
            var secondUnsequencedHashCode = secondCollection.GetUnsequencedHashCode();

            // Assert
            Assert.That(firstUnsequencedHashCode, Is.EqualTo(secondUnsequencedHashCode));
        }

        [Test]
        public void GetUnsequencedHashCode_EqualButChangedCollection_SameHashCode()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var newItems = GetLowercaseStrings(Random);
            var collection = GetCollection(items);

            // Act
            var firstUnsequencedHashCode = collection.GetUnsequencedHashCode();
            collection.AddAll(newItems);
            collection.RemoveAll(newItems);
            var secondUnsequencedHashCode = collection.GetUnsequencedHashCode();

            // Assert
            Assert.That(firstUnsequencedHashCode, Is.EqualTo(secondUnsequencedHashCode));
        }

        [Test]
        public void GetUnsequencedHashCode_CachedValueIsUpdated_ExpectedHashCode()
        {
            // Arrange
            var sequence = GetStringCollection(Random, ReferenceEqualityComparer);
            var items = GetStrings(Random);
            var expected = GetCollection(items).GetUnsequencedHashCode();

            // Act
            var hashCode = sequence.GetUnsequencedHashCode();
            sequence.Clear();
            sequence.AddAll(items);
            hashCode = sequence.GetUnsequencedHashCode();

            // Assert
            Assert.That(hashCode, Is.EqualTo(expected));
        }

        // TODO: Test for shuffled list in IListTests

        #endregion

        #region Remove(T)

        [Test]
        public void RemoveT_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.Remove(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void RemoveT_AllowsNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);

            // Act
            var remove = collection.Remove(null);

            // Assert
            Assert.That(remove, Is.True);
        }

        [Test]
        public void RemoveT_RemoveExistingItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var existingItem = items.Choose(Random);
            var item = existingItem.ToLower();
            var expectedEvents = new[] {
                Removed(existingItem, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.Remove(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void RemoveT_RemoveNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);

            // Act & Assert
            Assert.That(() => collection.Remove(item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveT_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();

            // Act
            var remove = collection.Remove(item);

            // Assert
            Assert.That(remove, Is.False);
        }

        [Test]
        public void RemoveT_RemoveExistingItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = items.Choose(Random).ToLower();
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);

            // Act
            var remove = collection.Remove(item);

            // Assert
            Assert.That(remove, Is.True);
        }

        [Test]
        public void RemoveT_RemoveNewItem_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);

            // Act
            var remove = collection.Remove(item);

            // Assert
            Assert.That(remove, Is.False);
        }

        [Test]
        public void RemoveT_RemoveItemDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = items.Choose(Random).ToLower();
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Remove(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void RemoveT_RemoveItemDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Remove(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveT_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveT_DuplicatesByCounting_Fail()
        {
            Run.If(DuplicatesByCounting);

            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveT_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region Remove(T, out T)

        [Test]
        public void RemoveTOut_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            string removedItem;

            // Act & Assert
            Assert.That(() => collection.Remove(null, out removedItem), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void RemoveTOut_AllowsNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            string removedItem;

            // Act
            var remove = collection.Remove(null, out removedItem);

            // Assert
            Assert.That(remove, Is.True);
            Assert.That(removedItem, Is.Null);
        }

        [Test]
        public void RemoveTOut_RemoveExistingItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var existingItem = items.Choose(Random);
            var item = existingItem.ToLower();
            string removedItem;
            var expectedEvents = new[] {
                Removed(existingItem, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.Remove(item, out removedItem), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void RemoveTOut_RemoveNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);
            string removedItem;

            // Act & Assert
            Assert.That(() => collection.Remove(item, out removedItem), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveTOut_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();
            string removedItem;

            // Act
            var remove = collection.Remove(item, out removedItem);

            // Assert
            Assert.That(remove, Is.False);
            Assert.That(removedItem, Is.Null);
        }

        [Test]
        public void RemoveTOut_RemoveExistingItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var existingItem = items.Choose(Random);
            var item = existingItem.ToLower();
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            string removedItem;

            // Act
            var remove = collection.Remove(item, out removedItem);

            // Assert
            Assert.That(remove, Is.True);
            Assert.That(removedItem, Is.SameAs(existingItem));
        }

        [Test]
        public void RemoveTOut_RemoveNewItem_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);
            string removedItem;

            // Act
            var remove = collection.Remove(item, out removedItem);

            // Assert
            Assert.That(remove, Is.False);
            Assert.That(removedItem, Is.Null);
        }

        [Test]
        public void RemoveTOut_RemoveItemDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = items.Choose(Random).ToLower();
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            string removedItem;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Remove(item, out removedItem);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void RemoveTOut_RemoveItemDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);
            string removedItem;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Remove(item, out removedItem);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveTOut_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveTOut_DuplicatesByCounting_Fail()
        {
            Run.If(DuplicatesByCounting);

            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveTOut_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region RemoveAll(IEnumerable<T>)
        
        [Test]
        public void RemoveAll_AddNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveAll(null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void RemoveAll_DisallowNullInEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            var items = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveAll(items), Violates.PreconditionSaying(ItemsMustBeNonNull));
        }

        [Test]
        public void RemoveAll_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act
            var removeAll = collection.RemoveAll(items);

            // Assert
            Assert.That(removeAll, Is.False);
        }

        [Test]
        public void RemoveAll_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveAll(items), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveAll_EmptyEnumerable_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var items = Enumerable.Empty<string>();

            // Act
            var removeAll = collection.RemoveAll(items);

            // Assert
            Assert.That(removeAll, Is.False);
        }

        [Test]
        public void RemoveAll_EmptyEnumerable_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var items = Enumerable.Empty<string>();

            // Act & Assert
            Assert.That(() => collection.RemoveAll(items), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveAll_BothEmpty_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = Enumerable.Empty<string>();

            // Act
            var removeAll = collection.RemoveAll(items);

            // Assert
            Assert.That(removeAll, Is.False);
        }

        [Test]
        public void RemoveAll_BothEmpty_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = Enumerable.Empty<string>();

            // Act & Assert
            Assert.That(() => collection.RemoveAll(items), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveAll_NewItems_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var newItems = GetLowercaseStrings(Random);

            // Act
            var removeAll = collection.RemoveAll(newItems);

            // Assert
            Assert.That(removeAll, Is.False);
        }

        [Test]
        public void RemoveAll_NewItems_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var newItems = GetLowercaseStrings(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveAll(newItems), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveAll_RemoveCollectionItself_True()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);

            // Act
            var removeAll = collection.RemoveAll(items);

            // Assert
            Assert.That(removeAll, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void RemoveAll_RemoveSubsetDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);
            var count = GetCount(Random);
            var existingItems = items.Take(count).ShuffledCopy(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RemoveAll(existingItems);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void RemoveAll_RemoveNewItemsDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var newItems = GetLowercaseStrings(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RemoveAll(newItems);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void RemoveAll_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer, allowsNull: true);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act & Assert
            Assert.That(() => collection.RemoveAll(badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void RemoveAll_RemoveOneOfEachDuplicate_AllButOneLeft()
        {
            // Arrange
            var items = GetStrings(Random);
            var repeatedItems = items.SelectMany(item => item.Repeat(Random.Next(2, 5))).ToArray();
            var collection = GetCollection(repeatedItems);
            var itemCounts = repeatedItems.GroupBy(item => item).Select(grouping => new KeyValuePair<string, int>(grouping.Key, grouping.Count() - 1));
            
            // Act
            var removeAll = collection.RemoveAll(items);

            // Assert
            Assert.That(removeAll, Is.True);
            Assert.That(collection.GroupBy(item => item).Select(grouping => new KeyValuePair<string, int>(grouping.Key, grouping.Count())), Is.EquivalentTo(itemCounts));
        }

        [Test]
        public void RemoveAll_RemoveOverlap_OverlapRemoved()
        {
            // Arrange
            var remainingItems = GetStrings(Random);
            var overlappingItems = GetStrings(Random);
            var items = remainingItems.Concat(overlappingItems).ShuffledCopy(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var itemsToRemove = GetStrings(Random).Concat(overlappingItems).ShuffledCopy(Random);

            // Act
            var removeAll = collection.RemoveAll(itemsToRemove);

            // Assert
            Assert.That(removeAll, Is.True);
            Assert.That(collection, Is.EquivalentTo(remainingItems));
        }
        
        [Test]
        [Ignore("Figure out the best way to assess events")]
        public void RemoveAll_RemoveOverlap_RaisesExpectedEvents()
        {
            // Arrange
            var remainingItems = GetStrings(Random);
            var overlappingItems = GetStrings(Random);
            var items = remainingItems.Concat(overlappingItems).ShuffledCopy(Random);
            var collection = GetCollection(items);
            var itemsToRemove = GetStrings(Random).Concat(overlappingItems).ShuffledCopy(Random);
            var expectedEvents = new[] {
                // TODO: Add missing events
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.RemoveAll(itemsToRemove), Raises(expectedEvents).InNoParticularOrder().For(collection)); // TODO: Ignore order
        }

        // TODO: Remove subset
        // TODO: Raises events

        [Test]
        [Category("Unfinished")]
        public void RemoveAll_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveAll_FixedSizeCollection_Fail()
        {
            Run.If(IsFixedSize);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveAll_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
            // TODO: See RemoveAll_RemoveOneOfEachDuplicate_AllButOneLeft()
        }


        // TODO: Look at AddAll for inspiration

        #endregion

        #region RemoveDuplicates(T)

        [Test]
        public void RemoveDuplicates_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.RemoveDuplicates(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void RemoveDuplicates_RemoveNull_Removed()
        {
            // Arrange
            var count = GetCount(Random);
            var items = GetStrings(Random).WithRepeatedItem(() => null, count, Random);
            var collection = GetCollection(items, allowsNull: true);

            // Act
            var removeDuplicates = collection.RemoveDuplicates(null);

            // Assert
            Assert.That(removeDuplicates, Is.True);
            Assert.That(collection, Has.No.Null);
        }

        // TODO: Find a better way to test the differences caused by DuplicatesByCounting
        [Test]
        public void RemoveDuplicates_ExistingItems_RaisesExpectedEvents()
        {
            // Arrange
            var count = GetCount(Random);
            var item = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);
            var eventCount = AllowsDuplicates ? count : 1;
            var expectedEvents = DuplicatesByCounting
                ? new[] {
                    Removed(item, eventCount, collection),
                    Changed(collection),
                }
                : Removed(item, 1, collection).Repeat(eventCount).Append(Changed(collection)).ToArray();

            // Act & Assert
            Assert.That(() => collection.RemoveDuplicates(item), Raises(expectedEvents).For(collection));
        }

        // TODO: Test events properly

        [Test]
        public void RemoveDuplicates_RandomCollectionRemoveNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveDuplicates(item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveDuplicates_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();

            // Act
            var removeDuplicates = collection.RemoveDuplicates(item);

            // Assert
            Assert.That(removeDuplicates, Is.False);
        }

        [Test]
        public void RemoveDuplicates_RemoveDuplicatesDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);
            var item = items.Choose(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RemoveDuplicates(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void RemoveDuplicates_DuplicateItems_Empty()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = item.Repeat(count);
            var collection = GetCollection(items);

            // Act
            var removeDuplicates = collection.RemoveDuplicates(item);

            // Assert
            Assert.That(removeDuplicates, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void RemoveDuplicates_RandomCollectionRemoveNewItem_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);

            // Act
            var removeDuplicates = collection.RemoveDuplicates(item);

            // Assert
            Assert.That(removeDuplicates, Is.False);
        }

        [Test]
        public void RemoveDuplicates_EverySecondIsDuplicate_True([Values(true, false)] bool removeFirst)
        {
            // Arrange
            var count = GetCount(Random);
            var firstItem = GetUppercaseString(Random);
            var secondItem = GetLowercaseString(Random);
            var items = Enumerable.Range(0, count).SelectMany(i => new[] { firstItem, secondItem }).ToArray();
            var collection = GetCollection(items);
            var itemToRemove = removeFirst ? firstItem : secondItem;

            // Act
            var removeDuplicates = collection.RemoveDuplicates(itemToRemove);

            // Assert
            Assert.That(removeDuplicates, Is.True);
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveDuplicates_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveDuplicates_DuplicatesByCounting_Fail()
        {
            Run.If(DuplicatesByCounting);

            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveDuplicates_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region RetrieveAll(IEnumerable<T>)

        [Test]
        public void RetainAll_NullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.RetainAll(null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void RetainAll_DisallowsNullsInEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            var items = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => collection.RetainAll(items), Violates.PreconditionSaying(ItemsMustBeNonNull));
        }

        [Test]
        public void RetainAll_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act
            var retainAll = collection.RetainAll(items);

            // Assert
            Assert.That(retainAll, Is.False);
        }

        [Test]
        public void RetainAll_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act & Assert
            Assert.That(() => collection.RetainAll(items), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RetainAll_EmptyEnumerable_Empty()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var items = Enumerable.Empty<string>();

            // Act
            var retainAll = collection.RetainAll(items);

            // Assert
            Assert.That(retainAll, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        [Ignore("Figure out the best way to assess events")]
        public void RetainAll_EmptyEnumerable_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);
            var itemsToRemove = Enumerable.Empty<string>();
            var expectedEvents = new[] {
                // TODO: Add events
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.RetainAll(itemsToRemove), Raises(expectedEvents).InNoParticularOrder().For(collection));
        }

        [Test]
        public void RetainAll_BothEmpty_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Enumerable.Empty<string>();

            // Act
            var retainAll = collection.RetainAll(item);

            // Assert
            Assert.That(retainAll, Is.False);
        }

        [Test]
        public void RetainAll_BothEmpty_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Enumerable.Empty<string>();

            // Act & Assert
            Assert.That(() => collection.RetainAll(item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RetainAll_NewItems_Empty()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var newItems = GetLowercaseStrings(Random);

            // Act
            var retainAll = collection.RetainAll(newItems);

            // Assert
            Assert.That(retainAll, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        [Ignore("Figure out the best way to assess events")]
        public void RetainAll_NewItems_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var newItems = GetLowercaseStrings(Random);
            var expectedEvents = new[] {
                // TODO: Add events
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.RetainAll(newItems), Raises(expectedEvents).InNoParticularOrder().For(collection));
        }

        [Test]
        public void RetainAll_RetainCollectionItself_Unchanged()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);

            // Act
            var retainAll = collection.RetainAll(items);

            // Assert
            Assert.That(retainAll, Is.False);
            Assert.That(collection, Is.EquivalentTo(items));
        }

        // TODO: Event version of above

        [Test]
        public void RetainAll_RetainSubsetDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var count = GetCount(Random);
            var items = GetStrings(Random, count);
            var collection = GetCollection(items);
            var existingItems = items.Take(count / 2).ShuffledCopy(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RetainAll(existingItems);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void RetainAll_RetainCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RetainAll(items);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void RetainAll_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer, allowsNull: true);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act & Assert
            Assert.That(() => collection.RetainAll(badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void RetainAll_RetainOverlap_OverlapRetained()
        {
            // Arrange
            var removedItems = GetStrings(Random);
            var overlappingItems = GetStrings(Random);
            var items = removedItems.Concat(overlappingItems).ShuffledCopy(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var itemsToRetain = GetStrings(Random).Concat(overlappingItems).ShuffledCopy(Random);

            // Act
            var retainAll = collection.RetainAll(itemsToRetain);

            // Assert
            Assert.That(retainAll, Is.True);
            Assert.That(collection, Is.EquivalentTo(overlappingItems));
        }

        [Test]
        [Ignore("Figure out the best way to assess events")]
        public void RetainAll_RetainOverlap_RaisesExpectedEvents()
        {
            // Arrange
            var removedItems = GetStrings(Random);
            var overlappingItems = GetStrings(Random);
            var items = removedItems.Concat(overlappingItems).ShuffledCopy(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var itemsToRetain = GetStrings(Random).Concat(overlappingItems).ShuffledCopy(Random);
            var expectedEvents = new[] {
                // TODO: Add missing events
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.RetainAll(itemsToRetain), Raises(expectedEvents).InNoParticularOrder().For(collection)); // TODO: Ignore order
        }

        // TODO: Retain subset
        // TODO: Raises events

        #endregion

        #region UniqueItems

        [Test]
        public void UniqueItems_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.Empty);
        }

        [Test]
        public void UniqueItems_AllUniqueItems_EqualToItself()
        {
            // Arrange
            var collection = GetStringCollection(Random, ReferenceEqualityComparer);

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.EquivalentTo(collection).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void UniqueItems_EqualItems_OneItem()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var itemArray = new[] { item };
            var items = item.Repeat(count);
            var collection = GetCollection(items);

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.EquivalentTo(itemArray));
        }

        [Test]
        public void UniqueItems_RepeatedItems_OnlyUniqueItems()
        {
            // Arrange
            var originalItems = GetStrings(Random);
            var items = originalItems.SelectMany(item => item.Repeat(Random.Next(1, 4)));
            var collection = GetCollection(items);

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.EquivalentTo(originalItems));
        }

        #endregion

        #region UnsequencedEquals(ICollection<T>)

        [Test]
        public void UnsequencedEquals_EmptyCollections_True()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var otherCollection = GetEmptyCollection<string>();

            // Act
            var unsequencedEquals = collection.UnsequencedEquals(otherCollection);

            // Assert
            Assert.That(unsequencedEquals, Is.True);
        }

        [Test]
        public void UnsequencedEquals_EmptyAndNullCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();

            // Act
            var unsequencedEquals = collection.UnsequencedEquals(null);

            // Assert
            Assert.That(unsequencedEquals, Is.False);
        }

        [Test]
        public void UnsequencedEquals_EmptyAndRandomCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var otherCollection = GetStringCollection(Random);

            // Act
            var unsequencedEquals = collection.UnsequencedEquals(otherCollection);

            // Assert
            Assert.That(unsequencedEquals, Is.False);
        }

        [Test]
        public void UnsequencedEquals_RandomCollections_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var otherItems = GetLowercaseStrings(Random);
            var otherCollection = GetCollection(otherItems);

            // Act
            var unsequencedEquals = collection.UnsequencedEquals(otherCollection);

            // Assert
            Assert.That(unsequencedEquals, Is.False);
        }

        [Test]
        public void UnsequencedEquals_RandomCollectionAndItSelf_True()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act
            var unsequencedEquals = collection.UnsequencedEquals(collection);

            // Assert
            Assert.That(unsequencedEquals, Is.True);
        }

        [Test]
        public void UnsequencedEquals_EqualCollections_True()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);
            var otherCollection = GetCollection(items);

            // Act
            var unsequencedEquals = collection.UnsequencedEquals(otherCollection);

            // Assert
            Assert.That(unsequencedEquals, Is.True);
        }

        [Test]
        public void UnsequencedEquals_UnsequencedEqualCollections_True()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);
            var shuffledItems = items.ShuffledCopy(Random);
            var otherCollection = GetCollection(shuffledItems);

            // Act
            var unsequencedEquals = collection.UnsequencedEquals(otherCollection);

            // Assert
            Assert.That(unsequencedEquals, Is.True);
        }

        [Test]
        public void UnsequencedEquals_DifferentEqualityComparers_TrueInOneDirection()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var otherItems = items.Select(item => item.ToLower());
            var otherCollection = GetCollection(otherItems);

            // Act
            var collectionUnsequencedEqualsOtherCollection = collection.UnsequencedEquals(otherCollection);
            var otherCollectionUnsequencedEqualsCollection = otherCollection.UnsequencedEquals(collection);

            // Assert
            Assert.That(collectionUnsequencedEqualsOtherCollection, Is.True);
            Assert.That(otherCollectionUnsequencedEqualsCollection, Is.False);
        }

        [Test]
        public void UnsequencedEquals_EqualItemsButDifferentMultiplicity_False()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);
            var otherItems = items.SelectMany(item => item.Repeat(Random.Next(1, 4)));
            var otherCollection = GetCollection(otherItems);

            // Act
            var unsequencedEquals = collection.UnsequencedEquals(otherCollection);

            // Assert
            Assert.That(unsequencedEquals, Is.False);
        }

        [Test]
        public void UnsequencedEquals_EqualHashButDifferentItems_False()
        {
            // Arrange
            var items = new[] { -1657792980, -1570288808 };
            var collection = GetCollection(items);
            var otherItems = new[] { 1862883298, -272461342 };
            var otherCollection = GetCollection(otherItems);

            // Act
            var unsequencedHashCode = collection.GetUnsequencedHashCode();
            var otherUnsequencedHashCode = otherCollection.GetUnsequencedHashCode();
            var unsequencedEquals = collection.UnsequencedEquals(otherCollection);

            // Assert
            Assert.That(unsequencedHashCode, Is.EqualTo(otherUnsequencedHashCode));
            Assert.That(unsequencedEquals, Is.False);
        }

        [Test]
        [Category("Unfinished")]
        public void UnsequencedEquals_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region Update(T)

        // TODO: Test that the proper item is replaced (based on RemovesFromBeginning) when several exist

        [Test]
        public void Update_DisallowsNullUpdateNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.Update(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Update_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();

            // Act
            var update = collection.Update(item);

            // Assert
            Assert.That(update, Is.False);
        }

        [Test]
        public void Update_RandomCollectionUpdateNonContainedItem_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);

            // Act
            var update = collection.Update(item);

            // Assert
            Assert.That(update, Is.False);
        }

        [Test]
        public void Update_RandomCollectionUpdateContainedItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var item = items.Choose(Random).ToLower();

            // Act
            var update = collection.Update(item);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void Update_RandomCollectionUpdateContainedItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var oldItem = items.Choose(Random);
            var item = oldItem.ToLower();
            var expectedEvents = new[] {
                Removed(oldItem, 1, collection),
                Added(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.Update(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Update_IntegerCollectionUpdateContainedItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = new[] { 4, 54, 56, 8 };
            var collection = GetCollection(items, TenEqualityComparer.Default);
            var count = DuplicatesByCounting ? 2 : 1;
            var item = 53;
            var expectedEvents = new[] {
                Removed(54, count, collection),
                Added(item, count, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.Update(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Update_RandomCollectionUpdateNonContainedItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);

            // Act & Assert
            Assert.That(() => collection.Update(item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void Update_DuplicateItemCollection_ReplacesOneItem()
        {
            // TODO
            // Arrange
            var count = GetCount(Random);
            var item = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);

            // Act
            var update = collection.Update(item);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void Update_RandomCollectionUpdateFirstItem_Updated()
        {
            // Arrange
            var item = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random);
            items[0] = item;
            var collection = GetCollection(items);

            // Act
            var update = collection.Update(item);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void Update_UpdateItemDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = items.Choose(Random).ToLower();
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Update(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        // TODO: Null
        // TODO: Simple type, properly replaced

        // TODO: Proper item replaced based on RemovesFromBeginning

        [Test]
        [Category("Unfinished")]
        public void Update_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Update_DuplicatesByCounting_Fail()
        {
            Run.If(DuplicatesByCounting);

            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Update_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #region Update(T, out T)

        [Test]
        public void UpdateOut_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            string oldItem;

            // Act & Assert
            Assert.That(() => collection.Update(null, out oldItem), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void UpdateOut_AllowsNull_UpdatesNull()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            string oldItem;

            // Act
            var update = collection.Update(null, out oldItem);

            // Assert
            Assert.That(update, Is.True);
            Assert.That(oldItem, Is.Null);
        }

        [Test]
        public void UpdateOut_UpdateExistingItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var existingItem = items.Choose(Random);
            var item = existingItem.ToLower();
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            string oldItem;

            // Act
            var update = collection.Update(item, out oldItem);

            // Assert
            Assert.That(update, Is.True);
            Assert.That(oldItem, Is.SameAs(existingItem));
        }

        [Test]
        public void UpdateOut_UpdateNewItem_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);
            string oldItem;

            // Act
            var update = collection.Update(item, out oldItem);

            // Assert
            Assert.That(update, Is.False);
            Assert.That(oldItem, Is.Null);
        }

        [Test]
        public void UpdateOut_UpdateExistingItem_RaisesExpectedEvents()
        {
            // Arrange
            var count = GetCount(Random);
            var item = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);
            var duplicateItem = string.Copy(item);
            string oldItem;
            var eventCount = DuplicatesByCounting ? count : 1;
            var expectedEvents = new[] {
                Removed(item, eventCount, collection),
                Added(duplicateItem, eventCount, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.Update(duplicateItem, out oldItem), Raises(expectedEvents).For(collection));
        }

        // TODO: test that the right item is removed for IList<T>

        [Test]
        public void UpdateOut_UpdateNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);
            string oldItem;

            // Act & Assert
            Assert.That(() => collection.Update(item, out oldItem), RaisesNoEventsFor(collection));
        }

        [Test]
        public void UpdateOut_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();
            string oldItem;

            // Act
            var update = collection.Update(item, out oldItem);

            // Assert
            Assert.That(update, Is.False);
            Assert.That(oldItem, Is.Null);
        }

        [Test]
        public void UpdateOut_UpdateDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = items.Choose(Random).ToLower();
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            string oldItem;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Update(item, out oldItem);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void UpdateOut_UpdateDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);
            string oldItem;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Update(item, out oldItem);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOut_ReadOnlyCollection_Fail()
        {
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOut_DuplicatesByCounting_Fail()
        {
            Run.If(DuplicatesByCounting);

            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.Fail("Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOut_Set_Fail()
        {
            Run.If(!AllowsDuplicates);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion
    }
}