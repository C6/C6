// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;
using System.Text;

using C6.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Internal;

using static C6.EnumerationDirection;
using static C6.Collections.ExceptionMessages;
using static C6.Tests.Helpers.TestHelper;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public abstract class ISequencedTests : ICollectionTests
    {
        #region Factories

        protected abstract ISequenced<T> GetEmptySequence<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        protected abstract ISequenced<T> GetSequence<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false);

        private IDirectedCollectionValue<T> GetEmptyDirectedCollectionValue<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
            => GetEmptySequence(equalityComparer, allowsNull);

        private IDirectedCollectionValue<T> GetDirectedCollectionValue<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false)
            => GetSequence(enumerable, equalityComparer, allowsNull);

        #region Helpers

        private ISequenced<int> GetIntSequence(Random random, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetIntegers(random, GetCount(random)), equalityComparer, allowsNull);

        private ISequenced<int> GetIntSequence(Random random, int count, SCG.IEqualityComparer<int> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetIntegers(random, count), equalityComparer, allowsNull);

        private ISequenced<string> GetStringSequence(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        private ISequenced<string> GetStringSequence(Randomizer random, int count, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetStrings(random, count), equalityComparer, allowsNull);

        private IDirectedCollectionValue<string> GetStringDirectedCollectionValue(Randomizer random, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetStrings(random, GetCount(random)), equalityComparer, allowsNull);

        private IDirectedCollectionValue<string> GetStringDirectedCollectionValue(Randomizer random, int count, SCG.IEqualityComparer<string> equalityComparer = null, bool allowsNull = false)
            => GetSequence(GetStrings(random, count), equalityComparer, allowsNull);

        #endregion

        #region Inherited

        protected override ICollection<T> GetEmptyCollection<T>(SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetEmptySequence(equalityComparer, allowsNull);

        protected override ICollection<T> GetCollection<T>(SCG.IEnumerable<T> enumerable, SCG.IEqualityComparer<T> equalityComparer = null, bool allowsNull = false) => GetSequence(enumerable, equalityComparer, allowsNull);

        #endregion

        #endregion

        #region Test Methods

        #region IDirectedCollectionValue<T>

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
            var collection = GetEmptySequence<string>();
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
            var collection = GetSequence(items, allowsNull: true);
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

            // Act
            var backwards = collection.Backwards();
            collection.UpdateOrAdd(Random.GetString());

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

        #region ISeqenced<T>

        #region Methods

        #region GetSequencedHashCode()

        [Test]
        public void GetSequencedHashCode_EmptySequence_GeneratedHashCode()
        {
            // Arrange
            var sequence = GetEmptySequence<string>();
            var expected = SequencedEqualityComparer.GetSequencedHashCode(sequence);

            // Act
            var sequencedHashCode = sequence.GetSequencedHashCode();

            // Assert
            Assert.That(sequencedHashCode, Is.EqualTo(expected));
        }

        [Test]
        public void GetSequencedHashCode_RandomSequence_GeneratedHashCode()
        {
            // Arrange
            var sequence = GetStringSequence(Random);
            var expected = SequencedEqualityComparer.GetSequencedHashCode(sequence);

            // Act
            var sequencedHashCode = sequence.GetSequencedHashCode();

            // Assert
            Assert.That(sequencedHashCode, Is.EqualTo(expected));
        }

        [Test]
        public void GetSequencedHashCode_RandomSequenceWithNull_GeneratedHashCode()
        {
            // Arrange
            var items = GetStrings(Random).WithNull(Random);
            var sequence = GetSequence(items, allowsNull: true);
            var expected = SequencedEqualityComparer.GetSequencedHashCode(sequence);

            // Act
            var sequencedHashCode = sequence.GetSequencedHashCode();

            // Assert
            Assert.That(sequencedHashCode, Is.EqualTo(expected));
        }

        [Test]
        public void GetSequencedHashCode_EqualSequenceDifferentOrder_LikelyNotEqual()
        {
            // Arrange
            var items = GetStrings(Random);
            var firstSequence = GetSequence(items);
            var shuffledItems = items.ShuffledCopy(Random);
            var secondSequence = GetSequence(shuffledItems);
            var expected = firstSequence.GetSequencedHashCode(null) == secondSequence.GetSequencedHashCode(null);

            // Act
            var firstSequencedHashCode = firstSequence.GetSequencedHashCode();
            var secondSequencedHashCode = secondSequence.GetSequencedHashCode();
            var equals = firstSequencedHashCode == secondSequencedHashCode;

            // Assert
            Assert.That(equals, Is.EqualTo(expected));
        }

        [Test]
        public void GetSequencedHashCode_EqualButChangedSequence_SameHashCode()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var newItems = GetLowercaseStrings(Random);
            var sequence = GetSequence(items);

            // Act
            var firstSequencedHashCode = sequence.GetSequencedHashCode();
            sequence.AddRange(newItems);
            sequence.RemoveRange(newItems);
            var secondSequencedHashCode = sequence.GetSequencedHashCode();

            // Assert
            Assert.That(firstSequencedHashCode, Is.EqualTo(secondSequencedHashCode));
        }

        [Test]
        public void GetSequencedHashCode_CachedValueIsUpdated_ExpectedHashCode()
        {
            // Arrange
            var sequence = GetStringSequence(Random, ReferenceEqualityComparer);
            var items = GetStrings(Random);
            var expected = GetSequence(items).GetSequencedHashCode();

            // Act
            var hashCode = sequence.GetSequencedHashCode();
            sequence.Clear();
            sequence.AddRange(items);
            hashCode = sequence.GetSequencedHashCode();

            // Assert
            Assert.That(hashCode, Is.EqualTo(expected));
        }

        // TODO: Test for shuffled list in IListTests

        #endregion

        #region SequencedEquals(ISequence<T>)

        [Test]
        public void SequencedEquals_EmptySequences_True()
        {
            // Arrange
            var sequence = GetEmptySequence<string>();
            var otherSequence = GetEmptySequence<string>();

            // Act
            var sequencedEquals = sequence.SequencedEquals(otherSequence);

            // Assert
            Assert.That(sequencedEquals, Is.True);
        }

        [Test]
        public void SequencedEquals_EmptyAndNullSequence_False()
        {
            // Arrange
            var sequence = GetEmptySequence<string>();

            // Act
            var sequencedEquals = sequence.SequencedEquals(null);

            // Assert
            Assert.That(sequencedEquals, Is.False);
        }

        [Test]
        public void SequencedEquals_EmptyAndRandomSequence_False()
        {
            // Arrange
            var sequence = GetEmptySequence<string>();
            var otherSequence = GetStringSequence(Random);

            // Act
            var sequencedEquals = sequence.SequencedEquals(otherSequence);

            // Assert
            Assert.That(sequencedEquals, Is.False);
        }

        [Test]
        public void SequencedEquals_RandomSequences_False()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var sequence = GetSequence(items);
            var otherItems = GetLowercaseStrings(Random);
            var otherSequence = GetSequence(otherItems);

            // Act
            var sequencedEquals = sequence.SequencedEquals(otherSequence);

            // Assert
            Assert.That(sequencedEquals, Is.False);
        }

        [Test]
        public void SequencedEquals_RandomSequenceAndItSelf_True()
        {
            // Arrange
            var sequence = GetStringSequence(Random);

            // Act
            var sequencedEquals = sequence.SequencedEquals(sequence);

            // Assert
            Assert.That(sequencedEquals, Is.True);
        }

        [Test]
        public void SequencedEquals_EqualSequences_True()
        {
            // Arrange
            var items = GetStrings(Random);
            var sequence = GetSequence(items);
            var otherSequence = GetSequence(items);

            // Act
            var sequencedEquals = sequence.SequencedEquals(otherSequence);

            // Assert
            Assert.That(sequencedEquals, Is.True);
        }

        [Test]
        public void SequencedEquals_SequencedEqualSequences_False()
        {
            // Arrange
            var items = GetStrings(Random);
            var sequence = GetSequence(items);
            var shuffledItems = items.ShuffledCopy(Random);
            var otherSequence = GetSequence(shuffledItems);

            // Act
            var sequencedEquals = sequence.SequencedEquals(otherSequence);

            // Assert
            Assert.That(sequencedEquals, Is.False);
        }

        [Test]
        public void SequencedEquals_DifferentEqualityComparers_TrueInOneDirection()
        {
            // Arrange
            var items = GetUppercaseStrings(Random);
            var sequence = GetSequence(items, CaseInsensitiveStringComparer.Default);
            var otherItems = items.Select(item => item.ToLower());
            var otherSequence = GetSequence(otherItems);

            // Act
            var sequenceSequencedEqualsOtherSequence = sequence.SequencedEquals(otherSequence);
            var otherSequenceSequencedEqualsSequence = otherSequence.SequencedEquals(sequence);

            // Assert
            Assert.That(sequenceSequencedEqualsOtherSequence, Is.True);
            Assert.That(otherSequenceSequencedEqualsSequence, Is.False);
        }

        [Test]
        public void SequencedEquals_EqualItemsButDifferentMultiplicity_False()
        {
            // Arrange
            var items = GetStrings(Random);
            var sequence = GetSequence(items);
            var otherItems = items.SelectMany(item => item.Repeat(Random.Next(2, 4)));
            var otherSequence = GetSequence(otherItems);

            // Act
            var sequencedEquals = sequence.SequencedEquals(otherSequence);

            // Assert
            Assert.That(sequencedEquals, Is.False);
        }

        [Test]
        public void SequencedEquals_EqualHashButDifferentItems_False()
        {
            // Arrange
            var items = new[] { -1657792980, -1570288808 };
            var sequence = GetSequence(items);
            var otherItems = new[] { 1862883298, 957896270 };
            var otherSequence = GetSequence(otherItems);

            // Act
            var sequencedHashCode = sequence.GetSequencedHashCode();
            var otherSequencedHashCode = otherSequence.GetSequencedHashCode();
            var sequencedEquals = sequence.SequencedEquals(otherSequence);

            // Assert
            Assert.That(sequencedHashCode, Is.EqualTo(otherSequencedHashCode));
            Assert.That(sequencedEquals, Is.False);
        }

        [Test]
        [Category("Unfinished")]
        public void SequencedEquals_Set_Fail()
        {
            Assert.That(!AllowsDuplicates, Is.False, "Tests have not been written yet");
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}