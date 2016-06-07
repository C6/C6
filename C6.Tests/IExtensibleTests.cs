// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
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

        #region Inherited

        protected override IListenable<T> GetEmptyListenable<T>(bool allowsNull = false)
            => GetEmptyExtensible<T>(allowsNull: allowsNull);

        protected override IListenable<T> GetListenable<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false)
            => GetExtensible(enumerable, allowsNull: allowsNull);

        #endregion

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
        public void EqualityComparer_NoSpecifiedComparer_SameAsDefaultComparer()
        {
            // Arrange
            var collection = GetStringExtensible(Random, equalityComparer: null);

            // Act
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.Not.Null);
            Assert.That(equalityComparer, Is.SameAs(SCG.EqualityComparer<string>.Default));
        }

        [Test]
        public void EqualityComparer_CustomEqualityComparer_SameAsCustomEqualityComparer()
        {
            // Arrange
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);
            var collection = GetEmptyExtensible(customEqualityComparer);

            // Act
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(customEqualityComparer));
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
        public void Add_AllowsNullAddNull_True()
        {
            // Arrange
            var collection = GetStringExtensible(Random, allowsNull: true);

            // Act
            var add = collection.Add(null);

            // Assert
            Assert.That(add, Is.True);
        }

        [Test]
        public void Add_EmptyCollection_CollectionIsSameAsItem()
        {
            // Arrange
            var collection = GetEmptyExtensible<string>();
            var item = GetString(Random);
            var array = new[] { item };

            // Act
            var add = collection.Add(item);

            // Assert
            Assert.That(add, Is.True);
            Assert.That(collection, Is.EqualTo(array).ByReference<string>());
        }

        [Test]
        public void Add_AddDuplicateItem_AllowsDuplicates()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetExtensible(items, CaseInsensitiveStringComparer.Default);
            var duplicateItem = items.Choose(Random).ToLower();

            // Act
            var add = collection.Add(duplicateItem);

            // Assert
            Assert.That(add, Is.EqualTo(AllowsDuplicates));
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
            var add = items.Aggregate(true, (current, item) => current & collection.Add(item));

            // Assert
            Assert.That(add, Is.True);
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
            var add = false;

            // Act & Assert
            Assert.That(() => add = collection.Add(item), Raises(expectedEvents).For(collection));
            Assert.That(add, Is.True);
        }

        [Test]
        public void Add_AddDuplicateItem_RaisesNoEvents()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetExtensible(items, CaseInsensitiveStringComparer.Default);
            var duplicateItem = items.Choose(Random).ToLower();
            var add = true;

            // Act & Assert
            Assert.That(() => add = collection.Add(duplicateItem), RaisesNoEventsFor(collection));
            Assert.That(add, Is.False);
        }

        [Test]
        public void Add_AddDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringExtensible(Random);
            var item = GetString(Random);

            // Act & Assert
            Assert.That(() => collection.Add(item), Breaks.EnumeratorFor(collection));
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
            var collection = GetStringExtensible(Random, ReferenceEqualityComparer, allowsNull: true);
            var items = GetStrings(Random).WithNull(Random);
            var expected = collection.Union(items);

            // Act
            var addRange = collection.AddRange(items);

            // Assert
            Assert.That(addRange, Is.True);
            Assert.That(collection, Is.EquivalentTo(expected).ByReference<string>());
        }

        [Test]
        public void AddRange_AddEmptyEnumerableToEmptyCollection_Nothing()
        {
            // Arrange
            var collection = GetEmptyExtensible<string>();
            var empty = NoStrings;

            // Act
            var addRange = collection.AddRange(empty);

            // Assert
            Assert.That(addRange, Is.False);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void AddRange_AddEmptyEnumerable_Nothing()
        {
            // Arrange
            var collection = GetStringExtensible(Random, ReferenceEqualityComparer);
            var items = collection.ToArray();
            var empty = NoStrings;

            // Act
            var addRange = collection.AddRange(empty);

            // Assert
            Assert.That(addRange, Is.False);
            Assert.That(collection, Is.EqualTo(items).ByReference<string>());
        }

        [Test]
        public void AddRange_AddEmptyEnumerable_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringExtensible(Random, ReferenceEqualityComparer);
            var empty = NoStrings;
            var addRange = true;

            // Act & Assert
            Assert.That(() => addRange = collection.AddRange(empty), RaisesNoEventsFor(collection));
            Assert.That(addRange, Is.False);
        }

        [Test]
        public void AddRange_AddDuplicates_RaisesNoEvents()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringExtensible(Random, ReferenceEqualityComparer);
            var shuffledItems = collection.ShuffledCopy(Random);
            var addRange = true;

            // Act & Assert
            Assert.That(() => addRange = collection.AddRange(shuffledItems), RaisesNoEventsFor(collection));
            Assert.That(addRange, Is.False);
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
            Assert.That(collection, Is.EquivalentTo(expectedItems).ByReference<string>());
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
            var addRange = false;

            // Act & Assert
            Assert.That(() => addRange = collection.AddRange(items), Raises(expectedEvents).For(collection));
            Assert.That(addRange, Is.True);
        }

        [Test]
        public void AddRange_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var collection = GetStringExtensible(Random, ReferenceEqualityComparer, allowsNull: true);
            var items = collection.ToArray();
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();

            // Act & Assert
            Assert.That(() => collection.AddRange(badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EqualTo(items).ByReference<string>());
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void AddRange_AddDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringExtensible(Random);
            var items = GetStrings(Random);

            // Act & Assert
            Assert.That(() => collection.AddRange(items), Breaks.EnumeratorFor(collection));
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