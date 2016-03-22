// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;

using C6.Tests.Collections;
using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
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
            Assert.That(() => collection.Clear(), Is.Not.RaisingEventsFor(collection));
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
            Assert.That(() => collection.Clear(), _Is.Raising(expectedEvents).For(collection));
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
            Assert.That(() => enumerator.MoveNext(), Throws.TypeOf<InvalidOperationException>());
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



        #endregion

        #endregion
    }
}