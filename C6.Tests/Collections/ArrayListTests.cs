// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;

using static C6.EventTypes;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public class ArrayListTests : IListTests
    {
        #region Helper Methods

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
            var enumerable = new SCG.List<string>();

            // Act
            var collection = new ArrayList<string>(enumerable);

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Constructor_RandomSCGIList_Equal()
        {
            // Arrange
            var items = GetStrings(Random);
            var enumerable = new SCG.List<string>(items);

            // Act
            var collection = new ArrayList<string>(enumerable);

            // Assert
            Assert.That(collection, Is.EqualTo(items).Using(ReferenceEqualityComparer));
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