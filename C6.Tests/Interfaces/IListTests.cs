// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Linq;
using System.Text;

using C6.Contracts;
using C6.Tests.Contracts;
using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.Contracts.ContractMessage;
using static C6.ExceptionMessages;
using static C6.Tests.Helpers.CollectionEvent;
using static C6.Tests.Helpers.TestHelper;

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

        #endregion

        #region Methods

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
            var item = Random.GetString();

            // Act & Assert
            Assert.That(() => collection[0] = item, Violates.PreconditionSaying(ArgumentMustBeWithinBounds));
        }

        [Test]
        public void ItemSet_DisallowsNull_ViolatesPrecondition()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: false);
            var index = Random.Next(0, collection.Count);

            // Act & Assert
            Assert.That(() => collection[index] = null, Violates.PreconditionSaying(ItemMustBeNonNull));
        }

        [Test]
        public void ItemSet_RandomCollectionSetDuplicate_ViolatesPrecondition()
        {
            Run.If(!AllowsDuplicates);

            // Arrange
            var collection = GetStringList(Random);
            var index = Random.Next(0, collection.Count);
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
            var index = Random.Next(0, collection.Count);
            var item = collection.ToArray().Choose(Random);

            // Act
            collection[index] = item;

            // Assert
            Assert.That(collection[index], Is.SameAs(item));
            Assert.That(collection.CountDuplicates(item), Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void ItemSet_AllowsNull_Null()
        {
            // Arrange
            var collection = GetStringList(Random, allowsNull: true);
            var index = Random.Next(0, collection.Count);
            
            // Act
            collection[index] = null;
            
            // Assert
            Assert.That(collection[index], Is.Null);
        }

        [Test]
        public void ItemSet_RandomCollectionIndexZero_FirstItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = 0;

            // Act
            collection[index] = item;

            // Assert
            Assert.That(collection[index], Is.SameAs(item));
        }

        [Test]
        public void ItemSet_RandomCollectionIndexCountMinusOne_LastItem()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = collection.Count - 1;

            // Act
            collection[index] = item;

            // Assert
            Assert.That(collection[index], Is.SameAs(item));
        }

        [Test]
        public void ItemSet_RandomCollectionRandomIndex_ItemAtPositionIndex()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = Random.Next(0, collection.Count);

            // Act
            collection[index] = item;

            // Assert
            Assert.That(collection[index], Is.SameAs(item));
        }

        [Test]
        public void ItemSet_RandomCollectionRandomIndex_RaisesExpectedEvents()
        {
            // Arrange
            var collection = GetStringList(Random);
            var item = Random.GetString();
            var index = Random.Next(0, collection.Count);
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
            var index = Random.Next(0, collection.Count);

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
            Run.If(IsReadOnly);

            Assert.Fail("Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}