// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using C6.Contracts;
using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.Collections.ExceptionMessages;
using static C6.Tests.Helpers.CollectionEvent;
using static C6.Tests.Helpers.TestHelper;

using SC = System.Collections;
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

        #region SyncRoot

        // TODO: Test?

        #endregion

        #endregion

        #region Methods

        #region CopyTo(Array, int)

        [Test]
        public void SCICollectionCopyTo_InvalidType_ThrowsArgumentException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = new int[collection.Count];

            // Act & Assert
            Assert.That(() => ((SC.ICollection) collection).CopyTo(array, 0), Throws.ArgumentException.Because("Target array type is not compatible with the type of items in the collection."));
        }

        [Test]
        public void SCICollectionCopyTo_InvalidDimension_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var count = collection.Count;
            var array = new string[count, count];

            // Act & Assert
            Assert.That(() => ((SC.ICollection) collection).CopyTo(array, 0), Violates.Precondition);
        }

        [Test]
        public void SCICollectionCopyTo_NullArray_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act & Assert
            Assert.That(() => ((SC.ICollection) collection).CopyTo(null, 0), Violates.Precondition);
        }

        [Test]
        public void SCICollectionCopyTo_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = new string[collection.Count];

            // Act & Assert
            Assert.That(() => ((SC.ICollection) collection).CopyTo(array, -1), Violates.Precondition);
        }

        [Test]
        public void SCICollectionCopyTo_IndexOutOfBound_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = new string[collection.Count];
            var index = Random.Next(1, collection.Count);

            // Act & Assert
            Assert.That(() => ((SC.ICollection) collection).CopyTo(array, index), Violates.Precondition);
        }

        [Test]
        public void SCICollectionCopyTo_EqualSizeArray_Equals()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = new string[collection.Count];

            // Act
            ((SC.ICollection) collection).CopyTo(array, 0);

            // Assert
            Assert.That(array, Is.EqualTo(collection).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCICollectionCopyTo_CopyToRandomIndex_SectionEquals()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = GetStrings(Random, (int) (collection.Count * 1.7));
            var arrayIndex = Random.Next(0, array.Length - collection.Count);

            // Act
            ((SC.ICollection) collection).CopyTo(array, arrayIndex);
            var section = array.Skip(arrayIndex).Take(collection.Count);

            // Assert
            Assert.That(section, Is.EqualTo(collection).Using(ReferenceEqualityComparer));
        }

        #endregion

        #endregion

        #endregion

        #region SC.IList

        #region this[int]

        [Test]
        public void SCIListItemGet_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(int.MinValue, 0);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index], Violates.Precondition);
        }

        [Test]
        public void SCIListItemGet_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = collection.Count;

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index], Violates.Precondition);
        }

        [Test]
        public void SCIListItemGet_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var count = collection.Count;
            var index = Random.Next(count + 1, int.MaxValue);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index], Violates.Precondition);
        }

        [Test]
        public void SCIListItemGet_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyIndexed<string>();

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[0], Violates.Precondition);
        }

        [Test]
        public void SCIListItemGet_RandomCollectionWithNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetList(items, allowsNull: true);
            var index = collection.ToArray().IndexOf(null);

            // Act
            var item = ((SC.IList) collection)[index];

            // Act & Assert
            Assert.That(item, Is.Null);
        }

        [Test]
        public void SCIListItemGet_RandomCollectionIndexZero_FirstItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var first = collection.First();

            // Act
            var item = ((SC.IList) collection)[0];

            // Assert
            Assert.That(item, Is.SameAs(first));
        }

        [Test]
        public void SCIListItemGet_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var last = collection.Last();
            var count = collection.Count;

            // Act
            var item = ((SC.IList) collection)[count - 1];

            // Assert
            Assert.That(item, Is.SameAs(last));
        }

        [Test]
        public void SCIListItemGet_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = collection.ToArray();
            var index = Random.Next(0, array.Length);

            // Act
            var item = ((SC.IList) collection)[index];

            // Assert
            Assert.That(item, Is.SameAs(array[index]));
        }

        [Test]
        public void SCIListItemSet_InvalidType_ThrowsArgumentException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random);
            object item = Random.Next();
            var typeString = "System.String";
            var parameterName = "value";
            var exceptionMessage = $"The value \"{item}\" is not of type \"{typeString}\" and cannot be used in this generic collection.{Environment.NewLine}Parameter name: {parameterName}";

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index] = item, Throws.ArgumentException.Because(exceptionMessage));
        }

        [Test]
        public void SCIListItemSet_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(int.MinValue, 0);
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index] = item, Violates.Precondition);
        }

        [Test]
        public void SCIListItemSet_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = collection.Count;
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index] = item, Violates.Precondition);
        }

        [Test]
        public void SCIListItemSet_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(collection.Count + 1, int.MaxValue);
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index] = item, Violates.Precondition);
        }

        [Test]
        public void SCIListItemSet_EmptyCollection_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var index = 0;
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index] = item, Violates.Precondition);
        }

        [Test]
        public void SCIListItemSet_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);
            var index = GetIndex(collection, Random);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index] = null, Violates.UncaughtPrecondition);
        }

        [Test]
        public void SCIListItemSet_RandomCollectionSetDuplicate_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random);
            var item = collection.ToArray().Choose(Random);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection)[index] = item, Violates.PreconditionSaying(CollectionMustAllowDuplicates));
        }

        [Test]
        public void SCIListItemSet_RandomCollectionSetDuplicate_Inserted()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random);
            var item = collection.ToArray().Choose(Random);

            // Act
            ((SC.IList) collection)[index] = item;

            // Assert
            Assert.That(((SC.IList) collection)[index], Is.SameAs(item));
        }

        [Test]
        public void SCIListItemSet_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var index = GetIndex(collection, Random);
            var array = collection.ToArray();
            array[index] = null;

            // Act
            ((SC.IList) collection)[index] = null;

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListItemSet_RandomCollectionIndexZero_FirstItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = 0;
            var array = collection.ToArray();
            array[index] = item;

            // Act
            ((SC.IList) collection)[index] = item;

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListItemSet_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = collection.Count - 1;
            var array = collection.ToArray();
            array[index] = item;

            // Act
            ((SC.IList) collection)[index] = item;

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListItemSet_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random);
            var array = collection.ToArray();
            array[index] = item;

            // Act
            ((SC.IList) collection)[index] = item;

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListItemSet_RandomCollectionRandomIndex_RaisesExpectedEvents()
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
            Assert.That(() => ((SC.IList) collection)[index] = item, Raises(expectedEvents).For(collection));
        }

        [Test]
        public void SCIListItemSet_SetDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = GetIndex(collection, Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            ((SC.IList) collection)[index] = item;

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListItemSet_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Add(T)

        [Test]
        public void SCIListAdd_InvalidType_ThrowsArgumentException()
        {
            // Arrange
            var collection = GetStringList(Random);
            object item = Random.Next();
            var typeString = "System.String";
            var parameterName = "value";
            var exceptionMessage = $"The value \"{item}\" is not of type \"{typeString}\" and cannot be used in this generic collection.{Environment.NewLine}Parameter name: {parameterName}";

            // Act & Assert
            Assert.That(() => collection.Add(item), Throws.ArgumentException.Because(exceptionMessage));
        }

        [Test]
        public void SCIListAdd_DisallowsNullAddNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).Add(null), Violates.UncaughtPrecondition);
        }

        [Test]
        public void SCIListAdd_AllowsNullAddNull_ReturnsTrue()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var index = collection.Count;

            // Act
            var result = ((SC.IList) collection).Add(null);

            // Assert
            Assert.That(result, Is.EqualTo(index));
        }

        [Test]
        public void SCIListAdd_EmptyCollectionAddItem_CollectionIsSingleItemCollection()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var item = Random.GetString();
            var itemArray = new[] { item };

            // Act
            var result = ((SC.IList) collection).Add(item);

            // Assert
            Assert.That(result, Is.Zero);
            Assert.That(collection, Is.EqualTo(itemArray));
        }

        [Test]
        public void SCIListAdd_AddDuplicateItem_AllowsDuplicates()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetList(items, CaseInsensitiveStringComparer.Default);
            var duplicateItem = items.Choose(Random).ToLower();
            var index = AllowsDuplicates ? collection.Count : -1;

            // Act
            var result = ((SC.IList) collection).Add(duplicateItem);

            // Assert
            Assert.That(result, Is.EqualTo(index));
        }

        // TODO: Add test to IList<T>.Add ensuring that order is the same
        [Test]
        public void SCIListAdd_ManyItems_Equivalent()
        {
            // Arrange
            var referenceEqualityComparer = ComparerFactory.CreateReferenceEqualityComparer<string>();
            var collection = GetEmptyList(referenceEqualityComparer);
            var count = Random.Next(100, 250);
            var items = GetStrings(Random, count);

            // Act
            foreach (var item in items) {
                ((SC.IList) collection).Add(item); // TODO: Verify that items were added?
            }

            // Assert
            Assert.That(collection, Is.EqualTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListAdd_AddItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetList(items);
            var item = GetLowercaseString(Random);
            var expectedEvents = new[] {
                Added(item, 1, collection),
                Changed(collection)
            };

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).Add(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void SCIListAdd_AddDuplicateItem_RaisesNoEvents()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetList(items, CaseInsensitiveStringComparer.Default);
            var duplicateItem = items.Choose(Random).ToLower();

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).Add(duplicateItem), RaisesNoEventsFor(collection));
        }

        [Test]
        public void SCIListAdd_AddItemDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            ((SC.IList) collection).Add(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListAdd_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListAdd_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListAdd_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Contains(object)

        [Test]
        public void SCIListContains_InvalidType_False()
        {
            // Arrange
            var collection = GetStringList(Random);
            object item = Random.Next();

            // Act
            var contains = collection.Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void SCIListContains_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).Contains(null), Violates.UncaughtPrecondition);
        }

        [Test]
        public void SCIListContains_AllowNullContainsNull_True()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetList(items, allowsNull: true);

            // Act
            var contains = ((SC.IList) collection).Contains(null);

            // Assert
            Assert.That(contains, Is.True);
        }

        [Test]
        public void SCIListContains_AllowNullContainsNoNull_False()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetList(items, allowsNull: true);

            // Act
            var contains = ((SC.IList) collection).Contains(null);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void SCIListContains_EmptyCollection_False()
        {
            // Arrange
            var collection = GetEmptyCollection<string>();
            var item = Random.GetString();

            // Act
            var contains = ((SC.IList) collection).Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void SCIListContains_SingleItemCollectionNonDuplicateItem_False()
        {
            // Arrange
            var item = GetUppercaseString(Random);
            var itemArray = new[] { item };
            var collection = GetList(itemArray);
            var nonDuplicateItem = item.ToLower();

            // Act
            var contains = ((SC.IList) collection).Contains(nonDuplicateItem);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void SCIListContains_SingleItemCollectionDuplicateItem_True()
        {
            // Arrange
            var item = GetUppercaseString(Random);
            var itemArray = new[] { item };
            var collection = GetList(itemArray, CaseInsensitiveStringComparer.Default);
            var duplicateItem = item.ToLower();

            // Act
            var contains = ((SC.IList) collection).Contains(duplicateItem);

            // Assert
            Assert.That(contains, Is.True);
        }

        [Test]
        public void SCIListContains_SingleItemCollectionReferenceInequalItem_False()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetList(itemArray, ReferenceEqualityComparer);
            var nonDuplicateItem = string.Copy(item);

            // Act
            var contains = ((SC.IList) collection).Contains(nonDuplicateItem);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void SCIListContains_RandomCollectionNewItem_False()
        {
            // Arrange
            var collection = GetStringList(Random, ReferenceEqualityComparer);
            var item = Random.GetString();

            // Act
            var contains = ((SC.IList) collection).Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        public void SCIListContains_RandomCollectionExistingItem_True()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetList(items, CaseInsensitiveStringComparer.Default);
            var item = items.Choose(Random).ToLower();

            // Act
            var contains = ((SC.IList) collection).Contains(item);

            // Assert
            Assert.That(contains, Is.True);
        }

        [Test]
        public void SCIListContains_RandomCollectionNewItem_True()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetList(items, ReferenceEqualityComparer);
            var item = string.Copy(items.Choose(Random));

            // Act
            var contains = ((SC.IList) collection).Contains(item);

            // Assert
            Assert.That(contains, Is.False);
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListContains_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        // TODO: Should we rather cast the collection than the object?

        #region IndexOf(T)

        [Test]
        public void SCIListIndexOf_InvalidType_MinusOne()
        {
            // Arrange
            var collection = GetStringList(Random);
            object item = Random.Next();

            // Act
            var indexOf = collection.IndexOf(item);

            // Act & Assert
            Assert.That(indexOf, Is.EqualTo(-1));
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
            Assert.That(indexOf, Is.EqualTo(-1));
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
            Assert.That(indexOf, Is.EqualTo(-1));
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

        // TODO: Should we rather cast the collection than the object?

        #region Insert(int, object)

        [Test]
        public void SCIListInsert_InvalidType_ThrowsArgumentException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            object item = Random.Next();
            var typeString = "System.String";
            var parameterName = "value";
            var exceptionMessage = $"The value \"{item}\" is not of type \"{typeString}\" and cannot be used in this generic collection.{Environment.NewLine}Parameter name: {parameterName}";

            // Act & Assert
            Assert.That(() => collection.Insert(index, item), Throws.ArgumentException.Because(exceptionMessage));
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListInsert_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Remove(object)

        [Test]
        public void SCIListRemove_InvalidType_Nothing()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = collection.ToArray();
            object item = Random.Next();

            // Act
            collection.Remove(item);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListRemove_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).Remove(null), Violates.UncaughtPrecondition);
        }

        [Test]
        public void SCIListRemove_AllowsNull_RemovesNull()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetList(items, allowsNull: true);
            var expected = collection.Where(item => item != null).ToList();

            // Act
            ((SC.IList) collection).Remove(null);

            // Assert
            Assert.That(collection, Is.EqualTo(expected).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListRemove_RemoveExistingItem_RaisesExpectedEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetList(items, CaseInsensitiveStringComparer.Default);
            var existingItem = items.Choose(Random);
            var item = existingItem.ToLower();
            var expectedEvents = new[] {
                Removed(existingItem, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).Remove(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void SCIListRemove_RemoveNewItem_RaisesNoEvents()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).Remove(item), RaisesNoEventsFor(collection));
        }

        [Test]
        public void SCIListRemove_EmptyList_Empty()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var item = Random.GetString();

            // Act
            ((SC.IList) collection).Remove(item);

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void SCIListRemove_RemoveExistingItem_Removed()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = items.Choose(Random).ToLower(); // TODO: Could potentially fail, if there are duplicates
            var collection = GetList(items, CaseInsensitiveStringComparer.Default);
            var expected = collection.SkipIndex(collection.IndexOf(item)).ToList();

            // Act
            ((SC.IList) collection).Remove(item);

            // Assert
            Assert.That(collection, Is.EqualTo(expected).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListRemove_RemoveNewItem_Unchanged()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = GetLowercaseString(Random);
            var collection = GetList(items);
            var expected = collection.ToArray();

            // Act
            ((SC.IList) collection).Remove(item);

            // Assert
            Assert.That(collection, Is.EqualTo(expected).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListRemove_RemoveItemDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var item = items.Choose(Random).ToLower();
            var collection = GetList(items, CaseInsensitiveStringComparer.Default);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            ((SC.IList) collection).Remove(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void SCIListRemove_RemoveItemDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var collection = GetList(items);
            var item = GetLowercaseString(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            ((SC.IList) collection).Remove(item);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListRemove_ReadOnlyList_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListRemove_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListRemove_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region RemoveAt(int)

        [Test]
        public void SCIListRemoveAt_EmptyCollection_ViolatesPrecondtion()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).RemoveAt(0), Violates.Precondition);
        }

        [Test]
        public void SCIListRemoveAt_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(int.MinValue, 0);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).RemoveAt(index), Violates.Precondition);
        }

        [Test]
        public void SCIListRemoveAt_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = collection.Count;

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).RemoveAt(index), Violates.Precondition);
        }

        [Test]
        public void SCIListRemoveAt_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(collection.Count + 1, int.MaxValue);

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).RemoveAt(index), Violates.Precondition);
        }

        [Test]
        public void SCIListRemoveAt_RemoveDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(0, collection.Count);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            ((SC.IList) collection).RemoveAt(index);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void SCIListRemoveAt_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(0, collection.Count);
            var item = collection[index];
            var expectedEvents = new[] {
                RemovedAt(item, index, collection),
                Removed(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => ((SC.IList) collection).RemoveAt(index), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void SCIListRemoveAt_RandomCollection_ItemAtIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(0, collection.Count);
            var expectedItem = collection[index];
            var array = collection.SkipIndex(index).ToArray();

            // Act
            ((SC.IList) collection).RemoveAt(index);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListRemoveAt_RandomCollectionWithNullRemoveNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetList(items, allowsNull: true);
            var index = collection.IndexOf(null);
            var array = collection.SkipIndex(index).ToArray();

            // Act
            ((SC.IList) collection).RemoveAt(index);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListRemoveAt_SingleItemCollection_Item()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetList(itemArray);

            // Act
            ((SC.IList) collection).RemoveAt(0);

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void SCIListRemoveAt_RemoveFirstItem_Removed()
        {
            // Arrange
            var collection = GetStringList(Random);
            var items = collection.ToArray();
            var index = 0;
            var firstItem = collection[index];

            // Act
            ((SC.IList) collection).RemoveAt(index);

            // Assert
            Assert.That(collection, Is.EqualTo(items.Skip(1)).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCIListRemoveAt_RemoveLastItem_Removed()
        {
            // Arrange
            var collection = GetStringList(Random);
            var count = collection.Count;
            var items = collection.ToArray();
            var index = count - 1;
            var lastItem = collection[index];

            // Act
            ((SC.IList) collection).RemoveAt(index);

            // Assert
            Assert.That(collection, Is.EqualTo(items.Take(index)).Using(ReferenceEqualityComparer));
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListRemoveAt_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void SCIListRemoveAt_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
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

        #region RemoveAt(int)

        [Test]
        public void SCGIListRemoveAt_EmptyCollection_ViolatesPrecondtion()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => ((SCG.IList<string>) collection).RemoveAt(0), Violates.Precondition);
        }

        [Test]
        public void SCGIListRemoveAt_NegativeIndex_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(int.MinValue, 0);

            // Act & Assert
            Assert.That(() => ((SCG.IList<string>) collection).RemoveAt(index), Violates.Precondition);
        }

        [Test]
        public void SCGIListRemoveAt_IndexOfCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = collection.Count;

            // Act & Assert
            Assert.That(() => ((SCG.IList<string>) collection).RemoveAt(index), Violates.Precondition);
        }

        [Test]
        public void SCGIListRemoveAt_IndexLargerThanCount_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(collection.Count + 1, int.MaxValue);

            // Act & Assert
            Assert.That(() => ((SCG.IList<string>) collection).RemoveAt(index), Violates.Precondition);
        }

        [Test]
        public void SCGIListRemoveAt_RemoveDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(0, collection.Count);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            ((SCG.IList<string>) collection).RemoveAt(index);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void SCGIListRemoveAt_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(0, collection.Count);
            var item = collection[index];
            var expectedEvents = new[] {
                RemovedAt(item, index, collection),
                Removed(item, 1, collection),
                Changed(collection),
            };

            // Act & Assert
            Assert.That(() => ((SCG.IList<string>) collection).RemoveAt(index), Raises(expectedEvents).For(collection));
        }

        [Test]
        public void SCGIListRemoveAt_RandomCollection_ItemAtIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(0, collection.Count);
            var expectedItem = collection[index];
            var array = collection.SkipIndex(index).ToArray();

            // Act
            ((SCG.IList<string>) collection).RemoveAt(index);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCGIListRemoveAt_RandomCollectionWithNullRemoveNull_Null()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetList(items, allowsNull: true);
            var index = collection.IndexOf(null);
            var array = collection.SkipIndex(index).ToArray();

            // Act
            ((SCG.IList<string>) collection).RemoveAt(index);

            // Assert
            Assert.That(collection, Is.EqualTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCGIListRemoveAt_SingleItemCollection_Item()
        {
            // Arrange
            var item = Random.GetString();
            var itemArray = new[] { item };
            var collection = GetList(itemArray);

            // Act
            ((SCG.IList<string>) collection).RemoveAt(0);

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void SCGIListRemoveAt_RemoveFirstItem_Removed()
        {
            // Arrange
            var collection = GetStringList(Random);
            var items = collection.ToArray();
            var index = 0;
            var firstItem = collection[index];

            // Act
            ((SCG.IList<string>) collection).RemoveAt(index);

            // Assert
            Assert.That(collection, Is.EqualTo(items.Skip(1)).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void SCGIListRemoveAt_RemoveLastItem_Removed()
        {
            // Arrange
            var collection = GetStringList(Random);
            var count = collection.Count;
            var items = collection.ToArray();
            var index = count - 1;
            var lastItem = collection[index];

            // Act
            ((SCG.IList<string>) collection).RemoveAt(index);

            // Assert
            Assert.That(collection, Is.EqualTo(items.Take(index)).Using(ReferenceEqualityComparer));
        }

        [Test]
        [Category("Unfinished")]
        public void SCGIListRemoveAt_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void SCGIListRemoveAt_DuplicatesByCounting_Fail()
        {
            // TODO: Only one item is replaced based on AllowsDuplicates/DuplicatesByCounting
            Assert.That(DuplicatesByCounting, Is.False, "Tests have not been written yet");
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
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
            var expected = collection.InsertItem(index, item);

            // Act
            collection.Insert(index, item);

            // Assert
            Assert.That(collection, Is.EqualTo(expected).Using(ReferenceEqualityComparer));
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void Insert_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void InsertFirst_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void InsertLast_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void InsertRange_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
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

        #region IsSorted(Comparison<T>)

        [Test]
        public void IsSortedComparison_NullComparison_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act & Assert
            Assert.That(() => collection.IsSorted((Comparison<string>) null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void IsSortedComparison_EmptyCollection_True()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            Comparison<string> comparison = string.Compare;

            // Act
            var isSorted = collection.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedComparison_SingleItemCollection_True()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);
            Comparison<string> comparison = string.Compare;

            // Act
            var isSorted = collection.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedComparison_TwoItemsAscending_True()
        {
            // Arrange
            var items = new[] { Random.Next(int.MinValue, 0), Random.Next() };
            var collection = GetList(items);
            Comparison<int> comparison = (x, y) => x.CompareTo(y);

            // Act
            var isSorted = collection.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedComparison_TwoItemsDescending_False()
        {
            // Arrange
            var items = new[] { Random.Next(), Random.Next(int.MinValue, 0) };
            var collection = GetList(items);
            Comparison<int> comparison = (x, y) => x.CompareTo(y);

            // Act
            var isSorted = collection.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        public void IsSortedComparison_TwoEqualItems_True()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item, string.Copy(item) };
            var collection = GetList(items);
            Comparison<string> comparison = string.Compare;

            // Act
            var isSorted = collection.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedComparison_EqualItems_True()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = TestHelper.Repeat(() => string.Copy(item), count);
            var collection = GetList(items);
            Comparison<string> comparison = string.Compare;

            // Act
            var isSorted = collection.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.True);
        }


        [Test]
        public void IsSortedComparison_Comparables_ThrowsNothing()
        {
            // Arrange
            var items = GetComparables(Random);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.IsSorted(_nonComparableComparison), Throws.Nothing);
        }

        [Test]
        public void IsSortedComparison_NonDescendingRandomCollection_True()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted(_nonComparableComparison);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedComparison_Descending_False()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem + 1, previousItem + maxGap)), count).Reverse();
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted(_nonComparableComparison);

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        public void IsSortedComparison_AllButLastAreSorted_False()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count).Append(new Comparable(previousItem - 1));
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted(_nonComparableComparison);

            // Assert
            Assert.That(isSorted, Is.False);
        }

        #endregion

        #region IsSorted(IComparer<T>)

        [Test]
        public void IsSortedIComparer_NullComparer_DoesNotViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act & Assert
            Assert.That(() => collection.IsSorted((SCG.IComparer<string>) null), Does.Not.ViolatePrecondition());
        }

        [Test]
        public void IsSortedIComparer_EmptyCollection_True()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var comparer = SCG.Comparer<string>.Default;

            // Act
            var isSorted = collection.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedIComparer_SingleItemCollection_True()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);
            var comparer = SCG.Comparer<string>.Default;

            // Act
            var isSorted = collection.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedIComparer_TwoItemsAscending_True()
        {
            // Arrange
            var items = new[] { Random.Next(int.MinValue, 0), Random.Next() };
            var collection = GetList(items);
            var comparer = SCG.Comparer<int>.Default;

            // Act
            var isSorted = collection.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedIComparer_TwoItemsDescending_False()
        {
            // Arrange
            var items = new[] { Random.Next(), Random.Next(int.MinValue, 0) };
            var collection = GetList(items);
            var comparer = SCG.Comparer<int>.Default;

            // Act
            var isSorted = collection.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        public void IsSortedIComparer_TwoEqualItems_True()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item, string.Copy(item) };
            var collection = GetList(items);
            var comparer = SCG.Comparer<string>.Default;

            // Act
            var isSorted = collection.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedIComparer_EqualItems_True()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = TestHelper.Repeat(() => string.Copy(item), count);
            var collection = GetList(items);
            var comparer = SCG.Comparer<string>.Default;

            // Act
            var isSorted = collection.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedIComparer_NonComparables_ThrowsArgumentException()
        {
            // Arrange
            var items = GetNonComparables(Random);
            var collection = GetList(items);

            // Act & Assert
            // TODO: This is not the exception stated in the documentation!
            Assert.That(() => collection.IsSorted((SCG.IComparer<NonComparable>) null), Throws.ArgumentException.Because("At least one object must implement IComparable."));
        }

        [Test]
        public void IsSortedIComparer_Comparables_ThrowsNothing()
        {
            // Arrange
            var items = GetComparables(Random);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.IsSorted(NonComparableComparer), Throws.Nothing);
        }

        [Test]
        public void IsSortedIComparer_NonDescendingRandomCollection_True()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted(NonComparableComparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        public void IsSortedIComparer_Descending_False()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem + 1, previousItem + maxGap)), count).Reverse();
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted(NonComparableComparer);

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        public void IsSortedIComparer_AllButLastAreSorted_False()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count).Append(new Comparable(previousItem - 1));
            var collection = GetList(items);

            // Act
            var isSorted = collection.IsSorted(NonComparableComparer);

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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveFirst_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        [Test]
        [Category("Unfinished")]
        public void RemoveLast_FixedSizeCollection_Fail()
        {
            Assert.That(IsFixedSize, Is.False, "Tests have not been written yet");
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
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Shuffle()

        [Test]
        public void Shuffle_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => collection.Shuffle(), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Shuffle_SingleItemCollection_RaisesNoEvents()
        {
            // Arrange
            var items = GetStrings(Random, 1);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Shuffle(), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.EqualTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Shuffle_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act & Assert
            Assert.That(() => collection.Shuffle(), RaisesCollectionChangedEventFor(collection));
        }

        [Test]
        public void Shuffle_RandomCollection_ContainsSameItems()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = collection.ToArray();

            // Act
            collection.Shuffle();

            // Assert
            Assert.That(collection, Is.EquivalentTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Shuffle_RandomCollection_NotEqualThreeTimes()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            collection.Shuffle();
            var result1 = collection.ToArray();
            collection.Shuffle();
            var result2 = collection.ToArray();
            collection.Shuffle();
            var result3 = collection.ToArray();

            // Assert
            Assert.That(result1, Is.EquivalentTo(result2).And.EquivalentTo(result3));
            Assert.That(result1, Is.Not.EqualTo(result2).And.Not.EqualTo(result3), "If this test fails more than once, the Shuffle() method likely contains an error");
        }

        [Test]
        public void Shuffle_ShuffleDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Shuffle();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void Shuffle_ShuffleEmptyCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Shuffle();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void Shuffle_ShuffleSingleItemCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetStrings(Random, 1);
            var collection = GetList(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Shuffle();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        [Category("Unfinished")]
        public void Shuffle_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Shuffle(Random)

        [Test]
        public void ShuffleRandom_NullArgument_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act & Assert
            Assert.That(() => collection.Shuffle(null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void ShuffleRandom_EmptyCollection_RaisesNoEvents()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => collection.Shuffle(Random), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void ShuffleRandom_SingleItemCollection_RaisesNoEvents()
        {
            // Arrange
            var items = GetStrings(Random, 1);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Shuffle(Random), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.EqualTo(items).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void ShuffleRandom_RandomCollection_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act & Assert
            Assert.That(() => collection.Shuffle(Random), RaisesCollectionChangedEventFor(collection));
        }

        [Test]
        public void ShuffleRandom_RandomCollection_ContainsSameItems()
        {
            // Arrange
            var collection = GetStringList(Random);
            var array = collection.ToArray();

            // Act
            collection.Shuffle(Random);

            // Assert
            Assert.That(collection, Is.EquivalentTo(array).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void ShuffleRandom_RandomCollection_NotEqualThreeTimes()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            collection.Shuffle(Random);
            var result1 = collection.ToArray();
            collection.Shuffle(Random);
            var result2 = collection.ToArray();
            collection.Shuffle(Random);
            var result3 = collection.ToArray();

            // Assert
            Assert.That(result1, Is.EquivalentTo(result2).And.EquivalentTo(result3));
            Assert.That(result1, Is.Not.EqualTo(result2).And.Not.EqualTo(result3), "If this test fails more than once, the Shuffle(Random) method likely contains an error");
        }

        [Test]
        public void ShuffleRandom_EqualCollections_SameResultUsingEqualRandom()
        {
            // Arrange
            var seed = Random.Next();
            var random1 = new Random(seed);
            var random2 = new Random(seed);
            var collection1 = GetStringList(Random);
            var collection2 = GetList(collection1);

            // Act
            collection1.Shuffle(random1);
            collection2.Shuffle(random2);

            // Assert
            Assert.That(collection1, Is.EqualTo(collection2).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void ShuffleRandom_ShuffleDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Shuffle(Random);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        public void ShuffleRandom_ShuffleEmptyCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Shuffle(Random);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void ShuffleRandom_ShuffleSingleItemCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetStrings(Random, 1);
            var collection = GetList(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Shuffle(Random);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        [Category("Unfinished")]
        public void ShuffleRandom_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Sort()

        [Test]
        public void Sort_EmptyCollection_Nothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_SingleItemCollection_Nothing()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_TwoItemsAscending_Nothing()
        {
            // Arrange
            var items = new[] { Random.Next(int.MinValue, 0), Random.Next() };
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_TwoItemsDescending_Sorted()
        {
            // Arrange
            var items = new[] { Random.Next(), Random.Next(int.MinValue, 0) };
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_TwoEqualItems_Nothing()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item, string.Copy(item) };
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_EqualItems_Nothing()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = TestHelper.Repeat(() => string.Copy(item), count);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        // TODO: Test again when NUnit changes its behavior
        [Test]
        [Ignore("Because NUnit does not allow null values in Ordered: https://github.com/nunit/nunit/issues/1473")]
        public void Sort_RandomCollectionWithNull_Sorted()
        {
            // Arrange
            var items = GetStrings(Random);
            var index = Random.Next(1, items.Length);
            items[index] = null;
            var collection = GetList(items, allowsNull: true);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_NonComparables_ThrowsArgumentException()
        {
            // Arrange
            var items = GetNonComparables(Random);
            var collection = GetList(items);

            // Act & Assert
            // TODO: This is not the exception stated in the documentation!
            Assert.That(() => collection.Sort(), Throws.ArgumentException.Because("At least one object must implement IComparable."));
        }

        [Test]
        public void Sort_NonDescendingRandomCollection_Nothing()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_Descending_Sorted()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem + 1, previousItem + maxGap)), count).Reverse();
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_AllButLastAreSorted_Sorted()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count).Append(new Comparable(previousItem - 1));
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_RandomCollection_Sorted()
        {
            // Arrange
            var items = GetStrings(Random, 10000);
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void Sort_SortEmptyCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void Sort_SortSingleItemCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetStrings(Random, 1);
            var collection = GetList(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void Sort_SortSortedCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void Sort_SortDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort();

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void Sort_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Sort(Comparison<T>)

        [Test]
        public void SortComparison_NullComparison_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act & Assert
            Assert.That(() => collection.Sort((Comparison<string>) null), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        public void SortComparison_EmptyCollection_Nothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            Comparison<string> comparison = string.Compare;

            // Act & Assert
            Assert.That(() => collection.Sort(comparison), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_SingleItemCollection_Nothing()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);
            Comparison<string> comparison = string.Compare;

            // Act & Assert
            Assert.That(() => collection.Sort(comparison), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_TwoItemsAscending_Nothing()
        {
            // Arrange
            var items = new[] { Random.Next(int.MinValue, 0), Random.Next() };
            var collection = GetList(items);
            Comparison<int> comparison = (x, y) => x.CompareTo(y);

            // Act & Assert
            Assert.That(() => collection.Sort(comparison), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_TwoItemsDescending_Sorted()
        {
            // Arrange
            var items = new[] { Random.Next(), Random.Next(int.MinValue, 0) };
            var collection = GetList(items);
            Comparison<int> comparison = (x, y) => x.CompareTo(y);

            // Act & Assert
            Assert.That(() => collection.Sort(comparison), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_TwoEqualItems_Nothing()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item, string.Copy(item) };
            var collection = GetList(items);
            Comparison<string> comparison = string.Compare;

            // Act & Assert
            Assert.That(() => collection.Sort(comparison), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_EqualItems_Nothing()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = TestHelper.Repeat(() => string.Copy(item), count);
            var collection = GetList(items);
            Comparison<string> comparison = string.Compare;

            // Act & Assert
            Assert.That(() => collection.Sort(comparison), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        // TODO: Test again when NUnit changes its behavior
        [Test]
        [Ignore("Because NUnit does not allow null values in Ordered: https://github.com/nunit/nunit/issues/1473")]
        public void SortComparison_RandomCollectionWithNull_Sorted()
        {
            // Arrange
            var items = GetStrings(Random);
            var index = Random.Next(1, items.Length);
            items[index] = null;
            var collection = GetList(items, allowsNull: true);
            Comparison<string> comparison = string.Compare;

            // Act & Assert
            Assert.That(() => collection.Sort(comparison), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_NonDescendingRandomCollection_Nothing()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);


            // Act & Assert
            Assert.That(() => collection.Sort(_nonComparableComparison), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_Descending_Sorted()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem + 1, previousItem + maxGap)), count).Reverse();
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(_nonComparableComparison), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_AllButLastAreSorted_Sorted()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count).Append(new Comparable(previousItem - 1));
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(_nonComparableComparison), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_RandomCollection_Sorted()
        {
            // Arrange
            var items = GetStrings(Random, 10000);
            var collection = GetList(items);
            Comparison<string> comparison = string.Compare;

            // Act & Assert
            Assert.That(() => collection.Sort(comparison), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortComparison_SortEmptyCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            Comparison<string> comparison = string.Compare;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort(comparison);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void SortComparison_SortSingleItemCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetStrings(Random, 1);
            var collection = GetList(items);
            Comparison<string> comparison = string.Compare;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort(comparison);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void SortComparison_SortSortedCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort(_nonComparableComparison);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void SortComparison_SortDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            Comparison<string> comparison = string.Compare;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort(comparison);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void SortComparison_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
        }

        #endregion

        #region Sort(IComparer<T>)

        [Test]
        public void SortIComparer_NullComparison_DoesNotViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random);

            // Act & Assert
            Assert.That(() => collection.Sort((SCG.IComparer<string>) null), Does.Not.ViolatePrecondition());
        }

        [Test]
        public void SortIComparer_EmptyCollection_Nothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var comparer = SCG.Comparer<string>.Default;

            // Act & Assert
            Assert.That(() => collection.Sort(comparer), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_SingleItemCollection_Nothing()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item };
            var collection = GetList(items);
            var comparer = SCG.Comparer<string>.Default;

            // Act & Assert
            Assert.That(() => collection.Sort(comparer), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_TwoItemsAscending_Nothing()
        {
            // Arrange
            var items = new[] { Random.Next(int.MinValue, 0), Random.Next() };
            var collection = GetList(items);
            var comparer = SCG.Comparer<int>.Default;

            // Act & Assert
            Assert.That(() => collection.Sort(comparer), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_TwoItemsDescending_Sorted()
        {
            // Arrange
            var items = new[] { Random.Next(), Random.Next(int.MinValue, 0) };
            var collection = GetList(items);
            var comparer = SCG.Comparer<int>.Default;

            // Act & Assert
            Assert.That(() => collection.Sort(comparer), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_TwoEqualItems_Nothing()
        {
            // Arrange
            var item = Random.GetString();
            var items = new[] { item, string.Copy(item) };
            var collection = GetList(items);
            var comparer = SCG.Comparer<string>.Default;

            // Act & Assert
            Assert.That(() => collection.Sort(comparer), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_EqualItems_Nothing()
        {
            // Arrange
            var count = GetCount(Random);
            var item = Random.GetString();
            var items = TestHelper.Repeat(() => string.Copy(item), count);
            var collection = GetList(items);
            var comparer = SCG.Comparer<string>.Default;

            // Act & Assert
            Assert.That(() => collection.Sort(comparer), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        // TODO: Test again when NUnit changes its behavior
        [Test]
        [Ignore("Because NUnit does not allow null values in Ordered: https://github.com/nunit/nunit/issues/1473")]
        public void SortIComparer_RandomCollectionWithNull_Sorted()
        {
            // Arrange
            var items = GetStrings(Random);
            var index = Random.Next(1, items.Length);
            items[index] = null;
            var collection = GetList(items, allowsNull: true);
            var comparer = SCG.Comparer<string>.Default;

            // Act & Assert
            Assert.That(() => collection.Sort(comparer), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_NonDescendingRandomCollection_Nothing()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);


            // Act & Assert
            Assert.That(() => collection.Sort(NonComparableComparer), RaisesNoEventsFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_Descending_Sorted()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem + 1, previousItem + maxGap)), count).Reverse();
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(NonComparableComparer), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_AllButLastAreSorted_Sorted()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count).Append(new Comparable(previousItem - 1));
            var collection = GetList(items);

            // Act & Assert
            Assert.That(() => collection.Sort(NonComparableComparer), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_RandomCollection_Sorted()
        {
            // Arrange
            var items = GetStrings(Random, 10000);
            var collection = GetList(items);
            var comparer = SCG.Comparer<string>.Default;

            // Act & Assert
            Assert.That(() => collection.Sort(comparer), RaisesCollectionChangedEventFor(collection));

            // Assert
            Assert.That(collection, Is.Ordered);
        }

        [Test]
        public void SortIComparer_SortEmptyCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var comparer = SCG.Comparer<string>.Default;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort(comparer);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void SortIComparer_SortSingleItemCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var items = GetStrings(Random, 1);
            var collection = GetList(items);
            var comparer = SCG.Comparer<string>.Default;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort(comparer);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void SortIComparer_SortSortedCollectionDuringEnumeration_ThrowsNothing()
        {
            // Arrange
            var count = GetCount(Random);
            var previousItem = 0;
            var maxGap = 5;
            var items = TestHelper.Repeat(() => new Comparable(previousItem = Random.Next(previousItem, previousItem + maxGap)), count);
            var collection = GetList(items);
            var comparer = SCG.Comparer<Comparable>.Default;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort(comparer);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.Nothing);
        }

        [Test]
        public void SortIComparer_SortDuringEnumeration_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringList(Random);
            var comparer = SCG.Comparer<string>.Default;

            // Act
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            collection.Sort(comparer);

            // Assert
            Assert.That(() => enumerator.MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        [Test]
        [Category("Unfinished")]
        public void SortIComparer_ReadOnlyCollection_Fail()
        {
            Assert.That(IsReadOnly, Is.False, "Tests have not been written yet");
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


        private readonly Comparison<NonComparable> _nonComparableComparison = (x, y) => x.Value.CompareTo(y.Value);

        private SCG.IComparer<NonComparable> NonComparableComparer => _nonComparableComparison.ToComparer();

        #endregion
    }
}