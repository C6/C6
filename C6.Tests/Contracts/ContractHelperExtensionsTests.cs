// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics;
using System.Linq;

using C6.Tests.Helpers;

using NUnit.Framework;

using SC = System.Collections;
using SCG = System.Collections.Generic;

using static C6.Contracts.ContractHelperExtensions;


namespace C6.Tests.Contracts
{
    [TestFixture]
    public sealed class ContractHelperExtensionsTests : TestBase
    {
        #region ContainsRange<T>(this SCG.IEnumerable<T>, SCG.IEnumerable<T>, SCG.IEqualityComparer<T>)

        [Test]
        public void ContainsRange_SecondEmptyEnumerable_True()
        {
            // Arrange
            var first = TestHelper.GetStrings(Random);
            var second = Enumerable.Empty<string>();

            // Act
            var containsRange = first.ContainsRange(second);

            // Assert
            Assert.That(containsRange, Is.True);
        }

        [Test]
        public void ContainsRange_FirstEmptyEnumerable_False()
        {
            // Arrange
            var first = Enumerable.Empty<string>();
            var second = TestHelper.GetStrings(Random);

            // Act
            var containsRange = first.ContainsRange(second);

            // Assert
            Assert.That(containsRange, Is.False);
        }

        [Test]
        public void ContainsRange_Subset_True()
        {
            // Arrange
            var first = new[] {
                new ContainsStruct('A', 0),
                new ContainsStruct('F', 1),
                new ContainsStruct('B', 1),
                new ContainsStruct('G', 2),
                new ContainsStruct('C', 3),
                new ContainsStruct('E', 3),
                new ContainsStruct('H', 3),
                new ContainsStruct('D', 3),
            };
            var second = new[] {
                new ContainsStruct('A', 0),
                new ContainsStruct('B', 1),
                new ContainsStruct('C', 3),
                new ContainsStruct('D', 3),
                new ContainsStruct('E', 3),
            };

            // Act
            var containsRange = first.ContainsRange(second);

            // Assert
            Assert.That(containsRange, Is.True);
        }


        [DebuggerDisplay("{Letter} ({HashCode})")]
        private struct ContainsStruct
        {
            private char Letter { get; }
            private int HashCode { get; }

            public ContainsStruct(char letter, int hashCode)
            {
                Letter = letter;
                HashCode = hashCode;
            }

            public override bool Equals(object obj) => Letter == ((ContainsStruct) obj).Letter;

            public override int GetHashCode() => HashCode;
        }

        #endregion

        #region GetStructComparer<T>

        [Test]
        public void GetStructComparer_TwoNullObjects_Equal()
        {
            var comparer = CreateStructComparer<string>();
            Assert.That(null, Is.EqualTo(null).Using(comparer));
        }

        [Test]
        public void GetStructComparer_OneNullObject_NotEqual()
        {
            var s = Random.GetString();
            var comparer = CreateStructComparer<string>();
            Assert.That(s, Is.Not.EqualTo(null).Using(comparer));
            Assert.That(null, Is.Not.EqualTo(s).Using(comparer));
            Assert.That(s, Is.EqualTo(s).Using(comparer));
        }

        [Test]
        public void GetStructComparer_PairsWithNull_Equal()
        {
            var x = "X";

            var p1 = new Pair<string>(x, x);
            var p2 = new Pair<string>(x, null);

            var comparer = CreateStructComparer<Pair<string>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.Not.EqualTo(p2).Using(comparer));
            Assert.That(p2, Is.Not.EqualTo(p1).Using(comparer));
        }

        [Test]
        public void GetStructComparer_EqualPairOfEqualIntegerPairs_NotEqualUsingComparer()
        {
            var x = new Pair<int>(10, 1);
            var y = new Pair<int>(10, 2);

            var p1 = new Pair<Pair<int>>(x, x);
            var p2 = new Pair<Pair<int>>(x, y);

            var comparer = CreateStructComparer<Pair<Pair<int>>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.Not.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_EqualPairOfSameIntegerPairs_Equal()
        {
            var x = new Pair<int>(10, 1);

            var p1 = new Pair<Pair<int>>(x, x);
            var p2 = new Pair<Pair<int>>(x, x);

            var comparer = CreateStructComparer<Pair<Pair<int>>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_EqualsEqualIntegerPairs_NotEqualUsingComparer()
        {
            var p1 = new Pair<int>(10, 1);
            var p2 = new Pair<int>(10, 2);

            var comparer = CreateStructComparer<Pair<int>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.Not.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_DifferentIntegerPairs_NotEqual()
        {
            var p1 = new Pair<int>(-10, 1);
            var p2 = new Pair<int>(10, 1);

            var comparer = CreateStructComparer<Pair<int>>();
            Assert.That(p1, Is.Not.EqualTo(p2));
            Assert.That(p1, Is.Not.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_EqualIntegerPairs_Equal()
        {
            var p1 = new Pair<int>(10, 1);
            var p2 = new Pair<int>(10, 1);

            var comparer = CreateStructComparer<Pair<int>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_SameIntegerPairs_Equal()
        {
            var p = new Pair<int>(10, 1);

            var comparer = CreateStructComparer<Pair<int>>();
            Assert.That(p, Is.EqualTo(p));
            Assert.That(p, Is.EqualTo(p).Using(comparer));
        }

        [Test]
        public void GetStructComparer_EqualPairOfEqualStringPairs_NotEqualUsingComparer()
        {
            var x = "X";
            var y = string.Copy(x);

            var p1 = new Pair<string>(x, x);
            var p2 = new Pair<string>(x, y);

            var comparer = CreateStructComparer<Pair<string>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.Not.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_EqualPairOfSameStringPairs_Equal()
        {
            var x = "X";

            var p1 = new Pair<string>(x, x);
            var p2 = new Pair<string>(x, x);

            var comparer = CreateStructComparer<Pair<string>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_EqualsEqualStringPairs_NotEqualUsingComparer()
        {
            var x = "X";
            var y = "Y";

            var p1 = new Pair<string>(x, x);
            var p2 = new Pair<string>(x, y);

            var comparer = CreateStructComparer<Pair<string>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.Not.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_EqualStringPairs_Equal()
        {
            var x = "X";
            var y = "Y";

            var p1 = new Pair<string>(x, y);
            var p2 = new Pair<string>(x, y);

            var comparer = CreateStructComparer<Pair<string>>();
            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1, Is.EqualTo(p2).Using(comparer));
        }

        [Test]
        public void GetStructComparer_SameStringPairs_Equal()
        {
            var x = "X";
            var y = "Y";
            var p = new Pair<string>(x, y);

            var comparer = CreateStructComparer<Pair<string>>();
            Assert.That(p, Is.EqualTo(p));
            Assert.That(p, Is.EqualTo(p).Using(comparer));
        }

        [Test]
        public void GetStructComparer_DifferentStringPairs_NotEqual()
        {
            var x = "X";
            var y = "Y";

            var p1 = new Pair<string>(x, y);
            var p2 = new Pair<string>(y, y);

            var comparer = CreateStructComparer<Pair<string>>();
            Assert.That(p1, Is.Not.EqualTo(p2));
            Assert.That(p1, Is.Not.EqualTo(p2).Using(comparer));
        }

        #region Nested Types

        private struct Pair<T>
        {
            public Pair(T x, T y)
            {
                X = x;
                Y = y;
            }

            public T X { get; }
            public T Y { get; }

            public override bool Equals(object obj) => X.Equals(((Pair<T>) obj).X);

            public override int GetHashCode() => SCG.EqualityComparer<T>.Default.GetHashCode(X);

            public override string ToString() => $"{X}/{Y}";
        }


        [Test]
        public void Equals_EqualPairs_Equal()
        {
            var x = new Pair<int>(10, 1);
            var y = new Pair<int>(10, 1);

            Assert.That(x, Is.EqualTo(y));
        }

        [Test]
        public void Equals_SamePair_Equals()
        {
            var x = new Pair<int>(10, 1);

            Assert.That(x, Is.EqualTo(x));
        }

        [Test]
        public void Equals_EqualXPairs_Equal()
        {
            var x = new Pair<int>(10, 1);
            var y = new Pair<int>(10, 2);

            Assert.That(x, Is.EqualTo(y));
        }

        [Test]
        public void Equals_DifferentXPairs_NotEqual()
        {
            var x = new Pair<int>(-10, 1);
            var y = new Pair<int>(10, 1);

            Assert.That(x, Is.Not.EqualTo(y));
        }

        #endregion

        #endregion

        #region UnsequenceEqual TestCases

        private static SC.IEnumerable EqualTestCases => new[] {
            new TestCaseData(null, null),
            new TestCaseData(new int[] { }, new int[] { }),
            new TestCaseData(new[] { 1 }, new[] { 1 }),
            new TestCaseData(new[] { 1, 1, 1, 1, 1 }, new[] { 1, 1, 1, 1, 1 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 5, 4, 3, 2, 1 }),
            new TestCaseData(new[] { 1, 2, 3, 4, 5 }, new[] { 3, 4, 1, 5, 2 }),
        };

        private static SC.IEnumerable UnequalTestCases => new[] {
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
            Func<string> getString = () => Random.GetString();
            var items = getString.Repeat(count);
            var shuffledItems = items.ShuffledCopy(Random);

            // Act
            var result = items.UnsequenceEqual(shuffledItems);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion
    }
}