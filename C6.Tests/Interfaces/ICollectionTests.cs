// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Collections;
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
            Assert.That(() => enumerator.MoveNext(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(CollectionModified));
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
        public void Contains_DisallowNullContainsNull_ViolatesPrecondition()
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
        public void ContainsAll_DisallowsNullEnumerableWithNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull:false);
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
            var containedItems = items.Take(count).ToArray();
            items.Shuffle();
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
            var nonContainedItems = items.Take(count).Append(items.First()).ToArray();
            items.Shuffle();
            var collection = GetCollection(items, ReferenceEqualityComparer);

            // Act
            var containsAll = collection.ContainsAll(nonContainedItems);

            // Assert
            Assert.That(containsAll, Is.False);
        }

        [Test]
        public void ContainsAll_BadEnumerable_ThrowsExceptionButCollectionDoesntChange()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act & Assert
            Assert.That(() => collection.ContainsAll(badEnumerable), Throws.InstanceOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(items).Using(ReferenceEqualityComparer));
        }

        #endregion

        #region ContainsCount(T)

        [Test]
        public void ContainsCount_DisallowsNullContainsCountNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.ContainsCount(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void ContainsCount_EmptyCollection_Zero()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();

            // Act
            var containsCount = collection.ContainsCount(item);

            // Assert
            Assert.That(containsCount, Is.Zero);
        }

        [Test]
        public void ContainsCount_RandomCollectionWithCountEqualItems_Count()
        {
            // Arrange
            var item = GetLowercaseString(Random);
            var count = GetCount(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);

            // Act
            var containsCount = collection.ContainsCount(item);

            // Assert
            Assert.That(containsCount, Is.EqualTo(count));
        }

        [Test]
        public void ContainsCount_ValueTypeCollectionWithCountEqualItems_Count()
        {
            // Arrange
            var count = GetCount(Random);
            var equalityComparer = KeyEqualityComparer<int, int>();
            var items = GetKeyValuePairs(Random);
            var item = items.DifferentItem(() => new KVP(Random.Next()), equalityComparer);
            items = items.WithRepeatedItem(() => new KVP(item.Key, Random.Next()), count, Random);
            var collection = GetCollection(items, equalityComparer);

            // Act
            var containsCount = collection.ContainsCount(item);

            // Assert
            Assert.That(containsCount, Is.EqualTo(count));
        }

        [Test]
        public void ContainsCount_AllowsNull_Two()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            collection.Add(null);

            // Act
            var containsCount = collection.ContainsCount(null);

            // Assert
            Assert.That(containsCount, Is.EqualTo(2));
        }

        [Test]
        [Category("Unfinished")]
        public void ContainsCount_Set_Fail()
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
            Assert.That(() => enumerator.MoveNext(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(CollectionModified));
        }

        [Test]
        public void FindOrAdd_FindItemDuringEnumeration_ThrowsNoInvalidOperationException()
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
            var expected = HashCodeGenerator.GetUnsequencedHashCode(collection);

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
            var expected = HashCodeGenerator.GetUnsequencedHashCode(collection);

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
            var collection = GetCollection(items, allowsNull:true);
            var expected = HashCodeGenerator.GetUnsequencedHashCode(collection);

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
            var shuffledItems = items.ToArray();
            items.Shuffle(Random);
            var firstCollection = GetCollection(items);
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
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.With.Message.EqualTo(CollectionModified));
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
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.With.Message.EqualTo(CollectionModified));
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

        #region Update(T)

        // TODO: Test that the proper item is replaced (based on IsFifo) when several exist

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
            Assert.That(() => enumerator.MoveNext(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(CollectionModified));
        }

        // TODO: Null
        // TODO: Simple type, properly replaced

        // TODO: Proper item replaced based on FIFO

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
            var collection = GetStringCollection(Random, allowsNull:false);
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
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.With.Message.EqualTo(CollectionModified)); // TODO: introduce BecauseCollectionWasModified()
        }

        [Test]
        public void UpdateOut_UpdateDuringEnumeration_ThrowsNoInvalidOperationException()
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