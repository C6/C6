using NUnit.Framework;

using System.Linq;
using System.Text;

using NUnit.Framework.Internal;

using C6.Tests.Helpers;

using static C6.EnumerationDirection;
using static C6.Collections.ExceptionMessages;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class IDirectedCollectionValueTests : ICollectionValueTests
    {
        #region Factories

        protected abstract IDirectedCollectionValue<T> GetEmptyDirectedCollectionValue<T>(bool allowsNull = false);

        protected abstract IDirectedCollectionValue<T> GetDirectedCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false);

        protected abstract void ChangeCollection<T>(IDirectedCollectionValue<T> collection, T item);

        #region Helpers

        private IDirectedCollectionValue<string> GetStringSequence(Randomizer random, bool allowsNull = false)
            => GetDirectedCollectionValue(GetStrings(random, GetCount(random)), allowsNull);

        private IDirectedCollectionValue<string> GetStringSequence(Randomizer random, int count, bool allowsNull = false)
            => GetDirectedCollectionValue(GetStrings(random, count), allowsNull);

        private IDirectedCollectionValue<string> GetStringDirectedCollectionValue(Randomizer random, bool allowsNull = false)
            => GetDirectedCollectionValue(GetStrings(random, GetCount(random)), allowsNull);

        private IDirectedCollectionValue<string> GetStringDirectedCollectionValue(Randomizer random, int count, bool allowsNull = false)
            => GetDirectedCollectionValue(GetStrings(random, count), allowsNull);

        #endregion

        #region Inherited

        protected override ICollectionValue<T> GetEmptyCollectionValue<T>(bool allowsNull = false) => GetEmptyDirectedCollectionValue<T>(allowsNull);

        protected override ICollectionValue<T> GetCollectionValue<T>(SCG.IEnumerable<T> enumerable, bool allowsNull = false) => GetDirectedCollectionValue(enumerable, allowsNull);

        #endregion

        #endregion
        
        #region Test Methods

        #region Properties

        #region Direction

        [Test]
        public void Direction_EmptyCollection_Forwards()
        {
            // Arrange
            var directedCollectionValue = GetEmptyDirectedCollectionValue<string>();

            // Act
            var direction = directedCollectionValue.Direction;

            // Assert
            Assert.That(direction, Is.EqualTo(Forwards));
        }

        [Test]
        public void Direction_RandomCollection_Forwards()
        {
            // Arrange
            var directedCollectionValue = GetStringDirectedCollectionValue(Random);

            // Act
            var direction = directedCollectionValue.Direction;

            // Assert
            Assert.That(direction, Is.EqualTo(Forwards));
        }

        #endregion

        #endregion

        #region Methods

        #region Backwards()

        [Test]
        public void Backwards_EmptyCollection_Expected()
        {
            // Arrange
            var collection = GetEmptyDirectedCollectionValue<string>();
            var expected = new ExpectedDirectedCollectionValue<string>(
                NoStrings,
                ReferenceEqualityComparer,
                collection.AllowsNull,
                direction: Backwards
                );

            // Act
            var backwards = collection.Backwards();

            // Assert
            Assert.That(backwards, Is.EqualTo(expected));
        }

        [Test]
        public void Backwards_RandomCollection_Expected()
        {
            // Arrange
            var collection = GetStringSequence(Random);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection.Reverse(),
                ReferenceEqualityComparer,
                collection.AllowsNull,
                direction: Backwards
                );

            // Act
            var backwards = collection.Backwards();

            // Assert
            Assert.That(backwards, Is.EqualTo(expected));
        }

        [Test]
        public void Backwards_AllowsNull_Expected()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var collection = GetDirectedCollectionValue(items, allowsNull: true);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection.Reverse(),
                ReferenceEqualityComparer,
                collection.AllowsNull,
                direction: Backwards
                );

            // Act
            var backwards = collection.Backwards();

            // Assert
            Assert.That(backwards, Is.EqualTo(expected));
        }

        [Test]
        public void Backwards_BackwardsRandomCollection_Expected()
        {
            // Arrange
            var collection = GetStringSequence(Random);
            var expected = new ExpectedDirectedCollectionValue<string>(
                collection.ToArray(),
                ReferenceEqualityComparer,
                collection.AllowsNull
                );

            // Act
            var backwardsBackwards = collection.Backwards().Backwards();

            // Assert
            Assert.That(backwardsBackwards, Is.EqualTo(expected));
        }

        [Test]
        public void Backwards_ChangeCollectionInvalidatesDirectedCollectionValue_ThrowsInvalidOperationException()
        {
            // Arrange
            var collection = GetStringSequence(Random);
            var array = new string[collection.Count];
            var stringBuilder = new StringBuilder();
            var rest = 0;
            var item = GetString(Random);

            // Act
            var backwards = collection.Backwards();
            ChangeCollection(collection, item);

            // TODO: Refactor into separate DirectCollectionValueConstraint
            // Assert
            Assert.That(() => backwards.AllowsNull, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.Count, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.CountSpeed, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.Direction, Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.IsEmpty, Throws.InvalidOperationException.Because(CollectionWasModified));

            Assert.That(() => backwards.Backwards(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.Choose(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.CopyTo(array, 0), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.GetEnumerator().MoveNext(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.ToArray(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.Show(stringBuilder, ref rest, null), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.ToString(null, null), Throws.InvalidOperationException.Because(CollectionWasModified));

            Assert.That(() => backwards.Equals(null), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.GetHashCode(), Throws.InvalidOperationException.Because(CollectionWasModified));
            Assert.That(() => backwards.ToString(), Throws.InvalidOperationException.Because(CollectionWasModified));
        }

        #endregion

        #endregion

        #endregion
    }
}
