// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using C6.Tests.Contracts;

using NUnit.Framework;

using static C6.Contracts.ContractMessage;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        #region IsSorted TestCases

        private static SC.IEnumerable SortedTestCases => new[] {
            new TestCaseData(new int[] { }),
            new TestCaseData(new[] { 1 }),
            new TestCaseData(new[] { 0, 0, 0 }),
            new TestCaseData(new[] { 0, 1, 2, 3 }),
            new TestCaseData(new[] { 3, 5, 5, 6, 7 }),
            new TestCaseData(new[] { -7, -6, -5, -5, -3 }),
            new TestCaseData(new[] { int.MinValue, 0, int.MaxValue }),
        };

        private static SC.IEnumerable NotSortedTestCases => new[] {
            new TestCaseData(new[] { 1, 0 }),
            new TestCaseData(new[] { 3, 2, 1, 0 }),
            new TestCaseData(new[] { 3, 5, 5, 6, 5 }),
            new TestCaseData(new[] { -3, -5, -5, -6, -7 }),
            new TestCaseData(new[] { int.MaxValue, int.MinValue }),
        };

        private static SC.IEnumerable ReverseSortedTestCases => new[] {
            new TestCaseData(new int[] { }),
            new TestCaseData(new[] { 1 }),
            new TestCaseData(new[] { 0, 0, 0 }),
            new TestCaseData(new[] { 3, 2, 1, 0 }),
            new TestCaseData(new[] { 7, 6, 5, 5, 3 }),
            new TestCaseData(new[] { int.MaxValue, int.MinValue }),
        };

        #endregion

        #region IsSorted<T>(this SCG.IEnumerable<T>)

        [Test]
        public void IsSorted_DefaultComparerWithNullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            SCG.IEnumerable<int> enumerable = null;

            // Act & Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.That(() => enumerable.IsSorted(), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        [TestCaseSource(nameof(SortedTestCases))]
        public void IsSorted_DefaultComparer_IsSorted(int[] array)
        {
            // Act
            var isSorted = array.IsSorted();

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(NotSortedTestCases))]
        public void IsSorted_DefaultComparer_IsNotSorted(int[] array)
        {
            // Act
            var isSorted = array.IsSorted();

            // Assert
            Assert.That(isSorted, Is.False);
        }

        #endregion

        #region IsSorted<T>(this SCG.IEnumerable<T>, SCG.IComparer<T>)

        [Test]
        public void IsSorted_CustomComparerWithNullEnumerable_ViolatesPrecondition()
        {
            // Arrange
            SCG.IEnumerable<int> enumerable = null;
            var comparer = ComparerFactory.CreateComparer<int>((x, y) => x.CompareTo(y));

            // Act & Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.That(() => enumerable.IsSorted(comparer), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        [TestCaseSource(nameof(SortedTestCases))]
        public void IsSorted_NullComparer_IsSorted(int[] array)
        {
            // Arrange
            SCG.IComparer<int> comparer = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var isSorted = array.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(NotSortedTestCases))]
        public void IsSorted_NullComparer_IsNotSorted(int[] array)
        {
            // Arrange
            SCG.IComparer<int> comparer = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var isSorted = array.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        [TestCaseSource(nameof(SortedTestCases))]
        public void IsSorted_CustomComparer_IsSorted(int[] array)
        {
            // Arrange
            var comparer = ComparerFactory.CreateComparer<int>((x, y) => x.CompareTo(y));

            // Act
            var isSorted = array.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(NotSortedTestCases))]
        public void IsSorted_CustomComparer_IsNotSorted(int[] array)
        {
            // Arrange
            var comparer = ComparerFactory.CreateComparer<int>((x, y) => x.CompareTo(y));

            // Act
            var isSorted = array.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        [TestCaseSource(nameof(ReverseSortedTestCases))]
        public void IsSorted_ReversedComparer_IsSortedInReverse(int[] array)
        {
            // Arrange
            var comparer = ComparerFactory.CreateComparer<int>((x, y) => y.CompareTo(x));

            // Act
            var isSorted = array.IsSorted(comparer);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        #endregion

        #region IsSorted<T>(this SCG.IEnumerable<T>, Comparison<T>)

        [Test]
        public void IsSorted_NullComparison_ViolatesPrecondition()
        {
            // Arrange
            var enumerable = new[] { 1, 2, 3, 4 };
            Comparison<int> comparison = null;

            // Act & Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.That(() => enumerable.IsSorted(comparison), Violates.PreconditionSaying(ArgumentMustBeNonNull));
        }

        [Test]
        [TestCaseSource(nameof(SortedTestCases))]
        public void IsSorted_Comparison_IsSorted(int[] array)
        {
            // Arrange
            Comparison<int> comparison = (x, y) => x.CompareTo(y);

            // Act
            var isSorted = array.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(NotSortedTestCases))]
        public void IsSorted_Comparison_IsNotSorted(int[] array)
        {
            // Arrange
            Comparison<int> comparison = (x, y) => x.CompareTo(y);

            // Act
            var isSorted = array.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.False);
        }

        [Test]
        [TestCaseSource(nameof(ReverseSortedTestCases))]
        public void IsSorted_ReversedComparison_IsSortedInReverse(int[] array)
        {
            // Arrange
            Comparison<int> comparison = (x, y) => y.CompareTo(x);

            // Act
            var isSorted = array.IsSorted(comparison);

            // Assert
            Assert.That(isSorted, Is.True);
        }

        #endregion
    }
}