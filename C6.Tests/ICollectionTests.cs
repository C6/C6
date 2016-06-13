// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;
using System.Text;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.Collections.ExceptionMessages;
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

        private ICollection<T> GetCollection<T>(params T[] items) => GetCollection((SCG.IEnumerable<T>) items);

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
        public void Clear_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();

            // Act & Assert
            Assert.That(() => collection.Clear(), RaisesNoEventsFor(collection));
        }

        // TODO: Does this actually test anything? The first call to MoveNext() will always return false.
        [Test]
        public void Clear_ClearEmptyCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Clear();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void Clear_SingleItem_IsEmpty()
        {
            // Arrange
            var item = GetString(Random);
            var collection = GetCollection(item);

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
        public void Clear_RandomCollectionWithNull_IsEmpty()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);

            // Act
            collection.Clear();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Clear_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var count = collection.Count;
            var expectedEvents = new[] {
                Cleared(true, count, null, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.Clear(), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void Clear_ClearDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act & Assert
            Assert.That(() => collection.Clear(), Breaks.EnumeratorFor(collection));
        }

        [Test]
        [Category("Unfinished")]
        public void Clear_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Clear_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
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
        public void Contains_AllowsNullExistingNull_True()
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
        public void Contains_AllowsNullNewNull_False()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, allowsNull: true);

            // Act
            var contains = collection.Contains(null);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Contains_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);

            // Act
            var contains = collection.Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Contains_SingleItemCollectionNewItem_False()
        {
            // Arrange
            var item = GetUppercaseString(Random);
            var collection = GetCollection(item);
            var newItem = item.ToLower();

            // Act
            var contains = collection.Contains(newItem);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Contains_SingleItemCollectionExistingItem_True()
        {
            // Arrange
            var item = GetUppercaseString(Random);
            var collection = GetCollection(new[] { item }, CaseInsensitiveStringComparer.Default);
            var existingItem = item.ToLower();

            // Act
            var contains = collection.Contains(existingItem);

            // Assert
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Contains_RandomCollectionNewItem_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));

            // Act
            var contains = collection.Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Contains_RandomCollectionExistingItem_True()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var item = collection.Choose(Random).ToLower();

            // Act
            var contains = collection.Contains(item);

            // Assert
            Assert.That(contains, Is.True);
        }

        [Test]
        [Category("Unfinished")]
        public void Contains_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region ContainsRange(IEnumerable<T>)

        [Test]
        public void ContainsRange_NullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.ContainsRange(null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ContainsRange_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            var items = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => collection.ContainsRange(items), Violates.PreconditionSaying(ItemsMustBeNonNull));
        }

        [Test]
        public void ContainsRange_AllowsNullExistingNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            var count = GetCount(Random);
            var subset = collection.ShuffledCopy(Random).Take(count).ToArray();
            var subsetWithNull = subset.Contains(null) ? subset : subset.WithNull(Random);

            // Act
            var containsRange = collection.ContainsRange(subsetWithNull);

            // Assert
            Assert.That(containsRange, Is.True);
        }

        [Test]
        public void ContainsRange_AllowsNullNewNull_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);
            var count = GetCount(Random);
            var subsetWithNull =  collection.ShuffledCopy(Random).Take(count).WithNull(Random);

            // Act
            var containsRange = collection.ContainsRange(subsetWithNull);

            // Assert
            Assert.That(containsRange, Is.False);
        }

        [Test]
        public void ContainsRange_EmptyCollectionEmptyEnumerable_True()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = NoStrings;

            // Act
            var containsRange = collection.ContainsRange(items);

            // Assert
            Assert.That(containsRange, Is.True);
        }

        [Test]
        public void ContainsRange_EmptyCollectionNonEmptyEnumerable_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act
            var containsRange = collection.ContainsRange(items);

            // Assert
            Assert.That(containsRange, Is.False);
        }

        [Test]
        public void ContainsRange_LargerRangeThanCollection_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var items = collection.Append(collection.Choose()).ShuffledCopy(Random);

            // Act
            var containsRange = collection.ContainsRange(items);

            // Assert
            Assert.That(containsRange, Is.False);
        }

        [Test]
        public void ContainsRange_RandomCollectionEmptyEnumerable_True()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var items = NoStrings;

            // Act
            var containsRange = collection.ContainsRange(items);

            // Assert
            Assert.That(containsRange, Is.True);
        }

        [Test]
        public void ContainsRange_Subset_True()
        {
            // Arrange
            var collection = GetStringCollection(Random, ReferenceEqualityComparer);
            var count = GetCount(Random) / 2;
            var containedItems = collection.ShuffledCopy(Random).Take(count);

            // Act
            var containsRange = collection.ContainsRange(containedItems);

            // Assert
            Assert.That(containsRange, Is.True);
        }

        [Test]
        public void ContainsRange_SubsetWithDuplicates_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, ReferenceEqualityComparer);
            var count = GetCount(Random) / 2;
            var containedItems = collection.ShuffledCopy(Random).Take(count).ToArray();
            var subsetWithDuplicate = containedItems.Append(containedItems.Choose(Random));

            // Act
            var containsRange = collection.ContainsRange(subsetWithDuplicate);

            // Assert
            Assert.That(containsRange, Is.False);
        }

        [Test]
        public void ContainsRange_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer, allowsNull: true);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act & Assert
            Assert.That(() => collection.ContainsRange(badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(items).ByReference<string>());
        }

        #endregion

        #region CountDuplicates(T)

        [Test]
        public void CountDuplicates_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.CountDuplicates(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void CountDuplicates_AllowsNullNewNull_Count()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);

            // Act
            var countDuplicates = collection.CountDuplicates(null);

            // Assert
            Assert.That(countDuplicates, Is.Zero);
        }

        [Test]
        public void CountDuplicates_AllowsNullExistingNull_Count()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            var count = collection.Add(null) ? 2 : 1;

            // Act
            var countDuplicates = collection.CountDuplicates(null);

            // Assert
            Assert.That(countDuplicates, Is.EqualTo(count));
        }

        [Test]
        public void CountDuplicates_EmptyCollection_Zero()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);

            // Act
            var countDuplicates = collection.CountDuplicates(item);

            // Assert
            Assert.That(countDuplicates, Is.Zero);
        }

        [Test]
        public void CountDuplicates_SingleItemCollection_Count()
        {
            // Arrange
            var item = GetString(Random);
            var count = AllowsDuplicates ? GetCount(Random) : 1;
            var items = TestHelper.Repeat(() => string.Copy(item), count);
            var collection = GetCollection(items);

            // Act
            var countDuplicates = collection.CountDuplicates(item);

            // Assert
            Assert.That(countDuplicates, Is.EqualTo(count));
        }

        [Test]
        public void CountDuplicates_RandomCollectionWithEqualItems_Count()
        {
            // Arrange
            var item = GetLowercaseString(Random);
            var count = AllowsDuplicates ? GetCount(Random) : 1;
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);

            // Act
            var countDuplicates = collection.CountDuplicates(item);

            // Assert
            Assert.That(countDuplicates, Is.EqualTo(count));
        }

        // TODO: Keep?
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
        [Category("Unfinished")]
        public void CountDuplicates_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Find(ref T)

        [Test]
        public void Find_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            string item = null;

            // Act & Assert
            Assert.That(() => collection.Find(ref item), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Find_AllowsNullExistingNull_True()
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
        public void Find_AllowsNullNewNull_False()
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
            var item = GetString(Random);
            var refItem = item;

            // Act
            var find = collection.Find(ref refItem);

            // Assert
            Assert.That(find, Is.False);
            Assert.That(refItem, Is.SameAs(item));
        }

        [Test]
        public void Find_RandomCollectionExistingItem_True()
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
        public void Find_RandomCollectionNewItem_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));
            var refItem = item;

            // Act
            var find = collection.Find(ref refItem);

            // Assert
            Assert.That(find, Is.False);
            Assert.That(refItem, Is.SameAs(item));
        }

        [Test]
        public void Find_ValueTypeCollectionNewItem_False()
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
        public void Find_ValueTypeCollectionExistingItem_True()
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
        public void Find_RandomCollectionNewItem_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = GetString(Random);

            // Act & Assert
            Assert.That(() => collection.Find(ref item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void Find_RandomCollectionExistingItem_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.Choose(Random);

            // Act & Assert
            Assert.That(() => collection.Find(ref item), RaisesNoEventsFor(collection));
        }

        [Test]
        [Category("Unfinished")]
        public void Find_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        // TODO

        #endregion

        #region FindDuplicates(T)

        [Test]
        public void FindDuplicates_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.FindDuplicates(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void FindDuplicates_AllowsNullExistingNull_Nulls()
        {
            // Arrange
            var count = AllowsDuplicates ? GetCount(Random) : 1;
            var items = GetStrings(Random).WithRepeatedItem(() => null, count, Random);
            var collection = GetCollection(items, allowsNull: true);
            var expected = new ExpectedCollectionValue<string>(
                ((string) null).Repeat(count),
                collection.EqualityComparer,
                collection.AllowsNull
                //() => null
            );

            // Act
            var findDuplicates = collection.FindDuplicates(null);

            // Assert
            Assert.That(findDuplicates, Is.EqualTo(expected));
        }

        [Test]
        public void FindDuplicates_AllowsNullNewNull_IsEmpty()
        {
            // Arrange
            var count = GetCount(Random);
            var items = GetStrings(Random).WithRepeatedItem(() => null, count, Random);
            var collection = GetCollection(items, allowsNull: true);
            var expected = new ExpectedCollectionValue<string>(
                ((string) null).Repeat(AllowsDuplicates ? count : 1),
                collection.EqualityComparer,
                collection.AllowsNull
                //() => null
                );

            // Act
            var findDuplicates = collection.FindDuplicates(null);

            // Assert
            Assert.That(findDuplicates, Is.EqualTo(expected));
        }

        [Test]
        public void FindDuplicates_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var expected = new ExpectedCollectionValue<string>(
                NoStrings,
                collection.EqualityComparer,
                collection.AllowsNull
                );
            var item = GetString(Random);

            // Act
            var findDuplicates = collection.FindDuplicates(item);

            // Assert
            Assert.That(findDuplicates, Is.EqualTo(expected));
        }

        [Test]
        public void FindDuplicates_SingleItemCollection_WholeCollection()
        {
            // Arrange
            var item = GetString(Random);
            var count = AllowsDuplicates ? GetCount(Random) : 1;
            var items = ((Func<string>) (() => string.Copy(item))).Repeat(count);
            var collection = GetCollection(items);
            var expected = new ExpectedCollectionValue<string>(
                items,
                collection.EqualityComparer,
                collection.AllowsNull
                );

            // Act
            var findDuplicates = collection.FindDuplicates(item);

            // Assert
            Assert.That(findDuplicates, Is.EqualTo(expected));
        }

        [Test]
        public void FindDuplicates_RandomCollectionWithDuplicateItems_RepeatedItem()
        {
            // Arrange
            var item = GetLowercaseString(Random);
            var count = AllowsDuplicates ? GetCount(Random) : 1;
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);
            var expected = new ExpectedCollectionValue<string>(
                item.Repeat(count),
                collection.EqualityComparer,
                collection.AllowsNull
                );

            // Act
            var findDuplicates = collection.FindDuplicates(item);

            // Assert
            Assert.That(findDuplicates, Is.EqualTo(expected));
        }

        [Test]
        public void FindDuplicates_AlterDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var item = GetLowercaseString(Random);
            var count = GetCount(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);
            var enumerable = collection.FindDuplicates(item);

            // Act & Assert
            Assert.That(() => { while (!collection.Add(GetLowercaseString(Random))) {}}, Breaks.EnumeratorFor(enumerable));
        }

        [Test]
        public void FindDuplicates_ChangeCollectionInvalidatesCollectionValue_ThrowsInvalidOperationException()
        {
            // Arrange
            var item = GetString(Random);
            var count = GetCount(Random);
            var items = GetStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);
            var array = new string[collection.Count];
            var stringBuilder = new StringBuilder();
            var rest = 0;

            // Act
            var findDuplicates = collection.FindDuplicates(item);
            collection.UpdateOrAdd(GetString(Random));

            // TODO: Refactor into separate CollectionValueConstraint
            // Assert
            Assert.That(() => findDuplicates.AllowsNull, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.Count, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.CountSpeed, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.IsEmpty, Throws.InvalidOperationException.Because(CollectionWasModified));

            Assert.That(() => findDuplicates.Choose(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.CopyTo(array, 0), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.GetEnumerator().MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.ToArray(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.Show(stringBuilder, ref rest, null), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.ToString(null, null), Throws.InvalidOperationException.Because(CollectionWasModified));

            Assert.That(() => findDuplicates.Equals(null), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.GetHashCode(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => findDuplicates.ToString(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void FindDuplicates_DuplicatesByCounting_Fail()
        {
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void FindDuplicates_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

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
            var item = GetString(Random);
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
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);
            var expectedEvents = new[] {
                Added(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.FindOrAdd(ref item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void FindOrAdd_RandomCollectionFind_True()
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
        public void FindOrAdd_RandomCollectionAdd_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));
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
            var collection = GetStringCollection(Random);
            var item = collection.Choose(Random);

            // Act & Assert
            Assert.That(() => collection.FindOrAdd(ref item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void FindOrAdd_AddDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetCollection(items);

            // Act & Assert
            Assert.That(() => collection.FindOrAdd(ref item), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void FindOrAdd_FindIDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var item = items.Choose(Random).ToLower();

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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void FindOrAdd_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
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
            collection.AddRange(newItems);
            collection.RemoveRange(newItems);
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
            var expected = GetCollection(items, ReferenceEqualityComparer).GetUnsequencedHashCode();

            // Act
            var hashCode = sequence.GetUnsequencedHashCode();
            sequence.Clear();
            sequence.AddRange(items);
            hashCode = sequence.GetUnsequencedHashCode();

            // Assert
            Assert.That(hashCode, Is.EqualTo(expected));
        }

        // TODO: Test for shuffled list in IListTests

        #endregion

        #region ItemMultiplicities()

        // TODO: Implement once HashBag<T> is introduced!
        [Test]
        [Ignore("Not implemented yet!")]
        public void ItemMultiplicities_RandomCollection_NotImplemented()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act
            var itemMultiplicities = collection.ItemMultiplicities();

            // Assert
            Assert.That(itemMultiplicities, Is.Not.Empty);
        }

        #endregion

        #region Remove(T)

        [Test]
        public void Remove_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.Remove(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Remove_AllowsNullExistingNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            var expected = collection.Where(item => item != null).ToArray();

            // Act
            var remove = collection.Remove(null);

            // Assert
            Assert.That(remove, Is.True);
            Assert.That(collection, Is.EquivalentTo(expected).ByReference<string>());
        }

        [Test]
        public void Remove_AllowsNullNewNull_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);
            var expected = collection.ToArray();

            // Act
            var remove = collection.Remove(null);

            // Assert
            Assert.That(remove, Is.False);
            Assert.That(collection, Is.EqualTo(expected).ByReference<string>());
        }

        [Test]
        public void Remove_RandomCollectionExistingItem_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var existingItem = collection.Choose(Random);
            var item = existingItem.ToLower();
            var expectedEvents = new[] {
                Removed(existingItem, 1, collection),
                Changed(collection),
            };
            var remove = false;

            // Act & Assert
            Assert.That(() => remove = collection.Remove(item), Raises(expectedEvents).For(collection));
            Assert.That(remove, Is.True);
        }

        [Test]
        public void Remove_RandomCollectionNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);

            // Act & Assert
            Assert.That(() => collection.Remove(item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void Remove_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);

            // Act
            var remove = collection.Remove(item);

            // Assert
            Assert.That(remove, Is.False);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Remove_RandomCollectionExistingItem_True()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var item = collection.Choose(Random).ToLower();

            // Act
            var remove = collection.Remove(item);

            // Assert
            Assert.That(remove, Is.True);
        }

        [Test]
        public void Remove_RandomCollectionNewItem_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));
            var expected = collection.ToArray();

            // Act
            var remove = collection.Remove(item);

            // Assert
            Assert.That(remove, Is.False);
            Assert.That(collection, Is.EqualTo(expected).ByReference<string>());
        }

        [Test]
        public void Remove_RemoveExistingDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var item = collection.Choose(Random).ToLower();

            // Act & Assert
            Assert.That(() => collection.Remove(item), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void Remove_RemoveNewDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Remove(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        [Category("Unfinished")]
        public void Remove_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Remove_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Remove_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Remove(T, out T)

        [Test]
        public void RemoveOut_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            string removedItem;

            // Act & Assert
            Assert.That(() => collection.Remove(null, out removedItem), Violates.PreconditionSaying(ItemMustBeNonNull));
        }
        
        [Test]
        public void RemoveOut_AllowsNullExistingNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            var expected = collection.Where(item => item != null).ToArray();
            string removedItem;

            // Act
            var remove = collection.Remove(null, out removedItem);

            // Assert
            Assert.That(remove, Is.True);
            Assert.That(collection, Is.EquivalentTo(expected).ByReference<string>());
        }

        [Test]
        public void RemoveOut_AllowsNullNewNull_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);
            var expected = collection.ToArray();
            string removedItem;

            // Act
            var remove = collection.Remove(null, out removedItem);

            // Assert
            Assert.That(remove, Is.False);
            Assert.That(collection, Is.EqualTo(expected).ByReference<string>());
        }

        [Test]
        public void RemoveOut_RandomCollectionExistingItem_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var existingItem = collection.Choose(Random);
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
        public void RemoveOut_RandomCollectionNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);
            string removedItem;

            // Act & Assert
            Assert.That(() => collection.Remove(item, out removedItem), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveOut_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);
            string removedItem;

            // Act
            var remove = collection.Remove(item, out removedItem);

            // Assert
            Assert.That(remove, Is.False);
            Assert.That(removedItem, Is.Null);
        }

        [Test]
        public void RemoveOut_RandomCollectionExistingItem_True()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var existingItem = collection.Choose(Random);
            var item = existingItem.ToLower();
            string removedItem;

            // Act
            var remove = collection.Remove(item, out removedItem);

            // Assert
            Assert.That(remove, Is.True);
            Assert.That(removedItem, Is.SameAs(existingItem));
        }

        [Test]
        public void RemoveOut_RandomCollectionNewItem_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));
            var expected = collection.ToArray();
            string removedItem;

            // Act
            var remove = collection.Remove(item, out removedItem);

            // Assert
            Assert.That(remove, Is.False);
            Assert.That(removedItem, Is.Null);
            Assert.That(collection, Is.EqualTo(expected).ByReference<string>());
        }

        [Test]
        public void RemoveOut_RemoveDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var item = collection.Choose(Random).ToLower();
            string removedItem;

            // Act & Assert
            Assert.That(() => collection.Remove(item, out removedItem), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void RemoveOut_RemoveItemDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));
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
        public void RemoveOut_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveOut_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveOut_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region RemoveRange(IEnumerable<T>)

        [Test]
        public void RemoveRange_NullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveRange(null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void RemoveRange_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            var items = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveRange(items), Violates.PreconditionSaying(ItemsMustBeNonNull));
        }

        [Test]
        public void RemoveRange_AllowsNullExistingNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            var removeItems = GetStrings(Random).WithNull(Random);

            // Act
            var removeRange = collection.RemoveRange(removeItems);

            // Assert
            Assert.That(removeRange, Is.True);
        }

        [Test]
        public void RemoveRange_AllowsNullNewNull_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, allowsNull: true);
            var removeItems = GetLowercaseStrings(Random).WithNull(Random);

            // Act
            var removeRange = collection.RemoveRange(removeItems);

            // Assert
            Assert.That(removeRange, Is.False);
        }

        [Test]
        public void RemoveRange_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveRange(items), RaisesNoEventsFor(collection).And.False);
        }
        
        [Test]
        public void RemoveRange_EmptyEnumerable_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var items = NoStrings;

            // Act & Assert
            Assert.That(() => collection.RemoveRange(items), RaisesNoEventsFor(collection).And.False);
        }
        
        [Test]
        public void RemoveRange_BothEmpty_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = NoStrings;

            // Act & Assert
            Assert.That(() => collection.RemoveRange(items), RaisesNoEventsFor(collection).And.False);
        }
        
        [Test]
        public void RemoveRange_NewItems_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var newItems = GetLowercaseStrings(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveRange(newItems), RaisesNoEventsFor(collection).And.False);
        }

        [Test]
        public void RemoveRange_RemoveCollectionItself_True()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);

            // Act
            var removeRange = collection.RemoveRange(items);

            // Assert
            Assert.That(removeRange, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void RemoveRange_RemoveDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);
            var count = GetCount(Random);
            var existingItems = items.Take(count).ShuffledCopy(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveRange(existingItems), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void RemoveRange_RemoveNewItemsDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var newItems = GetLowercaseStrings(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RemoveRange(newItems);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void RemoveRange_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer, allowsNull: true);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act & Assert
            Assert.That(() => collection.RemoveRange(badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(items).ByReference<string>());
        }

        [Test]
        public void RemoveRange_RemoveOneOfEachDuplicate_AllButOneLeft()
        {
            // Arrange
            var items = GetStrings(Random);
            var repeatedItems = items.SelectMany(item => item.Repeat(Random.Next(2, 5))).ToArray();
            var collection = GetCollection(repeatedItems);
            var itemCounts = repeatedItems.GroupBy(item => item).Select(grouping => new KeyValuePair<string, int>(grouping.Key, grouping.Count() - 1));

            // Act
            var removeRange = collection.RemoveRange(items);

            // Assert
            Assert.That(removeRange, Is.True);
            Assert.That(collection.GroupBy(item => item).Select(grouping => new KeyValuePair<string, int>(grouping.Key, grouping.Count())), Is.EquivalentTo(itemCounts));
        }

        [Test]
        public void RemoveRange_RemoveOverlap_OverlapRemoved()
        {
            // Arrange
            var remainingItems = GetStrings(Random);
            var overlappingItems = GetStrings(Random);
            var items = remainingItems.Concat(overlappingItems).ShuffledCopy(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var itemsToRemove = GetStrings(Random).Concat(overlappingItems).ShuffledCopy(Random);

            // Act
            var removeRange = collection.RemoveRange(itemsToRemove);

            // Assert
            Assert.That(removeRange, Is.True);
            Assert.That(collection, Is.EquivalentTo(remainingItems));
        }

        [Test]
        [Ignore("Figure out the best way to assess events")]
        public void RemoveRange_RemoveOverlap_RaisesExpectedEvents()
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
            Assert.That(() => collection.RemoveRange(itemsToRemove), Raises(expectedEvents).InNoParticularOrder().For(collection)); // TODO: Ignore order
        }

        // TODO: Remove subset
        // TODO: Raises events

        [Test]
        [Category("Unfinished")]
        public void RemoveRange_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveRange_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveRange_Set_Fail()
        {
            // TODO: See RemoveRange_RemoveOneOfEachDuplicate_AllButOneLeft()
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }


        // TODO: Look at AddRange for inspiration

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
        public void RemoveDuplicates_AllowsNullExistingNulls_True()
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

        [Test]
        public void RemoveDuplicates_AllowsNullNewNull_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);
            var array = collection.ToArray();

            // Act
            var removeDuplicates = collection.RemoveDuplicates(null);

            // Assert
            Assert.That(removeDuplicates, Is.False);
            Assert.That(collection, Is.EqualTo(array).ByReference<string>());
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
        public void RemoveDuplicates_RandomCollectionNewItem_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));

            // Act & Assert
            Assert.That(() => collection.RemoveDuplicates(item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RemoveDuplicates_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);

            // Act
            var removeDuplicates = collection.RemoveDuplicates(item);

            // Assert
            Assert.That(removeDuplicates, Is.False);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void RemoveDuplicates_RemoveExistingDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.Choose(Random);

            // Act & Assert
            Assert.That(() => collection.RemoveDuplicates(item), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void RemoveDuplicates_RemoveNewDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RemoveDuplicates(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void RemoveDuplicates_DuplicateItems_Empty()
        {
            // Arrange
            var count = GetCount(Random);
            var item = GetString(Random);
            var items = item.Repeat(count);
            var collection = GetCollection(items);

            // Act
            var removeDuplicates = collection.RemoveDuplicates(item);

            // Assert
            Assert.That(removeDuplicates, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void RemoveDuplicates_RandomCollectionNewItem_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));
            var array = collection.ToArray();

            // Act
            var removeDuplicates = collection.RemoveDuplicates(item);

            // Assert
            Assert.That(removeDuplicates, Is.False);
            Assert.That(collection, Is.EqualTo(array).ByReference<string>());
        }

        [Test]
        public void RemoveDuplicates_RandomCollectionExistingItem_True()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.Choose(Random);
            var array = collection.Where(x => !x.Equals(item)).ToArray();

            // Act
            var removeDuplicates = collection.RemoveDuplicates(item);

            // Assert
            Assert.That(removeDuplicates, Is.True);
            Assert.That(collection, Is.EquivalentTo(array).ByReference<string>());
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveDuplicates_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveDuplicates_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region RetrieveAll(IEnumerable<T>)

        [Test]
        public void RetainRange_NullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.RetainRange(null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void RetainRange_DisallowsNullsInEnumerable_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            var items = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => collection.RetainRange(items), Violates.PreconditionSaying(ItemsMustBeNonNull));
        }

        [Test]
        public void RetainRange_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act
            var retainRange = collection.RetainRange(items);

            // Assert
            Assert.That(retainRange, Is.False);
        }

        [Test]
        public void RetainRange_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var items = GetStrings(Random);

            // Act & Assert
            Assert.That(() => collection.RetainRange(items), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RetainRange_EmptyEnumerable_Empty()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var items = Enumerable.Empty<string>();

            // Act
            var retainRange = collection.RetainRange(items);

            // Assert
            Assert.That(retainRange, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        [Ignore("Figure out the best way to assess events")]
        public void RetainRange_EmptyEnumerable_RaisesExpectedEvents()
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
            Assert.That(() => collection.RetainRange(itemsToRemove), Raises(expectedEvents).InNoParticularOrder().For(collection));
        }

        [Test]
        public void RetainRange_BothEmpty_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Enumerable.Empty<string>();

            // Act
            var retainRange = collection.RetainRange(item);

            // Assert
            Assert.That(retainRange, Is.False);
        }

        [Test]
        public void RetainRange_BothEmpty_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Enumerable.Empty<string>();

            // Act & Assert
            Assert.That(() => collection.RetainRange(item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void RetainRange_NewItems_Empty()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var newItems = GetLowercaseStrings(Random);

            // Act
            var retainRange = collection.RetainRange(newItems);

            // Assert
            Assert.That(retainRange, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        [Ignore("Figure out the best way to assess events")]
        public void RetainRange_NewItems_RaisesExpectedEvents()
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
            Assert.That(() => collection.RetainRange(newItems), Raises(expectedEvents).InNoParticularOrder().For(collection));
        }

        [Test]
        public void RetainRange_RetainCollectionItself_Unchanged()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);

            // Act
            var retainRange = collection.RetainRange(items);

            // Assert
            Assert.That(retainRange, Is.False);
            Assert.That(collection, Is.EquivalentTo(items));
        }

        // TODO: Event version of above

        [Test]
        public void RetainRange_RetainRangeDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var count = GetCount(Random);
            var items = GetStrings(Random, count);
            var collection = GetCollection(items);
            var existingItems = items.Take(count / 2).ShuffledCopy(Random);

            // Act & Assert
            Assert.That(() => collection.RetainRange(existingItems), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void RetainRange_RetainCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.RetainRange(items);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void RetainRange_BadEnumerable_ThrowsExceptionButCollectionDoesNotChange()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer, allowsNull: true);
            var badEnumerable = GetStrings(Random).AsBadEnumerable();

            // Act & Assert
            Assert.That(() => collection.RetainRange(badEnumerable), Throws.TypeOf<BadEnumerableException>());
            Assert.That(collection, Is.EquivalentTo(items).ByReference<string>());
        }

        [Test]
        public void RetainRange_RetainOverlap_OverlapRetained()
        {
            // Arrange
            var removedItems = GetStrings(Random);
            var overlappingItems = GetStrings(Random);
            var items = removedItems.Concat(overlappingItems).ShuffledCopy(Random);
            var collection = GetCollection(items, ReferenceEqualityComparer);
            var itemsToRetain = GetStrings(Random).Concat(overlappingItems).ShuffledCopy(Random);

            // Act
            var retainRange = collection.RetainRange(itemsToRetain);

            // Assert
            Assert.That(retainRange, Is.True);
            Assert.That(collection, Is.EquivalentTo(overlappingItems));
        }

        [Test]
        [Ignore("Figure out the best way to assess events")]
        public void RetainRange_RetainOverlap_RaisesExpectedEvents()
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
            Assert.That(() => collection.RetainRange(itemsToRetain), Raises(expectedEvents).InNoParticularOrder().For(collection)); // TODO: Ignore order
        }

        // TODO: Retain subset
        // TODO: Raises events

        #endregion

        #region UniqueItems

        [Test]
        public void UniqueItems_RandomCollectionWithNull_ResultContainsNull()
        {
            // Arrange
            var count = GetCount(Random);
            var items = GetStrings(Random).WithNull(Random);
            var duplicateItems = items.ShuffledCopy(Random).Take(count).Concat(items);
            var collection = GetCollection(duplicateItems, allowsNull: true);
            var expected = new ExpectedCollectionValue<string>(
                items,
                collection.EqualityComparer,
                collection.AllowsNull,
                sequenced: false
                );

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.EqualTo(expected));
        }

        [Test]
        public void UniqueItems_EmptyCollection_Empty()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var expected = new ExpectedCollectionValue<string>(
                NoStrings,
                collection.EqualityComparer,
                collection.AllowsNull
                );

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.EqualTo(expected));
        }

        [Test]
        public void UniqueItems_AllUniqueItems_EqualToItself()
        {
            // Arrange
            var collection = GetStringCollection(Random, ReferenceEqualityComparer);
            var expected = new ExpectedCollectionValue<string>(
                collection,
                collection.EqualityComparer,
                collection.AllowsNull,
                sequenced: false
                );

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.EqualTo(expected));
        }

        [Test]
        public void UniqueItems_EqualItems_OneItem()
        {
            // Arrange
            var count = GetCount(Random);
            var item = GetString(Random);
            var items = item.Repeat(count);
            var collection = GetCollection(items);
            var expected = new ExpectedCollectionValue<string>(
                new[] { item },
                collection.EqualityComparer,
                collection.AllowsNull,
                sequenced: false
                );

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.EqualTo(expected));
        }

        [Test]
        public void UniqueItems_RepeatedItems_OnlyUniqueItems()
        {
            // Arrange
            var originalItems = GetStrings(Random);
            var items = originalItems.SelectMany(item => item.Repeat(Random.Next(1, 4)));
            var collection = GetCollection(items);
            var expected = new ExpectedCollectionValue<string>(
                originalItems,
                collection.EqualityComparer,
                collection.AllowsNull,
                sequenced: false
                );

            // Act
            var uniqueItems = collection.UniqueItems();

            // Assert
            Assert.That(uniqueItems, Is.EqualTo(expected));
        }

        [Test]
        public void UniqueItems_RandomCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act & Assert
            Assert.That(() => collection.UniqueItems(), RaisesNoEventsFor(collection));
        }

        [Test]
        public void UniqueItems_UniqueItemsDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetStringCollection(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.UniqueItems();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void UniqueItems_ChangeCollectionInvalidatesCollectionValue_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var array = new string[collection.Count];
            var stringBuilder = new StringBuilder();
            var rest = 0;

            // Act
            var uniqueItems = collection.UniqueItems();
            collection.UpdateOrAdd(GetString(Random));

            // TODO: Refactor into separate CollectionValueConstraint
            // Assert
            Assert.That(() => uniqueItems.AllowsNull, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.Count, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.CountSpeed, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.IsEmpty, Throws.InvalidOperationException.Because(CollectionWasModified));

            Assert.That(() => uniqueItems.Choose(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.CopyTo(array, 0), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.GetEnumerator().MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.ToArray(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.Show(stringBuilder, ref rest, null), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.ToString(null, null), Throws.InvalidOperationException.Because(CollectionWasModified));

            Assert.That(() => uniqueItems.Equals(null), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.GetHashCode(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => uniqueItems.ToString(), Throws.InvalidOperationException.Because(CollectionWasModified));
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
            var collection = GetCollection(GetUppercaseStrings(Random));
            var otherCollection = GetCollection(GetLowercaseStrings(Random));

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
        public void UnsequencedEquals_OneCollectionCreatedFromAnother_True()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var otherCollection = GetCollection((SCG.IEnumerable<string>) collection);

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
            var otherItems = items.SelectMany(item => item.Repeat(Random.Next(2, 4)));
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
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Update(T)

        // TODO: Test that the proper item is replaced when several exist

        [Test]
        public void Update_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.Update(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void Update_AllowsNullNewNull_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);

            // Act
            var update = collection.Update(null);

            // Assert
            Assert.That(update, Is.False);
        }

        [Test]
        public void Update_AllowsNullExistingNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);

            // Act
            var update = collection.Update(null);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void Update_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);

            // Act
            var update = collection.Update(item);

            // Assert
            Assert.That(update, Is.False);
        }

        [Test]
        public void Update_RandomCollectionNewItem_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));

            // Act
            var update = collection.Update(item);

            // Assert
            Assert.That(update, Is.False);
        }

        [Test]
        public void Update_RandomCollectionExistingItem_True()
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
        public void Update_RandomCollectioneExistingItem_RaisesExpectedEvents()
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
        public void Update_RandomCollectionNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);
            var update = true;

            // Act & Assert
            Assert.That(() => update = collection.Update(item), RaisesNoEventsFor(collection));
            Assert.That(update, Is.False);
        }

        [Test]
        public void Update_UpdateExistingDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var item = collection.Choose(Random).ToLower();

            // Act & Assert
            Assert.That(() => collection.Update(item), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void Update_UpdateNewItemDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = items.Choose(Random).ToLower();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Update(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        // TODO: Use string copy
        [Test]
        public void Update_DuplicateItemCollection_ReplacesOneItem()
        {
            // Arrange
            var item = GetLowercaseString(Random);
            var count = GetCount(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(item, count, Random);
            var collection = GetCollection(items);
            var array = collection.ToArray();

            // Act
            var update = collection.Update(item);

            // Assert
            Assert.That(update, Is.True);
            Assert.That(collection, Is.EqualTo(array).ByReference<string>());
        }

        // TODO: This is rather an IList<T> test...
        [Test]
        public void Update_RandomCollectionFirstItem_Updated()
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

        // TODO: Proper item replaced

        [Test]
        [Category("Unfinished")]
        public void Update_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Update_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Update_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
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
        public void UpdateOut_AllowsNullNewNull_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);
            string oldItem;

            // Act
            var update = collection.Update(null, out oldItem);

            // Assert
            Assert.That(update, Is.False);
            Assert.That(oldItem, Is.Null);
        }

        [Test]
        public void UpdateOut_AllowsNullExistingNull_True()
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
        public void UpdateOut_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);
            string oldItem;

            // Act
            var update = collection.Update(item, out oldItem);

            // Assert
            Assert.That(update, Is.False);
            Assert.That(oldItem, Is.Null);
        }

        [Test]
        public void UpdateOut_RandomCollectionNewItem_False()
        {
            // Arrange
            var collection = GetStringCollection(Random);
            var item = collection.DifferentItem(() => GetString(Random));
            string oldItem;

            // Act
            var update = collection.Update(item, out oldItem);

            // Assert
            Assert.That(update, Is.False);
            Assert.That(oldItem, Is.Null);
        }

        [Test]
        public void UpdateOut_RandomCollectionExistingItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var existingItem = items.Choose(Random);
            var item = existingItem.ToLower();
            string oldItem;

            // Act
            var update = collection.Update(item, out oldItem);

            // Assert
            Assert.That(update, Is.True);
            Assert.That(oldItem, Is.SameAs(existingItem));
        }

        [Test]
        public void UpdateOut_RandomCollectionExistingItem_RaisesExpectedEvents()
        {
            // Arrange
            var count = GetCount(Random);
            var item = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(item, count, Random);
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
        public void UpdateOut_RandomCollectionNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);
            var update = true;
            string oldItem;

            // Act & Assert
            Assert.That(() => update = collection.Update(item, out oldItem), RaisesNoEventsFor(collection));
            Assert.That(update, Is.False);
        }

        [Test]
        public void UpdateOut_UpdateExistingDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var item = collection.Choose(Random).ToLower();
            string oldItem;

            // Act & Assert
            Assert.That(() => collection.Update(item, out oldItem), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void UpdateOut_UpdateNewDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = items.Choose(Random).ToLower();
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOut_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOut_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region UpdateOrAdd(T)

        // TODO: Test that the proper item is replaced when several exist

        [Test]
        public void UpdateOrAdd_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(null), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void UpdateOrAdd_AllowsNullAddNull_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);

            // Act
            var update = collection.UpdateOrAdd(null);

            // Assert
            Assert.That(update, Is.False);
        }

        [Test]
        public void UpdateOrAdd_AllowsNullUpdateNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);

            // Act
            var update = collection.UpdateOrAdd(null);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void UpdateOrAdd_EmptyCollection_Added()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);
            var itemArray = new[] { item };

            // Act
            var update = collection.UpdateOrAdd(item);

            // Assert
            Assert.That(update, Is.False);
            Assert.That(collection, Is.EqualTo(itemArray));
        }

        [Test]
        public void UpdateOrAdd_RandomCollectionAddItem_Added()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);

            // Act
            var update = collection.UpdateOrAdd(item);

            // Assert
            Assert.That(update, Is.False);
        }

        [Test]
        public void UpdateOrAdd_RandomCollectionUpdateItem_Updated()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var item = items.Choose(Random).ToLower();

            // Act
            var update = collection.UpdateOrAdd(item);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void UpdateOrAdd_RandomCollectionUpdateItem_RaisesExpectedEvents()
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
            Assert.That(() => collection.UpdateOrAdd(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void UpdateOrAdd_RandomCollectionAddItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);
            var expectedEvents = new[] {
                Added(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void UpdateOrAdd_DuplicateItemCollection_ReplacesOneItem()
        {
            // TODO
            // Arrange
            var count = GetCount(Random);
            var item = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);

            // Act
            var update = collection.UpdateOrAdd(item);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void UpdateOrAdd_RandomCollectionUpdateFirstItem_Update()
        {
            // Arrange
            var item = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random);
            items[0] = item;
            var collection = GetCollection(items);

            // Act
            var update = collection.UpdateOrAdd(item);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void UpdateOrAdd_UpdateDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var item = collection.Choose(Random).ToLower();

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(item), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void UpdateOrAdd_AddNewDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = items.Choose(Random).ToLower();

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(item), Breaks.EnumeratorFor(collection));
        }
        
        // TODO: Proper item replaced

        [Test]
        [Category("Unfinished")]
        public void UpdateOrAdd_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOrAdd_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOrAdd_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region UpdateOrAdd(T, out T)

        // TODO: Test that the proper item is replaced when several exist

        [Test]
        public void UpdateOrAddOut_DisallowsNullUpdateOrAddNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: false);
            string oldItem;

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(null, out oldItem), Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void UpdateOrAddOut_AllowsNullAddNull_False()
        {
            // Arrange
            var collection = GetStringCollection(Random, allowsNull: true);
            string oldItem;

            // Act
            var update = collection.UpdateOrAdd(null, out oldItem);

            // Assert
            Assert.That(update, Is.False);
        }

        [Test]
        public void UpdateOrAddOut_AllowsNullUpdateNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetCollection(items, allowsNull: true);
            string oldItem;

            // Act
            var update = collection.UpdateOrAdd(null, out oldItem);

            // Assert
            Assert.That(update, Is.True);
        }

        [Test]
        public void UpdateOrAddOut_EmptyCollection_Added()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = GetString(Random);
            var itemArray = new[] { item };
            string oldItem;

            // Act
            var update = collection.UpdateOrAdd(item, out oldItem);

            // Assert
            Assert.That(update, Is.False);
            Assert.That(oldItem, Is.Null);
            Assert.That(collection, Is.EqualTo(itemArray));
        }

        [Test]
        public void UpdateOrAddOut_RandomCollectionAddItem_Added()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);
            string oldItem;

            // Act
            var update = collection.UpdateOrAdd(item, out oldItem);

            // Assert
            Assert.That(update, Is.False);
            Assert.That(oldItem, Is.Null);
        }

        [Test]
        public void UpdateOrAddOut_RandomCollectionUpdateItem_Updated()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var existingItem = items.Choose(Random);
            var item = existingItem.ToLower();
            string oldItem;

            // Act
            var update = collection.UpdateOrAdd(item, out oldItem);

            // Assert
            Assert.That(update, Is.True);
            Assert.That(oldItem, Is.SameAs(existingItem));
        }

        [Test]
        public void UpdateOrAddOut_RandomCollectionUpdateItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items, CaseInsensitiveStringComparer.Default);
            var existingItem = items.Choose(Random);
            var item = existingItem.ToLower();
            string oldItem;
            var expectedEvents = new[] {
                Removed(existingItem, 1, collection),
                Added(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(item, out oldItem), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void UpdateOrAddOut_RandomCollectionAddItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = GetLowercaseString(Random);
            string oldItem;
            var expectedEvents = new[] {
                Added(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(item, out oldItem), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void UpdateOrAddOut_DuplicateItemCollection_ReplacesOneItem()
        {
            // TODO
            // Arrange
            var count = GetCount(Random);
            var item = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random).WithRepeatedItem(() => item, count, Random);
            var collection = GetCollection(items);
            string oldItem;

            // Act
            var update = collection.UpdateOrAdd(item, out oldItem);

            // Assert
            Assert.That(update, Is.True);
            Assert.That(oldItem, Is.SameAs(item));
        }

        [Test]
        public void UpdateOrAddOut_RandomCollectionUpdateFirstItem_Update()
        {
            // Arrange
            var existingItem = GetLowercaseString(Random);
            var items = GetUppercaseStrings(Random);
            items[0] = existingItem;
            var item = string.Copy(existingItem);
            var collection = GetCollection(items);
            string oldItem;

            // Act
            var update = collection.UpdateOrAdd(item, out oldItem);

            // Assert
            Assert.That(update, Is.True);
            Assert.That(oldItem, Is.SameAs(existingItem));
        }

        [Test]
        public void UpdateOrAddOut_UpdateDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var collection = GetStringCollection(Random, CaseInsensitiveStringComparer.Default);
            var item = collection.Choose(Random).ToLower();
            string oldItem;

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(item, out oldItem), Breaks.EnumeratorFor(collection));
        }

        [Test]
        public void UpdateOrAddOut_AddDuringEnumeration_BreaksEnumerator()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetCollection(items);
            var item = items.Choose(Random).ToLower();
            string oldItem;

            // Act & Assert
            Assert.That(() => collection.UpdateOrAdd(item, out oldItem), Breaks.EnumeratorFor(collection));
        }
        
        // TODO: Proper item replaced

        [Test]
        [Category("Unfinished")]
        public void UpdateOrAddOut_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOrAddOut_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void UpdateOrAddOut_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion
    }
}