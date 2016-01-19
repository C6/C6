using C6.Tests.Contracts;
using NUnit.Framework;
using static C6.EnumerationDirection;



namespace C6.Tests.Enums
{
    [TestFixture]
    public class EnumerationDirectionExtensionTests
    {
        [Test]
        public void IsForward_Forwards_True()
            => Assert.That(Forwards.IsForward(), Is.True);

        [Test]
        public void IsForward_Backwards_False()
            => Assert.That(Backwards.IsForward(), Is.False);

        [Test]
        public void IsForward_InvalidValue_ViolatesPrecondition()
        {
            var invalidEnumerationDirection = (EnumerationDirection)2;

            Assert.That(() => invalidEnumerationDirection.IsForward(), Violates.Precondition);
        }
    }
}