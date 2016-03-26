// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Linq;

using C6.Contracts;
using C6.Tests.Collections;

using NUnit.Framework;


namespace C6.Tests.Contracts
{
    [TestFixture]
    public sealed class ContractHelperExtensionsTests : TestBase
    {
        #region UnsequenceEqual TestCases

        private static IEnumerable EqualTestCases => new[] {
            new TestCaseData(null, null),
            new TestCaseData(new int[] { }, new int[] { }),
            new TestCaseData(new[] { 1 }, new[] { 1 }),
            new TestCaseData(new[] { 1, 1, 1, 1, 1 }, new[] { 1, 1, 1, 1, 1 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 5, 4, 3, 2, 1 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 3, 4, 1, 5, 2 }),
        };

        private static IEnumerable UnequalTestCases => new[] {
            new TestCaseData(null, new int[] { }),
            new TestCaseData(new int[] { }, null),
            new TestCaseData(new int[] { }, new[] { 1 }),
            new TestCaseData(new[] { 0 }, new[] { 1 }),
            new TestCaseData(new[] { 1 }, new[] { 1, 1, 1, 1, 1 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 3, 4, 5 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 3, 5 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 5, 4, 3, 3, 2, 1 }),
        };

        #endregion

        #region UnsequenceEqual<T>(this SCG.IEnumerable<T>, SCG.IEnumerable<T>)

        [Test]
        [TestCaseSource(nameof(EqualTestCases))]
        public void UnsequenceEqual_Equals_True(int[] first, int[] second)
        {
            // Act
            var result = first.UnsequenceEqual(second);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(UnequalTestCases))]
        public void UnsequenceEqual_NotEquals_False(int[] first, int[] second)
        {
            // Act
            var result = first.UnsequenceEqual(second);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region UnsequenceEqual<T>(this SCG.IEnumerable<T>, SCG.IEnumerable<T>, SCG.IEqualityComparer<T>)

        [Test]
        [TestCaseSource(nameof(EqualTestCases))]
        public void UnsequenceEqual_CustomComparerEquals_True(int[] first, int[] second)
        {
            // Arrange
            var comparer = ComparerFactory.CreateEqualityComparer((x, y) => x == y, (int x) => x.GetHashCode());

            // Act
            var result = first.UnsequenceEqual(second, comparer);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(UnequalTestCases))]
        public void UnsequenceEqual_CustomComparerNotEquals_False(int[] first, int[] second)
        {
            // Arrange
            var comparer = ComparerFactory.CreateEqualityComparer((x, y) => x == y, (int x) => x.GetHashCode());

            // Act
            var result = first.UnsequenceEqual(second, comparer);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase(new[] { "" }, new[] { "" }, true)]
        [TestCase(new[] { "" }, new[] { "Hello" }, false)]
        [TestCase(new[] { "Why", "Go", "Hello", "World", "Every time?" }, new[] { "Hallo", "Welt", "Wie", "Geht", "Es?" }, true)]
        public void UnsequenceEqual_IntialComparer_True(string[] first, string[] second, bool expectedResult)
        {
            // Arrange
            Func<string, string> firstLetterOrEmpty = s => string.IsNullOrEmpty(s) ? string.Empty : s.Substring(0, 1);
            Func<string, string, bool> equals = (x, y) => firstLetterOrEmpty(x).Equals(firstLetterOrEmpty(y));
            Func<string, int> getHashCode = x => firstLetterOrEmpty(x).GetHashCode();
            var comparer = ComparerFactory.CreateEqualityComparer(equals, getHashCode);

            // Act
            var result = first.UnsequenceEqual(second, comparer);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCaseSource(nameof(EqualTestCases))]
        public void UnsequenceEqual_EqualHashCodesEquals_True(int[] first, int[] second)
        {
            // Arrange
            Func<int, int, bool> equals = (x, y) => x == y;
            Func<int, int> getEqualHashCode = x => 0;
            var comparer = ComparerFactory.CreateEqualityComparer(equals, getEqualHashCode);

            // Act
            var result = first.UnsequenceEqual(second, comparer);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(UnequalTestCases))]
        public void UnsequenceEqual_EqualHashCodesNotEquals_False(int[] first, int[] second)
        {
            // Arrange
            Func<int, int, bool> equals = (x, y) => x == y;
            Func<int, int> getEqualHashCode = x => 0;
            var comparer = ComparerFactory.CreateEqualityComparer(equals, getEqualHashCode);

            // Act
            var result = first.UnsequenceEqual(second, comparer);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UnsequenceEqual_LargeEquals_True()
        {
            // Arrange
            const int count = 10000;
            var randomStrings = Enumerable.Range(0, count).Select(i => Random.GetString()).ToList();
            var first = randomStrings.ToList();
            var second = randomStrings.ToList();
            second.Shuffle(Random);

            // Act
            var result = first.UnsequenceEqual(second);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion
    }
}