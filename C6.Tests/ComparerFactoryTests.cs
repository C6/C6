// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using NUnit.Framework;

using static C6.ComparerFactory;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public class ComparerFactoryTests
    {
        #region GetStructComparer<T>

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
            var y = (x + "Y").Substring(0, 1);

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

        #endregion

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
    }
}