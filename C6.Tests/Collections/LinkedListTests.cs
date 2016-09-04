// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;

using C6.Collections;
using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;

using static System.Diagnostics.Contracts.Contract;

using static C6.EventTypes;
using static C6.Speed;
using static C6.Tests.Helpers.TestHelper;

using Assert = NUnit.Framework.Assert;
using SCG = System.Collections.Generic;


namespace C6.Tests.Collections
{
    [TestFixture]
    public class LinkedListTests : TestBase
    {
        #region Constructors

        [Test]
        public void Constructor_Default_Empty()
        {
            // Act
            var collection = new LinkedList<int>();

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Constructor_Default_DefaultEqualityComparer()
        {
            // Arrange
            var defaultEqualityComparer = SCG.EqualityComparer<string>.Default;

            // Act
            var collection = new LinkedList<string>();
            var equalityComparer = collection.EqualityComparer;

            // Assert
            Assert.That(equalityComparer, Is.SameAs(defaultEqualityComparer));
        }

        [Test]
        public void Constructor_DefaultForValueType_DisallowsNull()
        {
            // Act
            var collection = new LinkedList<int>();
            var allowsNull = collection.AllowsNull;

            // Assert
            Assert.That(allowsNull, Is.False);
        }

        [Test]
        public void Constructor_DefaultForNonValue_DisallowsNull()
        {
            // Act
            var collection = new LinkedList<string>();
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
            Assert.That(() => new LinkedList<int>(allowsNull: allowsNull), Violates.UncaughtPrecondition);
        }

        [Test]
        public void Constructor_ValueTypeCollectionDisallowsNull_DisallowsNull()
        {
            // Act
            var collection = new LinkedList<int>(allowsNull: false);

            // Assert
            Assert.That(collection.AllowsNull, Is.False);
        }

        [Test]
        public void Constructor_NonValueTypeCollection_AllowNull([Values(true, false)] bool allowNull)
        {
            // Act
            var collection = new LinkedList<string>(allowsNull: allowNull);
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
            Assert.That(() => new LinkedList<string>(enumerable), Violates.UncaughtPrecondition);
        }

        [Test]
        public void Constructor_EmptyEnumerable_Empty()
        {
            // Arrange
            var enumerable = Enumerable.Empty<int>();

            // Act
            var list = new LinkedList<int>(enumerable);

            // Assert
            Assert.That(list, Is.Empty);
        }

        [Test]
        public void Constructor_RandomNonValueTypeEnumerable_Equal()
        {
            // Arrange
            var array = GetStrings(Random);

            // Act
            var list = new LinkedList<string>(array);

            // Assert
            Assert.That(list, Is.EqualTo(array).ByReference<string>());
        }

        [Test]
        public void Constructor_EnumerableWithNull_ViolatesPrecondition()
        {
            // Arrange
            var array = GetStrings(Random).WithNull(Random);

            // Act & Assert
            Assert.That(() => new LinkedList<string>(array), Violates.UncaughtPrecondition);
        }

        [Test]
        public void Constructor_EnumerableBeingChanged_Unequal()
        {
            // Arrange
            var array = GetIntegers(Random);

            // Act
            var collection = new LinkedList<int>(array);
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
            Assert.That(() => new LinkedList<string>(array, allowsNull: false), Violates.UncaughtPrecondition);
        }

        [Test]
        public void Constructor_EqualityComparer_EqualsGivenEqualityComparer()
        {
            // Arrange
            var customEqualityComparer = ComparerFactory.CreateEqualityComparer<int>((i, j) => i == j, i => i);

            // Act
            var list = new LinkedList<int>(equalityComparer: customEqualityComparer);
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
            var list = new LinkedList<int>(enumerable, customEqualityComparer);
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
            var collection = new LinkedList<string>(enumerable);

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
            var collection = new LinkedList<string>(enumerable);

            // Assert
            Assert.That(collection, Is.EqualTo(items).ByReference<string>());
        }

        [Test]
        public void Constructor_EmptyICollectionValue_Empty()
        {
            // Arrange
            var collectionValue = new LinkedList<string>();

            // Act
            var collection = new LinkedList<string>(collectionValue);

            // Assert
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Constructor_RandomICollectionValue_Equal()
        {
            // Arrange
            var items = GetStrings(Random);
            var collectionValue = new LinkedList<string>(items);

            // Act
            var collection = new LinkedList<string>(collectionValue);

            // Assert
            Assert.That(collection, Is.EqualTo(items).ByReference<string>());
        }

        #endregion

        #region Methods

        #region Add(T)

        [Test]
        public void Add_InsertAddedToTheEnd_LastItemSame()
        {
            // Arrange
            var items = GetStrings(Random);
            var list = new LinkedList<string>(items);
            var item = GetString(Random);

            // Act
            list.Add(item);

            // Assert
            Assert.That(list.Last, Is.SameAs(item));
        }

        #endregion

        #region Choose()

        [Test]
        public void Choose_RandomCollection_LastItem()
        {
            // Arrange
            var enumerable = GetStrings(Random);
            var list = new LinkedList<string>(enumerable);
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
            var collection = new LinkedList<string>(items);
            var count = Random.Next(1, collection.Count);
            var startIndex = Random.Next(0, collection.Count - count);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection.Skip(startIndex).Take(count),
                collection.EqualityComparer,
                collection.AllowsNull,
                () => collection[startIndex + count - 1]
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
            var collection = new LinkedList<string>(items);
            var count = Random.Next(1, collection.Count);
            var startIndex = Random.Next(0, collection.Count - count);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection.Skip(startIndex).Take(count).Reverse(),
                collection.EqualityComparer,
                collection.AllowsNull,
                () => collection[startIndex + count - 1],
                EnumerationDirection.Backwards
                );

            // Act
            var getIndexRange = collection.GetIndexRange(startIndex, count).Backwards();

            // Assert
            Assert.That(getIndexRange, Is.EqualTo(expected));
        }

        #endregion

        #region Reverse

        [Test]
        public void Reverse_ReverseHalfFullCollection_Reversed()
        {
            // Arrange
            var items = GetStrings(Random);
            var collection = new LinkedList<string>(items);
            collection.RemoveIndexRange(0, collection.Count / 2);
            var expected = collection.ToArray().Reverse();

            // Act
            collection.Reverse();

            // Assert
            Assert.That(collection, Is.EqualTo(expected).ByReference<string>());
        }

        #endregion

        #endregion
    }


    [TestFixture]
    public class LinkedListListTests : IListTests
    {
        #region Properties

        protected override bool AllowsDuplicates => true;

        protected override Speed ContainsSpeed => Linear;

        protected override bool DuplicatesByCounting => false;

        protected override Speed IndexingSpeed => Linear;

        protected override bool IsFixedSize => false;

        protected override bool IsReadOnly => false;

        protected override EventTypes ListenableEvents => All;

        #endregion

        #region Methods

        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            Requires(collection is LinkedList<T>);

            var linkedList = (LinkedList<T>) collection;

            // TODO: Use Last
            yield return linkedList.Last();
        }

        protected override IList<T> GetEmptyList<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => new LinkedList<T>(equalityComparer, allowsNull);

        protected override IList<T> GetList<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => new LinkedList<T>(enumerable, equalityComparer, allowsNull);

        #endregion
    }


    [TestFixture]
    public class LinkedListStackTests : IStackTests
    {
        protected override bool IsReadOnly => false;
        protected override EventTypes ListenableEvents => All;

        protected override IStack<T> GetEmptyStack<T>(bool allowsNull = false) => new LinkedList<T>(allowsNull: allowsNull);
        protected override IStack<T> GetStack<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new LinkedList<T>(enumerable, allowsNull: allowsNull);
        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            Requires(collection is LinkedList<T>);

            var linkedList = (LinkedList<T>) collection;

            yield return linkedList.Last;
        }
    }


    [TestFixture]
    public class LinkedListQueueTests : IQueueTests
    {
        protected override bool IsReadOnly => false;
        protected override EventTypes ListenableEvents => All;

        protected override IQueue<T> GetEmptyQueue<T>(bool allowsNull = false) => new LinkedList<T>(allowsNull: allowsNull);
        protected override IQueue<T> GetQueue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new LinkedList<T>(enumerable, allowsNull: allowsNull);
        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection)
        {
            Requires(collection is LinkedList<T>);

            var linkedList = (LinkedList<T>) collection;

            yield return linkedList.Last;
        }
    }


    [TestFixture]
    public class LinkedListDirectedCollectionValueTests : IDirectedCollectionValueTests
    {
        protected override SCG.IEnumerable<T> ChooseItems<T>(ICollectionValue<T> collection) => new[] { collection.First() };

        protected override IDirectedCollectionValue<T> GetEmptyDirectedCollectionValue<T>(bool allowsNull = false) => new LinkedList<T>(allowsNull: allowsNull);

        protected override IDirectedCollectionValue<T> GetDirectedCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => new LinkedList<T>(enumerable, allowsNull: allowsNull);

        protected override void ChangeCollection<T>(IDirectedCollectionValue<T> collection, T item) => ((LinkedList<T>) collection).UpdateOrAdd(item);
    }
}