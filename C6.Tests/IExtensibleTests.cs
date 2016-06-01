// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.Collections.ExceptionMessages;
using static C6.Tests.Helpers.CollectionEvent;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class IExtensibleTests : IListenableTests
    {
        #region Factories

        protected abstract bool AllowsDuplicates { get; }
        protected abstract bool DuplicatesByCounting { get; }
        protected abstract bool IsFixedSize { get; }
        protected abstract bool IsReadOnly { get; }

        protected abstract IExtensible<T> GetEmptyExtensible<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        protected abstract IExtensible<T> GetExtensible<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        #region Helpers

        private IExtensible<int> GetIntExtensible(Random random, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetExtensible(GetIntegers(random, GetCount(random)), equalityComparer, allowsNull);

        private IExtensible<int> GetIntExtensible(Random random, int count, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetExtensible(GetIntegers(random, count), equalityComparer, allowsNull);

        private IExtensible<string> GetStringExtensible(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetExtensible(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        private IExtensible<string> GetStringExtensible(Randomizer random, int count, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetExtensible(GetStrings(random, count), equalityComparer, allowsNull);

        #endregion

        #region Inherited

        protected override IListenable<T> GetEmptyListenable<T>(bool allowsNull = false)
            => GetEmptyExtensible<T>(allowsNull: allowsNull);

        protected override IListenable<T> GetListenable<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false)
            => GetExtensible(enumerable, allowsNull: allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region Properties

        #region AllowsDuplicates

        [Test]
        public void AllowsDuplicates_RandomCollection_AllowsDuplicates()
        {
            // Arrange
            var collection = GetStringExtensible(Random);

            // Act
            var allowsDuplicates = collection.AllowsDuplicates;

            // Assert
            Assert.That(allowsDuplicates, Is.EqualTo(AllowsDuplicates));
        }

        #endregion

        #region DuplicatesByCounting

        [Test]
        public void DuplicatesByCounting_RandomCollection_DuplicatesByCounting()
        {
            // Arrange
            var collection = GetStringExtensible(Random);

            // Act
            var duplicatesByCounting = collection.DuplicatesByCounting;

            // Assert
            Assert.That(duplicatesByCounting, Is.EqualTo(DuplicatesByCounting));
        }

        #endregion

        #region EqualityComparer

        [Test]
        public void EqualityComparer_DefaultComparer_NotNull()
        {
            // Arrange
            var collection = GetStringExtensible(Random);

            // Act
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.Not.Null);
        }

        [Test]
        public void EqualityComparer_CustomEqualityComparer_Equal()
        {
            // Arrange
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);
            var collection = GetEmptyExtensible(customEqualityComparer);

            // Act
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(customEqualityComparer));
        }

        [Test]
        public void EqualityComparer_DefaultEqualityComparer_Equal()
        {
            // Arrange
            var defaultEqualityComparer = SCG.EqualityComparer<int>.Default;
            var collection = GetEmptyExtensible<int>();

            // Act
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(defaultEqualityComparer));
        }

        #endregion

        #region IsFixedSize

        [Test]
        public void IsFixedSize_RandomCollection_IsFixedSize()
        {
            // Arrange
            var collection = GetStringExtensible(Random);

            // Act
            var isFixedSize = collection.IsFixedSize;

            // Assert
            Assert.That(isFixedSize, Is.EqualTo(IsFixedSize));
        }

        #endregion

        #region IsReadOnly

        [Test]
        public void IsReadOnly_RandomCollection_IsReadOnly()
        {
            // Arrange
            var collection = GetStringExtensible(Random);

            // Act
            var isReadOnly = collection.IsReadOnly;

            // Assert
            Assert.That(isReadOnly, Is.EqualTo(IsReadOnly));
        }

        #endregion

        #endregion

        #region Methods

        #region Add(T)

        [Test]
        public void Add_DisallowsNullAddNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringExtensible(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.Add(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Add_AllowsNullAddNull_ReturnsTrue()
        {
            // Arrange
            var collection = GetStringExtensible(Random, allowsNull: true);

            // Act
            var result = collection.Add(null);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Add_EmptyCollectionAddItem_CollectionIsSingleItemCollection()
        {
            // Arrange
            var collection = GetEmptyExtensible<string>();
            var item = Random.GetString();
            var itemArray = new[] { item };

            // Act
            var result = collection.Add(item);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(collection, Is.EqualTo(itemArray));
        }

        [Test]
        public void Add_AddDuplicateItem_AllowsDuplicates()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetExtensible(items, CaseInsensitiveStringComparer.Default);
            var duplicateItem = items.Choose(Random).ToLower();

            // Act
            var result = collection.Add(duplicateItem);

            // Assert
            Assert.That(result, Is.EqualTo(AllowsDuplicates));
        }

        // TODO: Add test to IList<T>.Add ensuring that order is the same
        [Test]
        public void Add_ManyItems_Equivalent()
        {
            // Arrange
            var referenceEqualityComparer = ComparerFactory.CreateReferenceEqualityComparer<string>();
            var collection = GetEmptyExtensible(referenceEqualityComparer);
            var count = Random.Next(100, 250);
            var items = GetStrings(Random, count);

            // Act
            foreach (var item in items) {
                collection.Add(item); // TODO: Verify that items were added?
            }

            // Assert
            Assert.That(collection, Is.EquivalentTo(items));
        }

        [Test]
        public void Add_AddItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetExtensible(items);
            var item = GetLowercaseString(Random);
            var expectedEvents = new[] {
                Added(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.Add(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Add_AddDuplicateItem_RaisesNoEvents()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetExtensible(items, CaseInsensitiveStringComparer.Default);
            var duplicateItem = items.Choose(Random).ToLower();

            // Act & Assert
            Assert.That(() => collection.Add(duplicateItem), RaisesNoEventsFor(collection));
        }

        [Test]
        public void Add_AddItemDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringExtensible(Random);
            var item = Random.GetString();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Add(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void Add_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Add_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Add_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region AddRange(IEnumerable<T>)

        // TODO: Add test with unique items?

        [Test]
        public void AddRange_AddNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringExtensible(Random);

            // Act & Assert
            Assert.That(() => collection.AddRange(null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void AddRange_DisallowNullAddNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringExtensible(Random, allowsNull: false);
            var items = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => collection.AddRange(items), Violates.PreconditionSaying(ItemsMustBeNonNull));
        }

        [Test]
        public void AddRange_AllowNullAddNull_True()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetExtensible(items, ReferenceEqualityComparer, allowsNull: true);
            var newItems = GetStrings(Random).WithNull(Random);
            var allItems = items.Union(newItems);

            // Act
            var addRange = collection.AddRange(newItems);

            // Assert
            Assert.That(addRange, Is.True);
            Assert.That(collection, Is.EquivalentTo(allItems));
        }

        [Test]
        public void AddRange_EmptyEnumerableAndCollection_Nothing()
        {
            // Arrange
            var collection = GetEmptyExtensible<string>();
            var empty = NoStrings;

            // Act
            collection.AddRange(empty);

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void AddRange_AddEmptyEnumerable_Nothing()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetExtensible(items, ReferenceEqualityComparer);
            var empty = NoStrings;

            // Act
            var addRange = collection.AddRange(empty);

            // Assert
            Assert.That(addRange, Is.False);
            Assert.That(collection, Is.EquivalentTo(items));
        }

        [Test]
        public void AddRange_AddEmptyEnumerable_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringExtensible(Random, ReferenceEqualityComparer);
            var empty = NoStrings;

            // Act & Assert
            Assert.That(() => collection.AddRange(empty), RaisesNoEventsFor(collection));
        }

        [Test]
        public void AddRange_AddDuplicates_RaisesNoEvents()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var items = GetStrings(Random);
            var collection = GetExtensible(items, ReferenceEqualityComparer);
            var shuffledItems = items.ShuffledCopy(Random);

            // Act & Assert
            Assert.That(() => collection.AddRange(shuffledItems), RaisesNoEventsFor(collection));
        }

        [Test]
        public void AddRange_AddDuplicates_ExpectedItems()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetExtensible(items, ReferenceEqualityComparer);
            var expectedItems = AllowsDuplicates ? items.Concat(items) : items;

            // Act
            var addRange = collection.AddRange(items);

            // Assert
            Assert.That(addRange, Is.EqualTo(AllowsDuplicates));
            Assert.That(collection, Is.EquivalentTo(expectedItems));
        }

        [Test]
        public void AddRange_AddItemsWithDuplicates_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringExtensible(Random, ReferenceEqualityComparer);
            var item1 = Random.GetString();
            var item2 = Random.GetString();
            var item3 = Random.GetString();
            var items = new[] { item1, item2, item1, item3 };
            var expectedEvents = AllowsDuplicates
                ? new[] {
                    Added(item1, 1, collection),
                    Added(item2, 1, collection),
                    Added(item1, 1, collection),
                    Added(item3, 1, collection),
                    Changed(collection)
                }
                : new[] {
                    Added(item1, 1, collection),
                    Added(item2, 1, collection),
                    Added(item3, 1, collection),
                    Changed(collection)
                };

            // Act & Assert
            Assert.That(() => collection.AddRange(items), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void AddRange_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetExtensible(items, ReferenceEqualityComparer, allowsNull: true);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act & Assert
            Assert.That(() => collection.AddRange(badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void AddRange_AddItemsDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringExtensible(Random);
            var items = GetStrings(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.AddRange(items);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void AddRange_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void AddRange_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void AddRange_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion
    }
}