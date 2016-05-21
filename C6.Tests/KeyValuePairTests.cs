// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using C6.Tests.Helpers;

using NUnit.Framework;

using SCG = System.Collections.Generic;


namespace C6.Tests
{
    [TestFixture]
    public class KeyValuePairTests : TestBase
    {
        #region Constructors

        [Test]
        public void Constructor_OnlyKey_KeyIsGivenValue()
        {
            // Arrange
            var key = Random.Next();

            // Act
            var kvp = new KeyValuePair<int, string>(key);

            // Assert
            Assert.That(kvp.Key, Is.EqualTo(key));
        }


        [Test]
        public void Constructor_OnlyKey_ValueIsDefault()
        {
            // Act
            var kvp = new KeyValuePair<int, string>(1);

            // Assert
            Assert.That(kvp.Value, Is.EqualTo(default(string)));
        }


        [Test]
        public void Constructor_KeyAndValue_KeyAndValueHasGivenValues()
        {
            // Arrange
            var key = Random.Next();
            var value = Random.GetString(10);

            // Act
            var kvp = new KeyValuePair<int, string>(key, value);

            // Assert
            Assert.That(kvp.Key, Is.EqualTo(key));
            Assert.That(kvp.Value, Is.EqualTo(value));
        }

        #endregion

        #region ToString

        [Test]
        public void ToString_RandomPair_ContainsKeyBeforeValue()
        {
            // Arrange
            var key = Random.Next();
            var value = Random.GetString(10);
            var kvp = new KeyValuePair<int, string>(key, value);
            var toString = kvp.ToString();

            // Act
            var keyIndex = toString.IndexOf(key.ToString(), StringComparison.Ordinal);
            var valueIndex = toString.IndexOf(value, StringComparison.Ordinal);

            // Assert
            Assert.That(keyIndex, Is.Not.Negative.And.LessThan(valueIndex));
        }

        #endregion

        #region Equals

        [Test]
        public void Equals_EqualPairs_True()
        {
            // Arrange
            var key = Random.Next();
            var value = Random.GetString(10);
            var kvp1 = new KeyValuePair<int, string>(key, value);
            var kvp2 = new KeyValuePair<int, string>(key, value);

            // Act
            var result = kvp1.Equals(kvp2);

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_DifferentKeys_False()
        {
            // Arrange
            var negativeKey = Random.Next(int.MinValue, 0);
            var positiveKey = Random.Next(0, int.MaxValue);
            var kvp1 = new KeyValuePair<int, string>(negativeKey);
            var kvp2 = new KeyValuePair<int, string>(positiveKey);

            // Act
            var result = kvp1.Equals(kvp2);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_DifferentValues_False()
        {
            // Arrange
            var key = Random.Next();
            var lowercaseValue = Random.GetString(10, "abcdefghijkmnopqrstuvwxyz");
            var uppercaseValue = Random.GetString(10, "ABCDEFGHJKLMNOPQRSTUVWXYZ");
            var kvp1 = new KeyValuePair<int, string>(key, lowercaseValue);
            var kvp2 = new KeyValuePair<int, string>(key, uppercaseValue);

            // Act
            var result = kvp1.Equals(kvp2);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_Null_False()
        {
            // Arrange
            var key = Random.Next();
            var value = Random.GetString(10);
            var kvp1 = new KeyValuePair<int, string>(key, value);

            // Act
            var result = kvp1.Equals(null);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_SystemCollectionsGenericKeyValuePair_False()
        {
            // Arrange
            var key = Random.Next();
            var value = Random.GetString(10);
            var c6Kvp = new KeyValuePair<int, string>(key, value);
            var scgKvp = new SCG.KeyValuePair<int, string>(key, value);

            // Act
            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = c6Kvp.Equals(scgKvp);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region GetHashCode

        [Test]
        public void GetHashCode_DefaultPairs_EqualHashCodes()
        {
            // Arrange
            var kvp1 = new KeyValuePair<int, string>();
            var kvp2 = new KeyValuePair<int, string>();

            // Act
            var hashCode1 = kvp1.GetHashCode();
            var hashCode2 = kvp2.GetHashCode();

            // Assert
            Assert.That(hashCode1, Is.EqualTo(hashCode2));
        }


        [Test]
        public void GetHashCode_EqualPairs_EqualHashCodes()
        {
            // Arrange
            var key = Random.Next();
            var value = Random.GetString(10);
            var kvp1 = new KeyValuePair<int, string>(key, value);
            var kvp2 = new KeyValuePair<int, string>(key, value);

            // Act
            var hashCode1 = kvp1.GetHashCode();
            var hashCode2 = kvp2.GetHashCode();

            // Assert
            Assert.That(hashCode1, Is.EqualTo(hashCode2));
        }

        #endregion

        #region Operator Equality

        [Test]
        public void OperatorEquality_EqualPairs_True()
        {
            // Arrange
            var key = Random.Next();
            var value = Random.GetString(10);
            var kvp1 = new KeyValuePair<int, string>(key, value);
            var kvp2 = new KeyValuePair<int, string>(key, value);

            // Act
            var result = kvp1 == kvp2;

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void OperatorEquality_DifferentKeys_False()
        {
            // Arrange
            var negativeKey = Random.Next(int.MinValue, 0);
            var positiveKey = Random.Next(0, int.MaxValue);
            var kvp1 = new KeyValuePair<int, string>(negativeKey);
            var kvp2 = new KeyValuePair<int, string>(positiveKey);

            // Act
            var result = kvp1 == kvp2;

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void OperatorEquality_DifferentValues_False()
        {
            // Arrange
            var key = Random.Next();
            var lowercaseValue = Random.GetString(10, "abcdefghijkmnopqrstuvwxyz");
            var uppercaseValue = Random.GetString(10, "ABCDEFGHJKLMNOPQRSTUVWXYZ");
            var kvp1 = new KeyValuePair<int, string>(key, lowercaseValue);
            var kvp2 = new KeyValuePair<int, string>(key, uppercaseValue);

            // Act
            var result = kvp1 == kvp2;

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region Operator Inequality

        [Test]
        public void OperatorInequality_EqualPairs_True()
        {
            // Arrange
            var key = Random.Next();
            var value = Random.GetString(10);
            var kvp1 = new KeyValuePair<int, string>(key, value);
            var kvp2 = new KeyValuePair<int, string>(key, value);

            // Act
            var result = kvp1 != kvp2;

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void OperatorInequality_DifferentKeys_False()
        {
            // Arrange
            var negativeKey = Random.Next(int.MinValue, 0);
            var positiveKey = Random.Next(0, int.MaxValue);
            var kvp1 = new KeyValuePair<int, string>(negativeKey);
            var kvp2 = new KeyValuePair<int, string>(positiveKey);

            // Act
            var result = kvp1 != kvp2;

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void OperatorInequality_DifferentValues_False()
        {
            // Arrange
            var key = Random.Next();
            var lowercaseValue = Random.GetString(10, "abcdefghijkmnopqrstuvwxyz");
            var uppercaseValue = Random.GetString(10, "ABCDEFGHJKLMNOPQRSTUVWXYZ");
            var kvp1 = new KeyValuePair<int, string>(key, lowercaseValue);
            var kvp2 = new KeyValuePair<int, string>(key, uppercaseValue);

            // Act
            var result = kvp1 != kvp2;

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        // TODO: Test IShowable members
        // TODO: Test IFormattable members
    }
}