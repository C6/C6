// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;
using System.Reflection;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.EventTypes;
using static C6.Tests.Helpers.TestHelper;
using static C6.Tests.Helpers.CollectionEvent;
using static C6.ExceptionMessages;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public class ArrayListTests : IListTests
    {
        #region Helper Methods
        
        private IList<string> GetStringList(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetList(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        #endregion

        #region Factories

        #endregion

        #region ArrayList<T>

        #region Constructors

        [Test]
        public void Constructor_Default_Empty()
        {
            // Act
            var collection = new ArrayList<int>();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Constructor_Default_DefaultEqualityComparer()
        {
            // Arrange
            var defaultEqualityComparer = SCG.EqualityComparer<string>.Default;

            // Act
            var collection = new ArrayList<string>();
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(defaultEqualityComparer));
        }

        [Test]
        public void Constructor_DefaultForValueType_DisallowsNull()
        {
            // Act
            var collection = new ArrayList<int>();
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        [Test]
        public void Constructor_DefaultForNonValue_DisallowsNull()
        {
            // Act
            var collection = new ArrayList<string>();
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        [Test]
        public void Constructor_ValueTypeCollectionAllowsNull_ViolatesPrecondition()
        {
            // Arrange
            var allowsNull = true;

            // Act & Assert
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.That(() => new ArrayList<int>(allowsNull: allowsNull), Violates.UncaughtPrecondition);
        }

        [Test]
        public void Constructor_ValueTypeCollectionDisallowsNull_DisallowsNull()
        {
            // Act
            var collection = new ArrayList<int>(allowsNull: false);

            // Assert
            Assert.That(collection.AllowsNull, Is.False);
        }

        [Test]
        public void Constructor_NonValueTypeCollection_AllowNull([Values(true, false)] bool allowNull)
        {
            // Act
            var collection = new ArrayList<string>(allowsNull: allowNull);
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.EqualTo(allowNull));
        }

        [Test]
        public void Constructor_NullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            SCG.IEnumerable<string> enumerable = null;

            // Act & Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.That(() => new ArrayList<string>(enumerable), Violates.UncaughtPrecondition);
        }

        [Test]
        public void Constructor_EmptyEnumerable_Empty()
        {
            // Arrange
            var enumerable = Enumerable.Empty<int>();

            // Act
            var list = new ArrayList<int>(enumerable);

            // Assert
            Assert.That(list, Is.Empty);
        }

        [Test]
        public void Constructor_RandomNonValueTypeEnumerable_Equal()
        {
            // Arrange
            var array = GetStrings(Random);

            // Act
            var list = new ArrayList<string>(array);

            // Assert
            Assert.That(list, Is.EqualTo(array));
        }

        [Test]
        public void Constructor_EnumerableWithNull_ViolatesPrecondition()
        {
            // Arrange
            var array = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => new ArrayList<string>(array), Violates.UncaughtPrecondition);
        }

        [Test]
        public void Constructor_EnumerableBeingChanged_Unequal()
        {
            // Arrange
            var array = GetIntegers(Random);

            // Act
            var collection = new ArrayList<int>(array);
            for (var i = 0; i < array.Length; i++) {
                array[i] *= -1;
            }

            // Assert
            Assert.That(collection, Is.Not.EqualTo(array));
        }

        [Test]
        public void Constructor_EnumerableWithNullDisallowNull_ViolatesPrecondition()
        {
            // Arrange
            var array = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => new ArrayList<string>(array, allowsNull: false), Violates.UncaughtPrecondition);
        }

        [Test]
        public void Constructor_EqualityComparer_EqualsGivenEqualityComparer()
        {
            // Arrange
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);

            // Act
            var list = new ArrayList<int>(equalityComparer: customEqualityComparer);
            var equalityComparer = list.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(customEqualityComparer));
        }

        [Test]
        public void Constructor_EnumerableConstructorEqualityComparer_EqualsGivenEqualityComparer()
        {
            // Arrange
            var enumerable = Enumerable.Empty<int>();
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);

            // Act
            var list = new ArrayList<int>(enumerable, customEqualityComparer);
            var equalityComparer = list.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(customEqualityComparer));
        }

        [Test]
        public void Constructor_EmptySCGIList_Empty()
        {
            // Arrange
            var enumerable = NoStrings.ToList();

            // Act
            var collection = new ArrayList<string>(enumerable);

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Constructor_RandomSCGIList_Equal()
        {
            // Arrange
            var items = GetStrings(Random).ToList();

            // Act
            var collection = new ArrayList<string>(items);

            // Assert
            Assert.That(collection, Is.EqualTo(GetStrings(Random)).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void Constructor_EmptyICollectionValue_Empty()
        {
            // Arrange
            var collectionValue = new ArrayList<string>();

            // Act
            var collection = new ArrayList<string>(collectionValue);

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Constructor_RandomICollectionValue_Equal()
        {
            // Arrange
            var items = GetStrings(Random);
            var collectionValue = new ArrayList<string>(items);

            // Act
            var collection = new ArrayList<string>(collectionValue);

            // Assert
            Assert.That(collection, Is.EqualTo(items).Using(ReferenceEqualityComparer));
        }

        #endregion

        #region Methods

        #region Add(T)

        [Test]
        public void Add_InsertAddedToTheEnd_LastItemSame()
        {
            // Arrange
            var items = GetStrings(Random);
            var list = new ArrayList<string>(items);
            var item = Random.GetString();

            // Act
            list.Add(item);

            // Assert
            Assert.That(list.Last(), Is.SameAs(item)); // TODO: Update to Last
        }

        #endregion

        #region Choose()

        [Test]
        public void Choose_RandomCollection_LastItem()
        {
            // Arrange
            var enumerable = GetStrings(Random);
            var list = new ArrayList<string>(enumerable);
            var lastItem = enumerable.Last();

            // Act
            var choose = list.Choose();

            // Assert
            Assert.That(choose, Is.SameAs(lastItem));
        }

        #endregion

        #region GetIndexRange(int, int)

        [Test]
        public void GetIndexRange_ForwardsRange_ChooseReturnsLastItem()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetList(items);
            var count = Random.Next(1, collection.Count);
            var startIndex = Random.Next(0, collection.Count - count);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection,
                collection.Skip(startIndex).Take(count),
                chooseFunction: () => collection[startIndex + count - 1]
            );

            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, count);

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(expected));
        }

        [Test]
        public void GetIndexRange_BackwardsRange_ChooseReturnsLastItem()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = GetList(items);
            var count = Random.Next(1, collection.Count);
            var startIndex = Random.Next(0, collection.Count - count);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection,
                collection.Skip(startIndex).Take(count).Reverse(),
                direction: EnumerationDirection.Backwards,
                chooseFunction: () => collection[startIndex + count - 1]
            );

            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, count).Backwards();

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(expected));
        }

        #endregion

        #region InsertRange(int, IEnumerable<T>)

        public static SCG.IEnumerable<T> ToEnumerable<T>(SCG.IEnumerable<T> items) => items;
        public static SCG.IEnumerable<T> ToArray<T>(SCG.IEnumerable<T> items) => items.ToArray();
        public static SCG.IEnumerable<T> ToList<T>(SCG.IEnumerable<T> items) => items.ToList();
        public static SCG.IEnumerable<T> ToCollectionValue<T>(SCG.IEnumerable<T> items) => new ArrayList<T>(items);

        public static SCG.IEnumerable<T> Map<T>(string methodName, SCG.IEnumerable<T> enumerable)
            => (SCG.IEnumerable<T>) typeof(ArrayListTests).GetMethod(methodName).MakeGenericMethod(typeof(T)).Invoke(null, new object[] { enumerable });

        [Test]
        [TestCase(nameof(ToEnumerable))]
        [TestCase(nameof(ToArray))]
        [TestCase(nameof(ToList))]
        [TestCase(nameof(ToCollectionValue))]
        public void InsertRange_RandomCollectionInsertExistingItems_InsertedRange(string mapperMethod)
        {
            Run.If(AllowsDuplicates);
            
            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var count = GetCount(Random);
            var items = Map(mapperMethod, collection.ShuffledCopy(Random).Take(count));
            var expected = collection.InsertItems(index, items);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(expected).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_RandomCollectionInsertExistingItemsAsCollectionValue_InsertedRange()
        {
            Run.If(AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = GetIndex(collection, Random, true);
            var count = GetCount(Random);
            var items = GetCollectionValue(collection.ShuffledCopy(Random).Take(count));
            var expected = collection.ToArray().InsertItems(index, items);

            // Act
            collection.InsertRange(index, items);

            // Assert
            Assert.That(collection, Is.EqualTo(expected).Using(ReferenceEqualityComparer));
        }

        [Test]
        public void InsertRange_EmptyCollectionInsertCollection_Items()
        {
            // Arrange
            var collection = GetEmptyList<string>();
            var index = 0;
            var items = GetStrings(Random).ToList();

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

        #endregion

        #endregion

        protected override EventTypes ListenableEvents => All;
        protected override bool AllowsDuplicates => true;
        protected override bool DuplicatesByCounting => false;
        protected override bool IsFixedSize => false;
        protected override bool IsReadOnly => false;
        protected override Speed ContainsSpeed => Speed.Linear;

        protected override Speed IndexingSpeed => Speed.Constant;

        protected override IList<T> GetEmptyList<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => new ArrayList<T>(equalityComparer: equalityComparer, allowsNull: allowsNull);

        protected override IList<T> GetList<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => new ArrayList<T>(enumerable, equalityComparer, allowsNull);
    }
}