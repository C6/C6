// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using C6.Tests.Contracts;

using NUnit.Framework;

using static C6.Contracts.ContractMessage;
using static C6.EnumerationDirection;


namespace C6.Tests
{
    [TestFixture]
    public class EnumerationDirectionExtensionTests
    {
        #region IsForward()

        [Test]
        public void IsForward_Forwards_True()
        {
            // Act
            var isForward = Forwards.IsForward();

            // Assert
            Assert.That(isForward, Is.True);
        }

        [Test]
        public void IsForward_Backwards_False()
        {
            // Act
            var isForward = Backwards.IsForward();

            // Assert
            Assert.That(isForward, Is.False);
        }

        [Test]
        public void IsForward_InvalidValue_ViolatesPrecondition()
        {
            // Arrange
            var invalidEnumerationDirection = (EnumerationDirection) 2;

            // Act & Assert
            Assert.That(() => invalidEnumerationDirection.IsForward(), Violates.PreconditionSaying(EnumMustBeDefined));
        }

        #endregion

        #region IsOppositeOf(EnumerationDirection)

        [Test]
        public void IsOppositeOf_InvalidEnumerationDirection_ViolatesPrecondtion()
        {
            // Arrange
            var invalidEnumerationDirection = (EnumerationDirection) 2;

            // Act & Assert
            Assert.That(() => invalidEnumerationDirection.IsOppositeOf(Forwards), Violates.PreconditionSaying(EnumMustBeDefined));
        }

        [Test]
        public void IsOppositeOf_OtherInvalidEnumerationDirection_ViolatesPrecondtion()
        {
            // Arrange
            var invalidEnumerationDirection = (EnumerationDirection) 2;

            // Act & Assert
            Assert.That(() => Forwards.IsOppositeOf(invalidEnumerationDirection), Violates.PreconditionSaying(EnumMustBeDefined));
        }

        [Test]
        [TestCase(Forwards, Forwards, false)]
        [TestCase(Forwards, Backwards, true)]
        [TestCase(Backwards, Forwards, true)]
        [TestCase(Backwards, Backwards, false)]
        public void IsOppositeOf_AllPossibleCombinations_Result(EnumerationDirection oneDirection, EnumerationDirection otherDirection, bool result)
        {
            // Act
            var isOppositeOf = oneDirection.IsOppositeOf(otherDirection);

            // Assert
            Assert.That(isOppositeOf, Is.EqualTo(result));
        }

        #endregion
    }
}